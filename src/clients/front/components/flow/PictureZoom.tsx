import React from "react";
import Image from "next/image";
import {IPictureSummary} from "../../lib/ajaxHelper";
import {useKeyPressEvent} from "react-use";
import useSWR from "swr";
import {thumbsLoader} from "../../lib/pictureLoaders";

interface Props {
    picture: IPictureSummary;
    togglePictureZoom: (picture: IPictureSummary) => void;
    previousPictureZoom: () => void;
    nextPictureZoom: () => void;
}

export const PictureZoom = ({picture, togglePictureZoom, previousPictureZoom, nextPictureZoom}: Props) => {

    useKeyPressEvent('ArrowRight', () => {
        nextPictureZoom();
    })

    useKeyPressEvent('ArrowLeft', () => {
        previousPictureZoom();
    })

    useKeyPressEvent('Escape', () => {
        togglePictureZoom(picture);
    })


    const {data: pictureInfo} = useSWR('/api/pictures/show/' + picture.organisationId + '/' + picture.id);

    return <>
        <div className="fixed top-0 left-0 right-0 bottom-0 z-50 overflow-auto bg-gray-600 flex"
             onKeyUp={(e) => console.log(e)}>
            <div className="grow relative opacity-100 m-1 cursor-pointer">
                <Image loader={thumbsLoader} layout={"fill"} objectFit={"contain"} src={"/api/pictures/thumbs/" + picture.token} alt={picture.name}/>
            </div>
            <div onClick={() => previousPictureZoom()} className={"w-8 cursor-pointer flex absolute left-0 top-0 bottom-0"}>
                <span className="material-icons m-auto text-gray-500">keyboard_arrow_left</span>
            </div>
            <div onClick={() => nextPictureZoom()} className={"w-8 cursor-pointer flex absolute right-0 top-0 bottom-0"}>
                <span className="material-icons m-auto text-gray-500">keyboard_arrow_right</span>
            </div>
            {pictureInfo &&
                <div className={"h-8 absolute right-0 left-0 bottom-0 text-center text-sm text-gray-200 opacity-60"}>
                    {pictureInfo.data.name}
                </div>
            }
            <div onClick={() => togglePictureZoom(picture)} className={"w-8 h-8 m-1 cursor-pointer flex absolute right-0 top-0"}>
                <span className="material-icons m-auto text-gray-500">close_fullscreen</span>
            </div>
        </div>
    </>
}