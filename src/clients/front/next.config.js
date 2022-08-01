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
                // destination: 'https://picshare-dev-app-api-qvcq4otddsiqo.azurewebsites.net/api/:path*'
                destination: 'http://localhost:7071/api/:path*'
            }
        ]
    }
}
