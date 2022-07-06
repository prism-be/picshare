interface Props {
    title?: string;
}

const Loader = ({title}: Props) => {
    return <>
        <div className="flex justify-center items-center pt-5 pb-10 text-prism italic">{title}</div>
        <div className="flex justify-center items-center">
            <div className="spinner-border animate-spin text-prism inline-block w-12 h-12 border-4 rounded-full" role="status">
                <span className="visually-hidden">{title}</span>
            </div>
        </div>
    </>
}

export default Loader;