﻿import styles from "../styles/modules/alert.module.scss";

interface Props {
    title?: string;
    message?: string;
    small?: boolean;
    type: 'info' | 'alert'
}

const Alert = ({title, message, small, type}: Props) => {

    let color: string;

    switch (type) {
        case "info":
            color = "teal-600";
            break;
        case "alert":
            color = "red-800";
            break;
    }

    return <div className={styles.alert}>
        <div className={"flex my-2 mx-4 md:mx-0" + (small ? " lg:w-6/12" : " w-full")}>
            <div className={"bg-gray-100 border-t-4 border-" + color + " rounded-b text-" + color + " px-4 py-3 shadow-md w-full"} role="alert">
                <div className="flex">
                    <div className="py-1">
                        <svg className={"fill-current h-6 w-6 text-" + color + " mr-4"} xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20">
                            <path d="M2.93 17.07A10 10 0 1 1 17.07 2.93 10 10 0 0 1 2.93 17.07zm12.73-1.41A8 8 0 1 0 4.34 4.34a8 8 0 0 0 11.32 11.32zM9 11V9h2v6H9v-4zm0-6h2v2H9V5z"/>
                        </svg>
                    </div>
                    <div className="py-1.5">
                        {title && <p className={"font-bold text-" + color}>{title}</p>}
                        {message && <p className={"text-sm text-" + color}>{message}</p>}
                    </div>
                </div>
            </div>
        </div>
    </div>
}

export default Alert;