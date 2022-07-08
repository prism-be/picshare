export async function getData(route: string): Promise<any> {
    const prefix = process.env.NEXT_PUBLIC_API_ROOT ? process.env.NEXT_PUBLIC_API_ROOT : "";

    const response = await fetch(prefix + route, {
        method: "GET",
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json',
            'Authorization': getAuthorization()
        },

        //make sure to serialize your JSON body
    });

    return response.json();
}

const getAuthorization = (): string => {
    const accessToken = localStorage.getItem('accessToken');

    if (accessToken) {
        return 'Bearer ' + accessToken;
    }

    return '';
}