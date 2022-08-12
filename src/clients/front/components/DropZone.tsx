import styles from '../styles/modules/dropzone.module.scss'
import {useTranslation} from "next-i18next";
import {DragEvent, useState} from "react";
import {postFile} from "../lib/ajaxHelper";
import {useSWRConfig} from "swr";

interface Props {
    small?: boolean;
}

const DropZone = ({small}: Props) => {

    let currentFileIndex = -1;
    let files: File[]= [];

    const {mutate} = useSWRConfig();

    const [percentageDone, setPercentageDone] = useState(0);
    const [inDropZone, setInDropZone] = useState(false);
    const [inProgress, setInProgress] = useState(false);
    
    const {t} = useTranslation('common')

    const handleDragEnter = (e: DragEvent<HTMLDivElement>) => {
        e.preventDefault();
        e.stopPropagation();
        setInDropZone(true);
    };

    // onDragLeave sets inDropZone to false
    const handleDragLeave = (e: DragEvent<HTMLDivElement>) => {
        e.preventDefault();
        e.stopPropagation();

        setInDropZone(false);
    };

    const handleDragOver = (e: DragEvent<HTMLDivElement>) => {
        e.preventDefault();
        e.stopPropagation();

        // set dropEffect to copy i.e copy of the source item
        e.dataTransfer.dropEffect = "copy";
        setInDropZone(true);
    };

    const handleDrop = (e: DragEvent<HTMLDivElement>) => {
        e.preventDefault();
        e.stopPropagation();

        // get files from event on the dataTransfer object as an array
        for (let i = 0; i < e.dataTransfer.files.length; i++) {
            const file: File = e.dataTransfer.files[i];
            files.push(file);
        }
        
        setInDropZone(false);
        setPercentageDone(0);

        if (!inProgress) {
            setInProgress(true);

            // noinspection JSIgnoredPromiseFromCall
            processNextFile();
        }
    };

    const processNextFile = async () => {
        currentFileIndex++;

        if (currentFileIndex < files.length) {
            const file = files[currentFileIndex];

            if (file.type.toLowerCase() === "image/jpeg") {
                await postFile("/api/pictures/upload", file);
                setTimeout(() => mutate('/api/pictures/flow'), 30000);
            }
        }

        const percentage = Math.round((currentFileIndex + 1) / files.length * 100);
        setPercentageDone(percentage);

        if (currentFileIndex == files.length) {
            setInProgress(false);
            files = [];
            currentFileIndex = -1;
            setPercentageDone(0);
        } else if (currentFileIndex < files.length) {
            // noinspection ES6MissingAwait
            processNextFile();
        }
    };
    
    const handleFileSelected = (e: any) =>
    {
        e.preventDefault();
        e.stopPropagation();
        
        for (let i = 0; i < e.target.files.length; i++) {
            const file: File = e.target.files[i];
            files.push(file);
        }
        
        if (!inProgress) {
            setInProgress(true);

            // noinspection JSIgnoredPromiseFromCall
            processNextFile();
        }
    }

    return <>
        <div className={styles.dropzone + (inDropZone ? " " + styles.dropZoneActive : "")}
             onDragEnter={(e) => handleDragEnter(e)}
             onDragLeave={(e) => handleDragLeave(e)}
             onDragOver={(e) => handleDragOver(e)}
             onDrop={(e) => handleDrop(e)}
        >
            <input type="file" multiple onChange={(e) => handleFileSelected(e)}/>
            <div className={styles.text}>
                <div>
                    <div>
                        <h4>
                            {t("components.dropzone.drop-anywhere")}
                            { !small && <>
                            <br/>{t("components.dropzone.or")}
                                <p className="">{t("components.dropzone.select")}</p>
                            </>}
                        </h4>
                    </div>
                </div>
                <div className={styles.progress}>
                    {inProgress && <>
                        <div className={styles.progressDone} style={{width: percentageDone + "%"}}></div>
                        <div className={styles.progressTodo} style={{width: (100 - percentageDone) + "%"}}></div>
                    </>}
                </div>
            </div>
        </div>
    </>
}

export default DropZone;