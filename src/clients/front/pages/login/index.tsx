// noinspection JSUnusedGlobalSymbols

import type {NextPage} from 'next'
import {useTranslation} from "next-i18next";
import {serverSideTranslations} from "next-i18next/serverSideTranslations";
import Button from "../../components/Button";
import HtmlLink from "../../components/HtmlLink";

export const getStaticProps = async ({locale}: any) => ({
    props: {
        ...(await serverSideTranslations(locale, ['login', 'common']))
    }
})

const Login: NextPage = () => {

    const {t} = useTranslation('login')

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
                    <form className="w-full max-w-xl bg-slate-50 rounded-lg shadow-md p-6">
                        <div className="flex flex-wrap -mx-3 mb-6">
                            <div className="w-full md:w-full px-3 mb-6">
                                <label className="block uppercase tracking-wide text-gray-700 text-xs font-bold mb-2"
                                       htmlFor='Password'>{t("login.form.email")}</label>
                                <input
                                    className="appearance-none block w-full bg-white text-gray-900 font-medium border border-gray-400 rounded-lg py-3 px-3 leading-tight focus:outline-none"
                                    type='email' required/>
                            </div>
                            <div className="w-full md:w-full px-3 mb-6">
                                <label className="block uppercase tracking-wide text-gray-700 text-xs font-bold mb-2"
                                       htmlFor='Password'>{t("login.form.password")}</label>
                                <input
                                    className="appearance-none block w-full bg-white text-gray-900 font-medium border border-gray-400 rounded-lg py-3 px-3 leading-tight focus:outline-none"
                                    type='password' required/>
                            </div>
                            <div className="w-full flex items-center justify-between px-3 mb-3 ">
                                <label htmlFor="remember" className="flex items-center w-1/2">
                                    <input type="checkbox" name="" id="" className="mr-1 bg-white shadow"/>
                                    <span className="text-sm text-gray-700 pt-1">{t("login.form.remember")}</span>
                                </label>
                                <div className="w-1/2 text-right">
                                    <HtmlLink href="#" textSmall={true}>{t("login.form.forget")}</HtmlLink>
                                </div>
                            </div>
                            <div className="w-full md:w-full px-3">
                                <Button text={t("login.form.go")} />
                            </div>
                        </div>
                    </form>
                </div>
            </div>

        </div>
    )
}

export default Login
