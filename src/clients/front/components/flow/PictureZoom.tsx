﻿import React from "react";
import Image, {ImageLoaderProps} from "next/image";
import {IPictureSummary} from "../../lib/ajaxHelper";
import useSWR from "swr";

interface Props {
    picture: IPictureSummary;
    togglePictureZoom: (picture: IPictureSummary) => void;
    previousPictureZoom: () => void;
    nextPictureZoom: () => void;
}

export const PictureZoom = ({picture, togglePictureZoom, previousPictureZoom, nextPictureZoom}: Props) => {
    const myLoader = ({src, width}: ImageLoaderProps) => {
        if (width <= 960) {
            return src + "/960/540/?accessToken=" + localStorage.getItem('accessToken');
        }

        if (width <= 1920) {
            return src + "/1920/1080/?accessToken=" + localStorage.getItem('accessToken');
        }

        return src + "/3840/2160/?accessToken=" + localStorage.getItem('accessToken');
    }

    const {data: pictureInfo} = useSWR('/api/pictures/show/' + picture.organisationId + '/' + picture.id);

    let touchStartX = 0;
    const onTouchStart = (event: React.Touch) => {
        touchStartX = event.pageX;
    }

    const onTouchEnd = (event: React.Touch) => {
        const swipeLength = event.pageX - touchStartX;

        if (swipeLength < 0) {
            previousPictureZoom();
        }

        if (swipeLength > 0) {
            nextPictureZoom();
        }
    }

    return <>
        <div className="fixed top-0 left-0 right-0 bottom-0 z-50 overflow-auto bg-gray-600 flex"
             onTouchStart={(e) => onTouchStart(e.changedTouches[0])} onTouchEnd={(e) => onTouchEnd(e.changedTouches[0])}>
            <div onClick={() => nextPictureZoom()} className="grow relative opacity-100 m-1 cursor-pointer">
                <Image loader={myLoader} layout={"fill"} objectFit={"contain"} src={"/api/pictures/thumbs/" + picture.organisationId + "/" + picture.id} alt={picture.name}/>
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