import {FieldValues, UseFormRegister} from "react-hook-form";

interface Props {
    label: string;
    name: string;
    type: string;
    required: boolean;
    register: UseFormRegister<FieldValues>;
    error: any;
}


const InputText = ({label, name, type, required, register, error}: Props) => {

    return <div className="w-full md:w-full px-3 mb-6">
        <label
            className="block uppercase tracking-wide text-gray-700 text-xs font-bold mb-2">{label} {required && " *"} </label>
        <input
            className={"appearance-none block w-full bg-white font-medium border rounded-lg py-3 px-3 leading-tight focus:outline-none " + (error ? "border-red-800" : "border-gray-400")}
            type={type}
            {...register(name, {required})}/>
        {error?.message && <p className={"text-sm text-red-800"}>{error.message}</p>}
    </div>
}

export default InputText;
