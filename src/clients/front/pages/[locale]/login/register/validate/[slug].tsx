import {useRouter} from 'next/router'
import {useTranslation} from "next-i18next";
import Loader from "../../../../../components/Loader";
import {getData} from "../../../../../lib/ajaxHelper";
import Alert from "../../../../../components/Alert";
import Button from "../../../../../components/Button";
import HtmlLink from "../../../../../components/HtmlLink";
import {useState} from "react";
import { makeStaticProps } from '../../../../../lib/getStatic'
import {GetStaticPaths} from "next";

const getStaticProps = makeStaticProps(['login', 'common'])

export const getStaticPaths: GetStaticPaths<{ slug: string }> = async () => {

    return {
        paths: [], //indicates that no page needs be created at build time
        fallback: 'blocking' //indicates the type of fallback
    }
}

export { getStaticProps }

const Validate = () => {

    const [started, setStarted] = useState(false);
    const [inProgress, setInProgress] = useState(false);
    const [errorMessage, setErrorMessage] = useState('');
    const [confirmationMessage, setConfirmationMessage] = useState('');

    const router = useRouter();
    const {slug} = router.query;

    const {t} = useTranslation(['login', 'common']);

    const validate = async () => {
        setStarted(true);
        setInProgress(true);
        const response = await getData("/api/mailing/validate/" + slug);
        setInProgress(false);


        if (response.status !== 200) {
            setErrorMessage(t("validation.error") + " " + response.status);
            return;
        }

        setConfirmationMessage(t("validation.success"));

        setTimeout(() => router.push("/login"), 3000);
    }

    return <>

        <div className="container max-w-full mx-auto px-6">
            <div className="text-center mt-24">
                <div className="flex items-center justify-center">
                </div>
                <h2 className="text-4xl text-teal-600 tracking-tight">
                    {t("validation.title")}
                </h2>
                <span className="text-sm">{t("register.or")}&nbsp;
                    <HtmlLink href="/login">{t("register.login")}</HtmlLink>
                    </span>
            </div>
            <div className="flex justify-center my-2 mx-4 md:mx-0">
                <div className="w-full max-w-xl bg-slate-50 rounded-lg shadow-md p-6 text-center">

                    {!started && <div className="pb-3">{t("validation.help")}</div>}
                    {!started && <Button text={t("validation.button")} onClick={validate}></Button>}

                    {inProgress && <Loader title={t("validation.loading")}/>}

                    {errorMessage && <div className="w-full md:w-full px-3">
                        <Alert message={errorMessage} type={"alert"}/>
                    </div>}

                    {confirmationMessage && <div className="w-full md:w-full px-3">
                        <Alert message={confirmationMessage} type={"info"}/>
                    </div>}
                </div>


            </div>


        </div>


    </>
}

export default Validate;