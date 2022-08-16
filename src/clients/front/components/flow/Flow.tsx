import styles from "../../styles/modules/flow.module.scss";
import {useEffect, useState} from "react";
import useSWR from "swr";
import {Thumbnail} from "./Thumbnail";
import {format, parseISO, parseJSON} from "date-fns";
import {getCurrentLocale} from "../../lib/locales";
import {PictureZoom} from "./PictureZoom";
import {appInsights} from "../../lib/AppInsights";
import {getData, IFlow, IPictureSummary} from "../../lib/ajaxHelper";
import {useRouter} from "next/router";

const getFlow = async (route: string): Promise<IFlow> => {

    const response = await getData(route);

    if (response.status === 200) {
        return response.data;
    }

    return {
        organisationId: '',
        pictures: []
    };
}

interface IGroupedFlow {
    date: Date;
    day: string;
    pictures: IPictureSummary[];
}

const Flow = () => {

    const router = useRouter();
    
    const {data: flow} = useSWR("/api/pictures/flow", getFlow);

    const [groupedFlows, setGroupedFlows] = useState<IGroupedFlow[]>([]);
    const [pictures, setPictures] = useState<IPictureSummary[]>([]);
    const [selectedPictures, setSelectedPictures] = useState<string[]>([]);
    const [zoomPicture, setZoomPicture] = useState<IPictureSummary | null>(null);
    const [zoomPicturePrevious, setZoomPicturePrevious] = useState<IPictureSummary | null>(null);
    const [zoomPictureNext, setZoomPictureNext] = useState<IPictureSummary | null>(null);
    const [zoom, setZoom] = useState(false);
    
    useEffect(() => {
        setZoom(!!router.query.zoom);
    }, [router.query.zoom])

    useEffect(() => {
        if (flow) {

            let data: IGroupedFlow[] = [];

            setPictures(flow.pictures);

            flow.pictures.forEach((picture) => {
                const pictureDate = parseJSON(picture.date);
                const day: string = format(pictureDate, 'yyyy-MM-dd');

                let existing = data.find(x => x.day === day);

                if (!existing) {
                    existing = {
                        date: parseISO(day),
                        day,
                        pictures: []
                    }
                    data.push(existing);
                }

                existing.pictures.push(picture);
            })

            setGroupedFlows(data);
        }
    }, [flow])

    const togglePictureSelection = (id: string) => {
        const index = selectedPictures.indexOf(id);

        if (index === -1) {
            selectedPictures.push(id);
        } else {
            selectedPictures.splice(index, 1);
        }

        setSelectedPictures(selectedPictures);
    }

    const togglePictureZoom = (picture: IPictureSummary) => {

        if (picture.id === zoomPicture?.id) {
            displayAndTrackPicture(null);
            return;
        }

        displayAndTrackPicture(picture);
    }
    
    const displayAndTrackPicture = (picture: IPictureSummary | null) => {
        
        if (picture == null)
        {
            // noinspection JSIgnoredPromiseFromCall
            router.push("/");
            setZoomPicture(null);
            return;
        }

        const pathNameTrimmed = router.pathname.replace(/\/+$/, '');
        // noinspection JSIgnoredPromiseFromCall
        router.push(pathNameTrimmed + "/?zoom=true", pathNameTrimmed + "/zoom");
        setZoomPicture(picture);

        let position = pictures.indexOf(picture);
        setPreviousAndNextPictures(position);
        
        appInsights.trackPageView({
            uri: 'flow/' + picture.organisationId + "/" + picture.id
        });
    }

    const previousPictureZoom = () => {
        if (zoomPicture == null) {
            return;
        }

        let position = pictures.indexOf(zoomPicture);
        position++;

        if (position >= pictures.length) {
            return;
        }

        displayAndTrackPicture(pictures[position]);
    }

    const nextPictureZoom = () => {
        if (zoomPicture === null) {
            return;
        }

        let position = pictures.indexOf(zoomPicture);
        position--;

        if (position < 0) {
            return;
        }

        displayAndTrackPicture(pictures[position]);
    }
    
    const setPreviousAndNextPictures = (position: number) => {
        if (position > 0)
        {
            setZoomPicturePrevious(pictures[position - 1]);
        }
        else
        {
            setZoomPicturePrevious(null);
        }
        
        if (position < pictures.length - 1)
        {
            setZoomPictureNext(pictures[position + 1]);
        }
        else
        {
            setZoomPictureNext(null);
        }
    }

    return <>
        <div className="">
            {groupedFlows && groupedFlows.map(groupedFlow => <div key={groupedFlow.day} className={styles.groupedFlow}>
                <h1>
                    {format(groupedFlow.date, 'EEEE d MMMM yyyy', {
                        locale: getCurrentLocale()
                    })}
                </h1>
                <div className={styles.grid}>
                    {groupedFlow.pictures && groupedFlow.pictures.map(picture =>
                        <Thumbnail picture={picture} key={picture.id}
                                   togglePictureSelection={togglePictureSelection}
                                   togglePictureZoom={togglePictureZoom}
                        />
                    )}
                </div>
            </div>)}

            {zoomPicture && zoom && <PictureZoom picture={zoomPicture} picturePrevious={zoomPicturePrevious} pictureNext={zoomPictureNext} togglePictureZoom={togglePictureZoom} nextPictureZoom={nextPictureZoom} previousPictureZoom={previousPictureZoom}/>}

        </div>
    </>
}

export default Flow;