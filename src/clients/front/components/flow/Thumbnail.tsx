import Image, {ImageLoaderProps} from "next/image";

interface Props {
    picture: any;
}

export const Thumbnail = ({picture}: Props) => {
    const myLoader = ({src}: ImageLoaderProps) => {
        return src + "?accessToken=" + localStorage.getItem('accessToken');
    }
    
    return <div key={picture.id} className="h-full w-full p-0 border-white border-2">
        <Image loader={myLoader} layout={"responsive"} src={"/api/pictures/thumbs/" + picture.organisationId + "/" + picture.id + "/150/150/"} width={150} height={150} alt={picture.name}/>
    </div>
}