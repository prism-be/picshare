import Image, {ImageLoaderProps} from "next/image";
import {IPictureSummary} from "../../lib/ajaxHelper";
import {useState} from "react";

const config = require('../../lib/config.json');

interface Props {
    picture: IPictureSummary;
    togglePictureSelection: (id: string) => void;
    togglePictureZoom: (picture: IPictureSummary) => void;
}

export const Thumbnail = ({picture, togglePictureZoom}: Props) => {

    const [selected, setSelected]= useState(false);
    
    const myLoader = ({src}: ImageLoaderProps) => {
        return src + "?accessToken=" + localStorage.getItem('accessToken');
    }

    return <div key={picture.id} onClick={() => togglePictureZoom(picture)} className={"h-full w-full p-0 cursor-pointer border-2" + (selected ? " border-gray-500" : " border-white")}>
        <Image loader={myLoader} layout={"responsive"} src={config.api + "/api/pictures/thumbs/" + picture.organisationId + "/" + picture.id + "/150/150/"} width={150} height={150} alt={picture.name}/>
    </div>
}