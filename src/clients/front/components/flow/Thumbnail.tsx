import Image from "next/image";
import {IPictureSummary} from "../../lib/ajaxHelper";
import {useState} from "react";
import getConfig from "next/config";

interface Props {
    picture: IPictureSummary;
    togglePictureSelection: (id: string) => void;
    togglePictureZoom: (picture: IPictureSummary) => void;
}

const { publicRuntimeConfig: config } = getConfig()

export const Thumbnail = ({picture, togglePictureZoom}: Props) => {

    const [selected, setSelected]= useState(false);
    
    return <div key={picture.id} onClick={() => togglePictureZoom(picture)} className={"h-full w-full p-0 cursor-pointer border-2" + (selected ? " border-gray-500" : " border-white")}>
        <Image unoptimized={true} layout={"responsive"} src={config.apiRoot + "/api/pictures/thumbs/" + picture.token + '/150/150/'} width={150} height={150} alt={picture.name}/>
    </div>
}