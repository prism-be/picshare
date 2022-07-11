import {useTranslation} from "next-i18next";
import {DragEvent, useState} from "react";
import {postFile} from "../lib/ajaxHelper";

const DropZone = () => {
    
    let currentFileIndex = -1;

    const [percentageDone, setPercentageDone] = useState(0);
    const [inDropZone, setInDropZone] = useState(false);
    const [files, setFiles] = useState<File[]>([]);
    const [inProgress, setInProgress] = useState(false);

    const {t} = useTranslation('components')

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
            processNextFile();
        }
    };

    const processNextFile = async () => {
        currentFileIndex++;
        
        if (currentFileIndex < files.length)
        {
           const file = files[currentFileIndex];
           
           if (file.type.toLowerCase() === "image/jpeg")
           {
               await postFile("/api/pictures/upload", file);
           }
        }

        const percentage = Math.round((currentFileIndex + 1) / files.length * 100);
        setPercentageDone(percentage);
        
        if (currentFileIndex == files.length)
        {
            setInProgress(false);
            setFiles([]);
        }
        else if (currentFileIndex < files.length)
        {
            processNextFile();
        }
    };

    return <>
        <div className={"flex border border-dashed border-gray-500 relative rounded" + (inDropZone ? " bg-gray-200" : "")}
             onDragEnter={(e) => handleDragEnter(e)}
             onDragLeave={(e) => handleDragLeave(e)}
             onDragOver={(e) => handleDragOver(e)}
             onDrop={(e) => handleDrop(e)}
        >
            <input type="file" multiple className="cursor-pointer relative block opacity-0 w-full h-full p-20 z-50"/>
            <div className="flex flex-col text-center absolute top-0 right-0 left-0 bottom-0 m-auto">
                <div className={"flex grow"}>
                    <div className={"m-auto"}>
                        <h4>
                            {t("drop-anywhere")}
                            <br/>{t("or")}
                        </h4>
                        <p className="">{t("select")}</p>
                    </div>
                </div>
                <div className={"flex h-2"}>
                    {inProgress && <>
                        <div className={"h-full flex-auto bg-teal-600"} style={{width: percentageDone + "%"}}></div>
                        <div className={"h-full flex-auto bg-gray-200"} style={{width: (100 - percentageDone) + "%"}}></div>
                    </>}
                </div>
            </div>
        </div>
    </>
}

export default DropZone;