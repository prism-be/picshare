import {appWithTranslation, useTranslation} from 'next-i18next'
import '../styles/globals.css'
import type {AppProps} from 'next/app'
import Head from "next/head";
import {serverSideTranslations} from "next-i18next/serverSideTranslations";
import {SWRConfig} from "swr";
import fetchJson from "../lib/fetchJson";

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
            <meta name="viewport" content="width=device-width, initial-scale=1.0"/>
            <link rel="icon" type="image/png" href="/favicon.png"/>
           
        </Head>
        <SWRConfig
            value={{
                fetcher: fetchJson,
                onError: (err) => {
                    console.error(err);
                },
            }}
        >
            <Component {...pageProps} />
        </SWRConfig>
    </>
}

export default appWithTranslation(MyApp)
