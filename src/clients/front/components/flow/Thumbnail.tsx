import Image, {ImageLoaderProps} from "next/image";

interface Props {
    picture: any;
}

export const Thumbnail = ({picture}: Props) => {
    const myLoader = ({src}: ImageLoaderProps) => {
        return src + "?accessToken=" + localStorage.getItem('accessToken');
    }
    
    return <div key={picture.id} className=" h-[150px] w-[150px] mr-2 mb-2 p-0 border-gray-200 border">
        <Image loader={myLoader} src={"/api/pictures/thumbs/" + picture.organisationId + "/" + picture.id + "/150/150/"} width={150} height={150} alt={picture.name}/>
    </div>
}