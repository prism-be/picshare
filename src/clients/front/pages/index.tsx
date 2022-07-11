import type {NextPage} from 'next'
import {serverSideTranslations} from "next-i18next/serverSideTranslations";
import useUser from "../lib/useUser";
import Loader from "../components/Loader";
import {useTranslation} from "next-i18next";
import Header from "../components/design/Header";

export const getStaticProps = async ({locale}: any) => ({
    props: {
        ...(await serverSideTranslations(locale, ['common']))
    }
})

const Home: NextPage = () => {

    const {user} = useUser({redirectTo: '/login'})

    const {t} = useTranslation('common')

    if (!user || user.authenticated === false) {
        return <Loader title={t("header.title")}/>
    }

    return (
        <div>
            <Header />
        </div>
    )
}

export default Home
