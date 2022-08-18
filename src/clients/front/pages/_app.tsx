import '../styles/global.scss'

import {appWithTranslation, useTranslation} from 'next-i18next'
import type {AppProps} from 'next/app'
import Head from "next/head";
import {serverSideTranslations} from "next-i18next/serverSideTranslations";
import {SWRConfig} from "swr";
import {getData} from "../lib/ajaxHelper";

export const getStaticProps = async ({locale}: any) => ({
    props: {
        ...(await serverSideTranslations(locale, ['common']))
    }
})

const MyApp = ({Component, pageProps}: AppProps) => {

    const {t} = useTranslation('common')

    return <>
        <Head>
            <title>{t('header.title')}</title>
        </Head>
        <SWRConfig value={{fetcher: getData}}>
            <Component {...pageProps} />
        </SWRConfig>
    </>
}
export default appWithTranslation(MyApp);