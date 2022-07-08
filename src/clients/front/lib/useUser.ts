import {useEffect} from "react";
import Router, {useRouter} from "next/router";
import useSWR from 'swr'
import {getData} from "./ajaxHelper";

export default function useUser({
                                    redirectTo = "",
                                    redirectIfFound = false,
                                } = {}) {
    const prefix = process.env.NEXT_PUBLIC_API_ROOT ? process.env.NEXT_PUBLIC_API_ROOT : "";
    const {data: user, mutate: mutateUser} = useSWR(prefix + "/api/authentication/user/check", getData);

    const router = useRouter();
    
    useEffect(() => {
        if (!redirectTo || !user) {
            return;
        }
        
        if ((redirectTo && !redirectIfFound && !user.data.authenticated) ||(redirectIfFound && user.data.authenticated)) {
            router.push(redirectTo);
        }
    }, [user, redirectIfFound, redirectTo]);

    return {user, mutateUser};
}