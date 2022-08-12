import styles from "../../../../styles/pages/login.module.scss";
import {useRouter} from 'next/router'
import {useTranslation} from "next-i18next";
import Loader from "../../../../components/Loader";
import {postData} from "../../../../lib/ajaxHelper";
import Alert from "../../../../components/Alert";
import Button from "../../../../components/Button";
import HtmlLink from "../../../../components/HtmlLink";
import {useState} from "react";
import {serverSideTranslations} from "next-i18next/serverSideTranslations";
import {GetStaticPaths} from "next";

export const getStaticProps = async ({locale}: any) => ({
    props: {
        ...(await serverSideTranslations(locale, ['login', 'common']))
    }
})

export const getStaticPaths: GetStaticPaths<{ slug: string }> = async () => {

    return {
        paths: [], //indicates that no page needs be created at build time
        fallback: 'blocking' //indicates the type of fallback
    }
}

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
        const response = await postData("/api/mailing/validate/" + slug, {});
        setInProgress(false);


        if (response.status !== 200) {
            setErrorMessage(t("validation.error") + " " + response.status);
            return;
        }

        setConfirmationMessage(t("validation.success"));

        setTimeout(() => router.push("/login"), 3000);
    }

    return <>

        <div className={styles.container}>
            <div className={styles.header}>
                <h2 className={styles.title}>
                    {t("validation.title")}
                </h2>
                <span className={styles.subtitle}>
                    {t("register.or")}&nbsp;
                    <HtmlLink href="/login">{t("register.login")}</HtmlLink>
                </span>
            </div>
            <div className={styles.panel}>
                <div>
                    <div className={styles.form}>

                        {!started && <div className="pb-3">{t("validation.help")}</div>}
                        {!started && <Button text={t("validation.button")} onClick={validate}></Button>}

                        {inProgress && <Loader title={t("validation.loading")}/>}

                        {errorMessage && <div className={styles.alert}>
                            <Alert message={errorMessage} type={"alert"}/>
                        </div>}

                        {confirmationMessage && <div className={styles.alert}>
                            <Alert message={confirmationMessage} type={"info"}/>
                        </div>}
                    </div>
                </div>
            </div>
        </div>
    </>
}

export default Validate;