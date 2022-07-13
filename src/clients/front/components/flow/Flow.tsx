import {useEffect, useState} from "react";
import {getData} from "../../lib/ajaxHelper";
import {Thumbnail} from "./Thumbnail";

const Flow = () => {

    const [pictures, setPictures] = useState<any[]>([]);

    useEffect(() => {
        getData('/api/pictures/flow')
            .then(response => {
                setPictures(response.data.pictures);
            })
    }, [])


    return <>
        <div className="flex flex-wrap">
            {pictures && pictures.map(picture => <Thumbnail picture={picture} key={picture.id}/>)}
        </div>
    </>
}

export default Flow;