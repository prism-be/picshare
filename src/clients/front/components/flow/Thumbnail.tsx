import Image from "next/image";
import {IPictureSummary} from "../../lib/ajaxHelper";
import {thumbsLoader} from "../../lib/pictureLoaders";
import {useState} from "react";

interface Props {
    picture: IPictureSummary;
    togglePictureSelection: (id: string) => void;
    togglePictureZoom: (picture: IPictureSummary) => void;
}

export const Thumbnail = ({picture, togglePictureZoom}: Props) => {

    const [selected, setSelected]= useState(false);
    
    return <div key={picture.id} onClick={() => togglePictureZoom(picture)} className={"h-full w-full p-0 cursor-pointer border-2" + (selected ? " border-gray-500" : " border-white")}>
        <Image loader={thumbsLoader} layout={"responsive"} src={"/api/pictures/thumbs/" + picture.token + '/150/150/'} unoptimized={true} width={150} height={150} alt={picture.name}/>
    </div>
}