// noinspection JSUnusedGlobalSymbols

import {useTranslation} from "next-i18next";
import {useForm} from "react-hook-form";
import * as yup from "yup";
import {yupResolver} from '@hookform/resolvers/yup';
import {useState} from "react";
import InputText from "../../../../components/InputText";
import Alert from "../../../../components/Alert";
import HtmlLink from "../../../../components/HtmlLink";
import Button from "../../../../components/Button";
import { getStaticPaths, makeStaticProps } from '../../../../lib/getStatic'
import getConfig from 'next/config'

const getStaticProps = makeStaticProps(['login', 'common'])
export { getStaticPaths, getStaticProps }

const { publicRuntimeConfig: config } = getConfig()

const Register = () => {

    const [success, setSuccess] = useState(false);
    const [errorMessage, setErrorMessage] = useState('');

    const {t} = useTranslation(['login', 'common']);

    const schema = yup.object({
        name: yup.string().required(t("form.validation.required", {ns: 'common'})),
        organisation: yup.string().required(t("form.validation.required", {ns: 'common'})),
        login: yup.string().required(t("form.validation.required", {ns: 'common'})),
        email: yup.string().required(t("form.validation.required", {ns: 'common'}))
            .email(t("form.validation.email", {ns: 'common'})),
        password: yup.string().required(t("form.validation.required", {ns: 'common'})),
        passwordCheck: yup.string().required(t("form.validation.required", {ns: 'common'}))
            .oneOf([yup.ref('password'), null], t("form.validation.passwordMatch", {ns: 'common'})),
    }).required();


    const {register, handleSubmit, formState: {errors}} = useForm({resolver: yupResolver(schema)});
    const onSubmit = async (data: any) => {

        setErrorMessage('');

        const response = await fetch(config.apiRoot + "/api/authentication/register", {
            method: "post",
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json'
            },

            body: JSON.stringify(data)
        });

        if (response.status === 204) {
            setSuccess(true);
            return;
        }

        const responseData = await response.json();

        switch (responseData.code) {
            case 40901:
                setErrorMessage(t("register.error.organisationAlreadyExists"));
                break;
            case 40902:
                setErrorMessage(t("register.error.userAlreadyExists"));
                break;
            default:
                setErrorMessage("Unknown error : " + response.status + " - " + responseData.code)
                break;
        }
    };


    return (
        <>
            <div>

                <div className="container max-w-full mx-auto px-6">
                    <div className="text-center mt-24">
                        <div className="flex items-center justify-center">
                        </div>
                        <h2 className="text-4xl text-teal-600 tracking-tight">
                            {t("register.title")}
                        </h2>
                        <span className="text-sm">{t("register.or")}&nbsp;
                            <HtmlLink href="/login">{t("register.login")}</HtmlLink>
                    </span>
                    </div>

                    <div className="flex justify-center my-2 mx-4 md:mx-0">
                        <form className="w-full max-w-xl bg-slate-50 rounded-lg shadow-md p-6"
                              onSubmit={handleSubmit(onSubmit)}>
                            <div className="flex flex-wrap -mx-3">
                                {!success && <>
                                    <InputText name="name" label={t("register.form.name")} type="text"
                                               required={true} register={register} error={errors.name}/>
                                    <InputText name="organisation" label={t("register.form.organisation")} type="text"
                                               required={true} register={register} error={errors.organisation}/>
                                    <InputText name="login" label={t("register.form.login")} type="text"
                                               required={true} register={register} error={errors.login}/>
                                    <InputText name="email" label={t("register.form.email")} type="email"
                                               required={true} register={register} error={errors.email}/>
                                    <InputText name="password" label={t("register.form.password")} type="password"
                                               required={true} register={register} error={errors.password}/>
                                    <InputText name="passwordCheck" label={t("register.form.passwordCheck")} type="password"
                                               required={true} register={register} error={errors.passwordCheck}/>

                                    <div className="w-full md:w-full px-3 pt-3">
                                        <Button text={t("register.form.go")}/>
                                    </div>
                                </>}

                                {errorMessage && <div className="w-full md:w-full px-3 pt-10">
                                    <Alert message={errorMessage} type={"alert"}/>
                                </div>}

                                {success && <div className="w-full md:w-full px-3">
                                    <Alert title={t("register.success.title")} message={t("register.success.message")} type={"info"}/>
                                </div>}

                            </div>
                        </form>
                    </div>
                </div>

            </div>
        </>
    )
}

export default Register
