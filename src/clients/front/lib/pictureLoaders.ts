import {ImageLoaderProps} from "next/image";

const apiRoot = process.env.NEXT_PUBLIC_API_ROOT;

export const thumbsLoader = ({src, width}: ImageLoaderProps) => {
    if (width == 150) {
        return apiRoot + src + "/150/150/";
    }
    
    if (width <= 960) {
        return apiRoot + src + "/960/540/";
    }

    if (width <= 1920) {
        return apiRoot + src + "/1920/1080/";
    }

    return apiRoot + src + "/3840/2160/";
}