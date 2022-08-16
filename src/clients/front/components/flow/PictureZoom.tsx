import styles from "../../styles/modules/flow.zoom.module.scss";
import React from "react";
import Image, {ImageLoaderProps} from "next/future/image";
import {IPictureSummary} from "../../lib/ajaxHelper";
import {useKeyPressEvent} from "react-use";
import useSWR from "swr";
import getConfig from "next/config";

interface Props {
    picture: IPictureSummary;
    picturePrevious: IPictureSummary | null;
    pictureNext: IPictureSummary | null;
    togglePictureZoom: (picture: IPictureSummary) => void;
    previousPictureZoom: () => void;
    nextPictureZoom: () => void;
}

export const PictureZoom = ({picture, picturePrevious, pictureNext, togglePictureZoom, previousPictureZoom, nextPictureZoom}: Props) => {

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

    const { publicRuntimeConfig: config } = getConfig()

    const thumbsLoader = ({src, width}: ImageLoaderProps) => {
        const baseSrc = config.apiRoot + "/api/pictures/thumbs/" + src;
        const widthSuffix = getWidthSuffix(width);
        
        return baseSrc + widthSuffix;
    }
    
    const getWidthSuffix = (width: number) => {
        if (width <= 960) {
            return "/960/540/";
        }

        if (width <= 1280) {
            return "/1280/720/";
        }

        if (width <= 1920) {
            return "/1920/1080/";
        }

        if (width <= 2560) {
            return "/2560/1440/";
        }

        return "/3840/2160/";
    }

    return <>
        <div className={styles.cover}>
            <div className={styles.picture}>
                {picturePrevious && <Image className={styles.preload} loader={thumbsLoader} sizes="100vw" fill={true} src={picturePrevious.token} alt={picture.name}/>}
                {pictureNext && <Image className={styles.preload} loader={thumbsLoader} sizes="100vw" fill={true} src={pictureNext.token} alt={picture.name}/>}
                <Image loader={thumbsLoader} sizes="100vw" fill={true} src={picture.token} alt={picture.name}/>
            </div>
            <div onClick={() => previousPictureZoom()} className={styles.navigation + " " + styles.previous}>
                <span className="material-icons">keyboard_arrow_left</span>
            </div>
            <div onClick={() => nextPictureZoom()} className={styles.navigation + " " + styles.next}>
                <span className="material-icons">keyboard_arrow_right</span>
            </div>
            {pictureInfo &&
                <div className={styles.summary}>
                    {pictureInfo.data.name}
                </div>
            }
            <div onClick={() => togglePictureZoom(picture)} className={styles.close}>
                <span className="material-icons">close_fullscreen</span>
            </div>
        </div>
        <div>
            
        </div>
    </>
}