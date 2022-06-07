/** @type {import('next').NextConfig} */
const nextConfig = {
  reactStrictMode: true,
  async rewrites() {
    return [
      {
        source: '/hubs/:slug*',
        destination: 'http://localhost:5047/hubs/:slug*'
      },
    ]
  },
}

module.exports = nextConfig
