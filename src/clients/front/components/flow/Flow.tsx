import {useEffect, useState} from "react";
import {getData, IFlow, IPictureSummary, performRefreshToken} from "../../lib/ajaxHelper";
import {Thumbnail} from "./Thumbnail";
import useSWR from "swr";
import {format, parseISO, parseJSON} from "date-fns";
import {getCurrentLocale} from "../../lib/locales";
import {PictureZoom} from "./PictureZoom";

const getFlow = async (route: string): Promise<IFlow> => {

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

interface IGroupedFlow {
    date: Date;
    day: string;
    pictures: IPictureSummary[];
}

const Flow = () => {

    const {data: flow} = useSWR("/api/pictures/flow", getFlow);

    const [groupedFlows, setGroupedFlows] = useState<IGroupedFlow[]>([]);
    const [selectedPictures, setSelectedPictures] = useState<string[]>([]);
    const [zoomPicture, setZoomPicture] = useState('');
    const [organisationId, setOrganisationId] = useState('');

    useEffect(() => {
        if (flow) {

            let data: IGroupedFlow[] = [];

            setOrganisationId(flow.organisationId);

            flow.pictures.forEach((picture) => {
                const pictureDate = parseJSON(picture.date);
                const day: string = format(pictureDate, 'yyyy-MM-dd');

                let existing = data.find(x => x.day === day);

                if (!existing) {
                    existing = {
                        date: parseISO(day),
                        day,
                        pictures: []
                    }
                    data.push(existing);
                }

                existing.pictures.push(picture);
            })

            setGroupedFlows(data);
        }
    }, [flow])

    const togglePictureSelection = (id: string) => {
        const index = selectedPictures.indexOf(id);

        if (index === -1) {
            selectedPictures.push(id);
        } else {
            selectedPictures.splice(index, 1);
        }

        setSelectedPictures(selectedPictures);
    }

    const togglePictureZoom = (id: string) => {

        if (id === zoomPicture) {
            setZoomPicture('');
            return;
        }
        setZoomPicture(id);
    }

    return <>
        <div className="">
            {groupedFlows && groupedFlows.map(groupedFlow => <div key={groupedFlow.day} className="pb-5">
                <h1 className="text-gray-500">
                    {format(groupedFlow.date, 'EEEE d MMMM yyyy', {
                        locale: getCurrentLocale()
                    })}
                </h1>
                <div className="grid grid-cols-3 sm:grid-cols-4 md:grid-cols-6 lg:grid-cols-10 xl:grid-cols-12 gap-1">
                    {groupedFlow.pictures && groupedFlow.pictures.map(picture =>
                        <Thumbnail picture={picture} key={picture.id}
                                   togglePictureSelection={togglePictureSelection}
                                   togglePictureZoom={togglePictureZoom}
                        />
                    )}
                </div>
            </div>)}

            {zoomPicture && <PictureZoom organisationId={organisationId} pictureId={zoomPicture} togglePictureZoom={togglePictureZoom}/>}

        </div>
    </>
}

export default Flow;