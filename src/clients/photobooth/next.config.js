/** @type {import('next').NextConfig} */
const nextConfig = {
  reactStrictMode: true,
  async rewrites() {
    return [
      {
        source: '/hubs',
        destination: 'http://localhost:5047/hubs',
      },
    ]
  }
}

module.exports = nextConfig
