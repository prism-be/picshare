import Link from "next/link";

interface Props
{
    children: JSX.Element;
    href: string;
    textSmall?:boolean;
}

const HtmlLink = ({children, href, textSmall}: Props) => {
    return <Link href={href}><a className={"text-teal-600" + (textSmall ? " text-sm tracking-tight" : "")}>{children}</a></Link>
}

export default HtmlLink;