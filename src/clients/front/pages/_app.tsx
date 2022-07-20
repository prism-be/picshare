import '../styles/globals.css'
import {appWithTranslation, useTranslation} from 'next-i18next'
import type {AppProps} from 'next/app'
import Head from "next/head";
import {serverSideTranslations} from "next-i18next/serverSideTranslations";
import {SWRConfig} from "swr";
import {withAITracking} from "@microsoft/applicationinsights-react-js";
import {loadAppInsights, reactPlugin} from "../lib/AppInsights";
import {getData} from "../lib/ajaxHelper";
import {useEffect} from "react";

export const getStaticProps = async ({locale}: any) => ({
    props: {
        ...(await serverSideTranslations(locale, ['common']))
    }
})

const MyApp = ({Component, pageProps}: AppProps) => {

    const {t} = useTranslation('common')
    
    useEffect(() => {
        loadAppInsights();
    }, []);

    return <>
        <Head>
            <title>{t('header.title')}</title>
            <meta name="viewport" content="width=device-width, initial-scale=1.0"/>
            <link rel="icon" type="image/png" href="/favicon.png"/>

        </Head>
        <SWRConfig>
            <Component {...pageProps} />
        </SWRConfig>
    </>
}
export default withAITracking(reactPlugin, appWithTranslation(MyApp));