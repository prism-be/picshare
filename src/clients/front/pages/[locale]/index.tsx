import type {NextPage} from 'next'
import useUser from "../../lib/useUser";
import Loader from "../../components/Loader";
import {useTranslation} from "next-i18next";
import Header from "../../components/design/Header";
import Flow from "../../components/flow/Flow";
import { getStaticPaths, makeStaticProps } from '../../lib/getStatic'

const getStaticProps = makeStaticProps(['login', 'common'])
export { getStaticPaths, getStaticProps }

const Home: NextPage = () => {

    const {user} = useUser({redirectTo: '/login'})

    const {t} = useTranslation('common')

    if (!user || !user.authenticated) {
        return <Loader title={t("header.title")}/>
    }

    return (
        <div>
            <Header/>
            <div className={"p-5"}>
                <Flow />
            </div>
        </div>
    )
}

export default Home
