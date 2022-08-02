import {ImageLoaderProps} from "next/image";

const config = require('./config.json');

export const thumbsLoader = ({src, width}: ImageLoaderProps) => {
    if (width == 150) {
        return config.api + src + "/150/150/";
    }
    
    if (width <= 960) {
        return config.api + src + "/960/540/";
    }

    if (width <= 1920) {
        return config.api + src + "/1920/1080/";
    }

    return config.api + src + "/3840/2160/";
}