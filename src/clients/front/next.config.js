/** @type {import('next').NextConfig} */

module.exports = {
    output: 'standalone',
    reactStrictMode: true,
    images: {
        domains: ['localhost'],
        loader: 'custom'
    },
    async rewrites() {
        return [
            {
                source: '/api/:path*',
                destination: 'http://localhost:7071/api/:path*'
            }
        ]
    }
}
