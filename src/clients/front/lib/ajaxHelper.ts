interface ObjectResult
{
    status: number;
    data: any | undefined;
}

export async function getData(route: string): Promise<ObjectResult> {
    const prefix = process.env.NEXT_PUBLIC_API_ROOT ? process.env.NEXT_PUBLIC_API_ROOT : "";

    const response = await fetch(prefix + route, {
        method: "GET",
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json',
            'Authorization': getAuthorization()
        },
    });

    if (response.status === 200)
    {
        return {
            status: response.status,
            data: await response.json()
        }
    }

    return {
        status: response.status,
        data: undefined
    }
}

export async function postData(route: string, body: any): Promise<any> {
    const prefix = process.env.NEXT_PUBLIC_API_ROOT ? process.env.NEXT_PUBLIC_API_ROOT : "";

    const response = await fetch(prefix + route, {
        body: JSON.stringify(body),
        method: "POST",
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json',
            'Authorization': getAuthorization()
        },
    });

    if (response.status === 200)
    {
        return {
            status: response.status,
            data: await response.json()
        }
    }

    return {
        status: response.status,
        data: undefined
    }
}

const getAuthorization = (): string => {
    const accessToken = localStorage.getItem('accessToken');

    if (accessToken) {
        return 'Bearer ' + accessToken;
    }

    return '';
}