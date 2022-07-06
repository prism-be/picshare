import type {NextPage} from 'next'
import {serverSideTranslations} from "next-i18next/serverSideTranslations";
import useUser from "../lib/useUser";
import Loader from "../components/Loader";
import {useTranslation} from "next-i18next";

export const getStaticProps = async ({locale}: any) => ({
    props: {
        ...(await serverSideTranslations(locale, ['common']))
    }
})

const Home: NextPage = () => {

    const {user} = useUser({redirectTo: '/login'})

    const {t} = useTranslation('common')

    if (!user || user.isLoggedIn === false) {
        return <Loader title={t("header.title")}/>
    }

    return (
        <div>

            <h1 className="text-3xl font-bold underline">
                Hello world!
            </h1>

        </div>
    )
}

export default Home
