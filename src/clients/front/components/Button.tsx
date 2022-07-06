import {MouseEventHandler} from "react";

interface Props {
    text: string;
    onClick?: MouseEventHandler<HTMLButtonElement>
}

const Button = ({text, onClick}: Props) => {
    return <>
        <button className="appearance-none block w-full bg-teal-600 text-gray-100 font-bold border border-gray-200 rounded-lg py-3 px-3 leading-tight hover:bg-teal-800 focus:bg-gray-500"
        onClick={onClick}>
            {text}
        </button>
    </>
}

export default Button;