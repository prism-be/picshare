import styles from "../styles/modules/loader.module.scss";

interface Props {
    title?: string;
}

const Loader = ({title}: Props) => {
    return <>
        <div className={styles.title}>{title}</div>

        <div className={styles.rollerContainer}>
            <div className={styles.roller}>
                <div></div>
                <div></div>
                <div></div>
                <div></div>
                <div></div>
                <div></div>
                <div></div>
                <div></div>
            </div>
        </div>
    </>
}

export default Loader;