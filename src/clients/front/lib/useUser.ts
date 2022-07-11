import {useEffect} from "react";
import {useRouter} from "next/router";
import useSWR from 'swr'
import {getData} from "./ajaxHelper";

const getUser = async (route: string) => {
    const response = await getData(route);
    
    if (response.status === 200) {
        return response.data;
    }
    
    return {
        authenticated: false
    };
}

export default function useUser({
                                    redirectTo = "",
                                    redirectIfFound = false,
                                } = {}) {
    const prefix = process.env.NEXT_PUBLIC_API_ROOT ? process.env.NEXT_PUBLIC_API_ROOT : "";
    const {data: user, mutate: mutateUser} = useSWR(prefix + "/api/authentication/user/check", getUser);

    const router = useRouter();

    useEffect(() => {
        if (!redirectTo || !user) {
            return;
        }

        if ((redirectTo && !redirectIfFound && !user.authenticated) || (redirectIfFound && user.authenticated)) {
            router.push(redirectTo);
        }
    }, [user, redirectIfFound, redirectTo]);

    return {user, mutateUser};
}