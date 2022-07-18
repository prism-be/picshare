import React from "react";
import Image, {ImageLoaderProps} from "next/image";

interface Props {
    organisationId: string;
    pictureId: string;
    togglePictureZoom: (id: string) => void;
    previousPictureZoom: () => void;
    nextPictureZoom: () => void;
}

export const PictureZoom = ({organisationId, pictureId, togglePictureZoom, previousPictureZoom, nextPictureZoom}: Props) => {
    const myLoader = ({src, width}: ImageLoaderProps) => {
        if (width <= 960) {
            return src + "/960/540/?accessToken=" + localStorage.getItem('accessToken');
        }

        if (width <= 1920) {
            return src + "/1920/1080/?accessToken=" + localStorage.getItem('accessToken');
        }

        return src + "/3840/2160/?accessToken=" + localStorage.getItem('accessToken');
    }

    return <>
        <div onClick={() => togglePictureZoom(pictureId)} className="fixed pin top-0 left-0 bottom-0 right-0 z-50 overflow-auto bg-gray-100 opacity-60 flex">
        </div>
        <div className="fixed h-screen top-0 left-0 right-0 z-50 overflow-auto bg-gray-100 flex">
            <div onClick={() => previousPictureZoom()} className={"w-8 cursor-pointer flex"}>
                <svg viewBox="0 0 24 24">
                    <path fill="#31859c" d="M20,11V13H8L13.5,18.5L12.08,19.92L4.16,12L12.08,4.08L13.5,5.5L8,11H20Z" />
                </svg>
            </div>
            <div onClick={() => togglePictureZoom(pictureId)} className="grow relative opacity-100 mt-6 mb-6 cursor-pointer">
                <Image loader={myLoader} layout={"fill"} objectFit={"contain"} src={"/api/pictures/thumbs/" + organisationId + "/" + pictureId}/>
            </div>
            <div onClick={() => nextPictureZoom()} className={"w-8 cursor-pointer flex"}>
                <svg viewBox="0 0 24 24">
                    <path fill="#31859c" d="M4,11V13H16L10.5,18.5L11.92,19.92L19.84,12L11.92,4.08L10.5,5.5L16,11H4Z" />
                </svg>
            </div>
        </div>
    </>
}