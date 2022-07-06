export async function getData(route: string): Promise<Response> {
    const prefix = process.env.NEXT_PUBLIC_API_ROOT ? process.env.NEXT_PUBLIC_API_ROOT : "";
    return fetch(prefix + route, {
        method: "GET",
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
        },

        //make sure to serialize your JSON body
    });
}