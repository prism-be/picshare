// Next.js API route support: https://nextjs.org/docs/api-routes/introduction
import type {NextApiRequest, NextApiResponse} from 'next'

type Data = {
    name: string,
    isLoggedIn: boolean,
}

export default function handler(
    _req: NextApiRequest,
    res: NextApiResponse<Data>
) {
    res.status(200).json({name: 'John Doe', isLoggedIn: false})
}
