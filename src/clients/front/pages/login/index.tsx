// noinspection JSUnusedGlobalSymbols

import styles from "../../styles/pages/login.module.scss";

import type {NextPage} from 'next'
import {useTranslation} from "next-i18next";
import Button from "../../components/Button";
import HtmlLink from "../../components/HtmlLink";
import * as yup from "yup";
import {useForm} from "react-hook-form";
import {yupResolver} from "@hookform/resolvers/yup";
import {useState} from "react";
import InputText from "../../components/InputText";
import Alert from "../../components/Alert";
import {postData} from "../../lib/ajaxHelper";
import {useRouter} from "next/router";
import useUser from "../../lib/useUser";
import {serverSideTranslations} from "next-i18next/serverSideTranslations";

export const getStaticProps = async ({locale}: any) => ({
    props: {
        ...(await serverSideTranslations(locale, ['login', 'common']))
    }
})

const Login: NextPage = () => {

    const {mutateUser} = useUser();

    const [errorMessage, setErrorMessage] = useState('');

    const {t} = useTranslation(['login', 'common']);

    const router = useRouter();

    const schema = yup.object({
        login: yup.string().required(t("form.validation.required", {ns: 'common'})),
        password: yup.string().required(t("form.validation.required", {ns: 'common'})),
    }).required();

    const {register, handleSubmit, formState: {errors}} = useForm({resolver: yupResolver(schema)});

    const onSubmit = async (data: any) => {
        setErrorMessage("");
        const result = await postData('/api/authentication/login', data);

        if (result.status === 200) {
            localStorage.setItem('accessToken', result.data.accessToken);
            localStorage.setItem('refreshToken', result.data.refreshToken);

            await mutateUser();
            await router.push('/');

            return;
        }

        setErrorMessage(t("login.error.invalidCredentials"));
    }

    return (
        <div>

            <div className={styles.container}>
                <div className={styles.header}>
                    <div className="flex items-center justify-center">
                    </div>
                    <h2 className={styles.title}>
                        {t("login.title")}
                    </h2>
                    <span className={styles.subtitle}>
                        {t("login.or")}&nbsp;
                        <HtmlLink href="/login/register">{t("login.register")}</HtmlLink>
                    </span>
                </div>

                <div className={styles.panel}>
                    <div>
                        <form className={styles.form} onSubmit={handleSubmit(onSubmit)}>
                            <div>
                                <InputText name="login" label={t("login.form.login")} type="text"
                                           required={true} register={register} error={errors.login}/>
                                <InputText name="password" label={t("login.form.password")} type="password"
                                           required={true} register={register} error={errors.password}/>

                                <div className={styles.button}>
                                    <Button text={t("login.form.go")}/>
                                </div>

                                {errorMessage && <div className={styles.alert}>
                                    <Alert message={errorMessage} type={"alert"}/>
                                </div>}

                            </div>
                        </form>
                    </div>
                </div>
            </div>

        </div>
    )
}

export default Login
