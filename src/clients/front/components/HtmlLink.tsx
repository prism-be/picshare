import styles from "../styles/modules/link.module.scss";
import Link from "./Link";

interface Props
{
    children: JSX.Element;
    href: string;
    textSmall?:boolean;
}

const HtmlLink = ({children, href, textSmall}: Props) => {
    return <Link href={href}><a className={(textSmall ? styles.small : "")}>{children}</a></Link>
}

export default HtmlLink;