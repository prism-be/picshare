import {useEffect, useState} from "react";
import {getData, performRefreshToken} from "../../lib/ajaxHelper";
import {Thumbnail} from "./Thumbnail";
import useSWR from "swr";

const getFlow = async (route: string) => {

    await performRefreshToken();

    const response = await getData(route);

    if (response.status === 200) {
        return response.data;
    }

    return {};
}

const Flow = () => {

    const {data: flow, mutate: mutateFlow} = useSWR("/api/pictures/flow", getFlow);
    
    const [pictures, setPictures] = useState<any[]>([]);

    useEffect(() => {
        if (flow) {
            setPictures(flow.pictures);
        }
    }, [flow])


    return <>
        <div className="flex flex-wrap">
            {pictures && pictures.map(picture => <Thumbnail picture={picture} key={picture.id}/>)}
        </div>
    </>
}

export default Flow;