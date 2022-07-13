import useUser from "../../lib/useUser";
import Link from "next/link";
import Image from "next/image";

const Header = () => {

    const {user} = useUser();

    return <>
        <div className="text-white bg-teal-600 h-12 flex flex-nowrap">
            <div className="flex">
                <Link href="/">
                    <a className="m-auto pl-2 pr-2">
                        <Image src="/images/logo-white.svg" height={42} width={42} alt={"Picshare by PRISM"}></Image>
                    </a>
                </Link>
            </div>
            <div className="grow bg-teal-600">
            </div>
            <div className="flex">
                <div className="m-auto pl-2 pr-2">
                    Hello {user.name} !
                </div>
            </div>
        </div>
    </>
}

export default Header;