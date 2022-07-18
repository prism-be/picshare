import {useEffect, useState} from "react";
import {getData, IFlow, IPictureSummary, performRefreshToken} from "../../lib/ajaxHelper";
import {Thumbnail} from "./Thumbnail";
import useSWR from "swr";
import {format, parseISO, parseJSON} from "date-fns";
import {useRouter} from "next/router";
import {getCurrentLocale} from "../../lib/locales";

const getFlow = async (route: string) : Promise<IFlow> => {

    await performRefreshToken();

    const response = await getData(route);

    if (response.status === 200) {
        return response.data;
    }

    return {
        organisationId: '',
        pictures: []
    };
}

interface IGroupedFlow
{
    date: Date;
    day: string;
    pictures: IPictureSummary[];
}

const Flow = () => {

    const router = useRouter();
    
    const {data: flow} = useSWR("/api/pictures/flow", getFlow);
    
    const [groupedFlows, setGroupedFlows] = useState<IGroupedFlow[]>([]);

    useEffect(() => {
        if (flow) {

            let data: IGroupedFlow[] = [];

            flow.pictures.forEach((picture) => {
                const pictureDate = parseJSON(picture.date);
                const day: string = format(pictureDate, 'yyyy-MM-dd');

                let existing = data.find(x => x.day === day);

                if (!existing)
                {
                    existing = {
                        date: parseISO(day),
                        day,
                        pictures: []
                    }
                    data.push(existing);
                }
                
                existing.pictures.push(picture);
            })
            
            console.log(router.locale);
            
            setGroupedFlows(data);
        }
    }, [flow])


    return <>
        <div className="">
            {groupedFlows && groupedFlows.map(groupedFlow => <div key={groupedFlow.day} className="pb-5">
                <h1 className="text-gray-500">
                    {format(groupedFlow.date, 'EEEE d MMMM yyyy', {
                        locale: getCurrentLocale()
                    })}
                </h1>
                <div className="grid grid-cols-3 sm:grid-cols-4 md:grid-cols-6 lg:grid-cols-10 xl:grid-cols-12 gap-1">
                    { groupedFlow.pictures && groupedFlow.pictures.map(picture => <Thumbnail picture={picture} key={picture.id}/>)}
                </div>
            </div>)}
        </div>
    </>
}

export default Flow;