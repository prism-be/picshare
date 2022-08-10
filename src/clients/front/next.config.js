/** @type {import('next').NextConfig} */
const { i18n } = require('./next-i18next.config');
module.exports = {
    output: 'standalone',
    reactStrictMode: true,
    images: {
        domains: ['localhost'],
        loader: 'custom'
    },
    publicRuntimeConfig: {
        apiRoot: process.env.NEXT_PUBLIC_API_URL
    },
    i18n
}
