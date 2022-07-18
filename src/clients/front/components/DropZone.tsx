import {useTranslation} from "next-i18next";
import {DragEvent, useState} from "react";
import {postFile} from "../lib/ajaxHelper";
import {useSWRConfig} from "swr";

interface Props {
    small?: boolean;
}

const DropZone = ({small}: Props) => {

    let currentFileIndex = -1;

    const {mutate} = useSWRConfig();

    const [percentageDone, setPercentageDone] = useState(0);
    const [inDropZone, setInDropZone] = useState(false);
    const [files, setFiles] = useState<File[]>([]);
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

        setFiles(files);

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
                setTimeout(async () => await mutate('/api/pictures/flow'), 2000);
            }
        }

        const percentage = Math.round((currentFileIndex + 1) / files.length * 100);
        setPercentageDone(percentage);

        if (currentFileIndex == files.length) {
            setInProgress(false);
            setFiles([]);
        } else if (currentFileIndex < files.length) {
            // noinspection ES6MissingAwait
            processNextFile();
        }
    };

    return <>
        <div className={"flex border border-dashed border-gray-300 relative rounded" + (inDropZone ? " bg-gray-200" : "")}
             onDragEnter={(e) => handleDragEnter(e)}
             onDragLeave={(e) => handleDragLeave(e)}
             onDragOver={(e) => handleDragOver(e)}
             onDrop={(e) => handleDrop(e)}
        >
            <input type="file" multiple className="cursor-pointer relative block opacity-0 w-full h-full z-50"/>
            <div className="flex flex-col text-center text-gray-500 absolute top-0 right-0 left-0 bottom-0 m-auto">
                <div className={"flex grow"}>
                    <div className={"m-auto"}>
                        <h4>
                            {t("components.dropzone.drop-anywhere")}
                            { !small && <>
                            <br/>{t("components.dropzone.or")}
                                <p className="">{t("components.dropzone.select")}</p>
                            </>}
                        </h4>
                    </div>
                </div>
                <div className={"flex h-2 absolute right-0 left-0 bottom-0"}>
                    {inProgress && <>
                        <div className={"h-full flex-auto bg-teal-600"} style={{width: percentageDone + "%"}}></div>
                        <div className={"h-full flex-auto bg-gray-300"} style={{width: (100 - percentageDone) + "%"}}></div>
                    </>}
                </div>
            </div>
        </div>
    </>
}

export default DropZone;