/** @type {import('next').NextConfig} */
const { i18n } = require('./next-i18next.config');
module.exports = {
    output: 'standalone',
    reactStrictMode: true,
    images: {
        domains: ['localhost'],
        loader: 'custom',
        imageSizes: [150, 300, 960, 1280, 1920, 2560, 3840]
    },
    publicRuntimeConfig: {
        apiRoot: process.env.NEXT_PUBLIC_API_URL
    },
    i18n,
    experimental: {
        images: {
            allowFutureImage: true
        }
    }
}
