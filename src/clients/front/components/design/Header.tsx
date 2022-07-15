import useUser from "../../lib/useUser";
import Link from "next/link";
import Image from "next/image";
import {useTranslation} from "next-i18next";
import {useRouter} from "next/router";
import {useSWRConfig} from "swr";
import React from "react";
import DropZone from "../DropZone";

const Header = () => {

    const {user} = useUser();
    const {t} = useTranslation('common');

    const router = useRouter();
    const {mutate} = useSWRConfig();

    const logout = async (e: React.MouseEvent<HTMLAnchorElement>) => {
        e.preventDefault();
        localStorage.removeItem('accessToken');
        localStorage.removeItem('refreshToken');
        await mutate('/api/authentication/user/check');
        await router.push('/login');
    }

    return <>
        <div className="h-16 flex flex-nowrap border-b border-gray-300">
            <div className="flex">
                <Link href="/">
                    <a className="m-auto pl-2 pr-2">
                        <Image src="/images/logo.svg" height={42} width={42} alt={"Picshare by PRISM"}></Image>
                    </a>
                </Link>
            </div>
            <div className="grow">
            </div>
            <div className="flex w-[150px] sm:w-[300px] lg:w-[400px]">
                <div className="flex p-2">
                <DropZone small={true} />
                </div>
            </div>
            <div className="flex">
                <div className="m-auto pl-2 pr-2 text-sm">
                    {t("header.hello")} {user.name} !<br/>
                    <a href="#" onClick={logout}>{t("header.logout")}</a>

                </div>
            </div>
        </div>
    </>
}

export default Header;