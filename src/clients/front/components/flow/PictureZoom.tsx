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

    let touchStartX = 0;
    let touchStartY = 0;
    const onTouchStart = (touches: React.TouchList) => {
        if (touches.length > 1)
        {
            return;
        }
        
        touchStartX = touches[0].pageX;
        touchStartY = touches[0].pageY;
    }

    const onTouchEnd = (touches: React.TouchList) => {
        
        if (touches.length > 1)
        {
            return;
        }
        
        const swipeXLength = touches[0].pageX - touchStartX;
        const swipeYLength = touches[0].pageY - touchStartY;

        let swipeRight = false;
        let swipeLeft = false;
        
        if (Math.abs(swipeXLength) > Math.abs(swipeYLength)) {
            swipeRight = swipeXLength > 0;
            swipeLeft = !swipeRight && swipeXLength < 0
        }
        
        if (swipeLeft) {
            nextPictureZoom();
        } else if (swipeRight) {
            previousPictureZoom();
        }
    }

    return <>
        <div className="fixed top-0 left-0 right-0 bottom-0 z-50 overflow-auto bg-gray-600 flex"
             onTouchStart={(e) => onTouchStart(e.changedTouches)} onTouchEnd={(e) => onTouchEnd(e.changedTouches)}
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