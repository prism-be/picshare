import {ImageLoaderProps} from "next/image";
import getConfig from 'next/config'

const { publicRuntimeConfig: config } = getConfig()

export const thumbsLoader = ({src, width}: ImageLoaderProps) => {
    if (width == 150) {
        return config.apiRoot + src + "/150/150/";
    }
    
    if (width <= 960) {
        return config.apiRoot + src + "/960/540/";
    }

    if (width <= 1920) {
        return config.apiRoot + src + "/1920/1080/";
    }

    return config.apiRoot + src + "/3840/2160/";
}