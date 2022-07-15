import {NextRouter} from "next/router"
import {enUS, fr} from "date-fns/locale";

export const getCurrentLocale = (router: NextRouter) => {
    return enUS;
}