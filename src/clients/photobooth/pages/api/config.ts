import type { NextApiRequest, NextApiResponse } from 'next'
import Config from "../contracts/config";

export default function handler(req: NextApiRequest, res: NextApiResponse<Config>) {
    res.status(200).json(
        {
            publicBackendUrl: process.env.PUBLIC_BACKEND_URL,
            publicFrontUrl: process.env.PUBLIC_FRONT_URL,
        })
}