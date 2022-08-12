import styles from "../styles/modules/loader.module.scss";

interface Props {
    title?: string;
}

const Loader = ({title}: Props) => {
    return <>
        <div className={styles.title}>{title}</div>
    </>
}

export default Loader;