/** @type {import('next').NextConfig} */

const {i18n} = require('./next-i18next.config');


module.exports = {
    output: 'standalone',
    reactStrictMode: true,
    images: {
        domains: ['localhost']
    },
    i18n,
    async rewrites() {
        return [
            {
                source: '/api/:path*',
                destination: 'http://localhost:3080/api/:path*'
            }
        ]
    }
}
