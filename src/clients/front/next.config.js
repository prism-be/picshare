/** @type {import('next').NextConfig} */

const {i18n} = require('./next-i18next.config');

const nextConfig = {
    reactStrictMode: true,
    experimental: {
        outputStandalone: true
    },
    images: {
        domains: ['localhost']
    },
    i18n
}

module.exports = nextConfig
