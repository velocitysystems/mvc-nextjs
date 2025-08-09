import Link from "next/link";

export default function Home() {
  return (
    <div className="container">
      <div className="card">
        <h1 className="title">MVC-NextJS Template</h1>
        <p className="subtitle">
          A modern hybrid web application combining ASP.NET MVC with Next.js.
          Explore the dynamic routing capabilities below.
        </p>
        
        <div className="nav-links">
          <Link href="/slug/12345" className="nav-link">
            🚀 View Slug: 12345
          </Link>
          <Link href="/slug/54321" className="nav-link">
            ⭐ View Slug: 54321
          </Link>
          <Link href="/slug/demo" className="nav-link">
            🔍 View Slug: demo
          </Link>
        </div>
        
        <p style={{ marginTop: '2rem', fontSize: '0.9rem', color: '#888' }}>
          Try navigating to different slug pages to see the dynamic routing in action!
        </p>
      </div>
    </div>
  );
}
