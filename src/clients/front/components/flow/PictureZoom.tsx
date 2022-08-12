import styles from "../../styles/modules/flow.zoom.module.scss";
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
        <div className={styles.cover}>
            <div className={styles.picture}>
                <Image loader={thumbsLoader} layout={"fill"} objectFit={"contain"} src={"/api/pictures/thumbs/" + picture.token} alt={picture.name}/>
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
    </>
}