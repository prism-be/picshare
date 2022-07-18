import React from "react";
import Image, {ImageLoaderProps} from "next/image";

interface Props {
    organisationId: string;
    pictureId: string;
    togglePictureZoom: (id: string) => void;
}

export const PictureZoom = ({organisationId, pictureId, togglePictureZoom}: Props) => {
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
        <div onClick={() => togglePictureZoom(pictureId)} className="fixed h-screen top-0 left-0 right-0 z-50 overflow-auto bg-gray-100 flex">
            <div className="grow relative opacity-100 m-6 cursor-pointer">
                <Image loader={myLoader} layout={"fill"} objectFit={"contain"} src={"/api/pictures/thumbs/" + organisationId + "/" + pictureId}/>
            </div>
        </div>
    </>
}