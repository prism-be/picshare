import styles from "../../styles/modules/flow.thumbnail.module.scss";

import Image, {ImageLoaderProps} from "next/future/image";
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

    const thumbsLoader = ({src, width}: ImageLoaderProps) => {
        const baseSrc = config.apiRoot + "/api/pictures/thumbs/" + src;
        const widthSuffix = getWidthSuffix(width);

        return baseSrc + widthSuffix;
    }

    const getWidthSuffix = (width: number) => {
        
        if (width <= 150) {
            return "/150/150/";
        }

        return "/300/300/";
    }

    const css = { height: 'auto', width: '150px' }
    
    return <div key={picture.id} onClick={() => togglePictureZoom(picture)} className={styles.thumbnail + (selected ? " " + styles.selected : "")}>
        <Image loader={thumbsLoader} src={picture.token} style={css} width={150} height={150} alt={picture.name}/>
    </div>
}