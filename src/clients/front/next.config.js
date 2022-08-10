/** @type {import('next').NextConfig} */

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
    i18n: {
        defaultLocale: 'en',
        locales: ['en']
    }
}
