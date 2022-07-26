// noinspection JSUnusedGlobalSymbols

import type {NextPage} from 'next'
import {useTranslation} from "next-i18next";
import Button from "../../../components/Button";
import HtmlLink from "../../../components/HtmlLink";
import * as yup from "yup";
import {useForm} from "react-hook-form";
import {yupResolver} from "@hookform/resolvers/yup";
import {useState} from "react";
import InputText from "../../../components/InputText";
import Alert from "../../../components/Alert";
import {postData} from "../../../lib/ajaxHelper";
import {useRouter} from "next/router";
import useUser from "../../../lib/useUser";
import { getStaticPaths, makeStaticProps } from '../../../lib/getStatic'

const getStaticProps = makeStaticProps(['login', 'common'])
export { getStaticPaths, getStaticProps }

const Login: NextPage = () => {
    
    const { mutateUser } = useUser();

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

            <div className="container max-w-full mx-auto py-24 px-6">
                <div className="text-center mt-24">
                    <div className="flex items-center justify-center">
                    </div>
                    <h2 className="text-4xl text-teal-600 tracking-tight">
                        {t("login.title")}
                    </h2>
                    <span className="text-sm">
                        {t("login.or")}&nbsp;
                        <HtmlLink href="/login/register">{t("login.register")}</HtmlLink>
                    </span>
                </div>

                <div className="flex justify-center my-2 mx-4 md:mx-0">
                    <div className="flex justify-center my-2 mx-4 md:mx-0">
                        <form className="w-full max-w-xl bg-slate-50 rounded-lg shadow-md p-6"
                              onSubmit={handleSubmit(onSubmit)}>
                            <div className="flex flex-wrap -mx-3">
                                <InputText name="login" label={t("login.form.login")} type="text"
                                           required={true} register={register} error={errors.login}/>
                                <InputText name="password" label={t("login.form.password")} type="password"
                                           required={true} register={register} error={errors.password}/>

                                <div className="w-full md:w-full px-3 pt-3">
                                    <Button text={t("login.form.go")}/>
                                </div>

                                {errorMessage && <div className="w-full md:w-full px-3 pt-10">
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
