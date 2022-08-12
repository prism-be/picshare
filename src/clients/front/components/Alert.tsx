import styles from "../styles/modules/alert.module.scss";

interface Props {
    title?: string;
    message?: string;
    small?: boolean;
    type: 'info' | 'alert'
}

const Alert = ({title, message, small, type}: Props) => {

    let alertColor: string;
    let textColor: string;

    switch (type) {
        case "info":
            alertColor = styles.alertInfo;
            textColor = styles.info;
            break;
        case "alert":
            alertColor = styles.alertError;
            textColor = styles.error;
            break;
    }

    return <div className={styles.alert + " " + alertColor}>
        <div className={(small ? styles.small : "")}>
            <div role="alert">
                <div>
                    <div className={styles.icon}>
                        <svg className={textColor} xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20">
                            <path d="M2.93 17.07A10 10 0 1 1 17.07 2.93 10 10 0 0 1 2.93 17.07zm12.73-1.41A8 8 0 1 0 4.34 4.34a8 8 0 0 0 11.32 11.32zM9 11V9h2v6H9v-4zm0-6h2v2H9V5z"/>
                        </svg>
                    </div>
                    <div className={styles.text}>
                        {title && <p className={styles.title + " " + textColor}>{title}</p>}
                        {message && <p className={styles.message + " " + textColor}>{message}</p>}
                    </div>
                </div>
            </div>
        </div>
    </div>
}

export default Alert;