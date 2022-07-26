import { useEffect } from 'react'
import { useRouter } from 'next/router'
import languageDetector from '../lib/languageDetector'

export const useRedirect = (to: string | null = null) => {
    const router = useRouter()
    const toWithDefault = to || router.asPath

    // language detection
    useEffect(() => {
        const detectedLng = languageDetector.detect()
        if (toWithDefault.startsWith('/' + detectedLng) && router.route === '/404') { // prevent endless loop
            router.replace('/' + detectedLng + router.route)
            return
        }
        
        if (!detectedLng)
        {
            return;
        }

        if (languageDetector.cache) {
            languageDetector.cache(detectedLng)
        }
        
        router.replace('/' + detectedLng + toWithDefault)
    })

    return <></>
};

export const Redirect = () => {
    useRedirect()
    return <></>
}

// eslint-disable-next-line react/display-name
export const getRedirect = (to: string) => () => {
    useRedirect(to)
    return <></>
}