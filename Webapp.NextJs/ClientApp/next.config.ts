import type { NextConfig } from "next";

const isProduction = process.env.NODE_ENV === "production";

// In development, we need to proxy API requests to the ASP.NET MVC backend.
const rewrites = async () => [
  {
    source: '/client/:path*',
    destination: `https://localhost:7260/client/:path*`,
  }
];

const nextConfig: NextConfig = {
  /* config options here */
  reactStrictMode: true,
  ...(isProduction && { output: 'export' }),
  ...(!isProduction && { rewrites: rewrites }),
  distDir: 'build',
  images: {
    unoptimized: true, // Required for static export
  },
};

export default nextConfig;
