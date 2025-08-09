'use client'

import { useState, useEffect } from "react";
import { useRouter } from 'next/router';
import Link from "next/link";

const Slug = () => {
    const router = useRouter();
    const [slugId, setSlugId] = useState<string>("");
    const { id } = router.query;

    useEffect(() => {
       setSlugId(id as string);
    }, [id]);

    const getSlugIcon = (slug: string) => {
        if (slug === '12345') return '🚀';
        if (slug === '54321') return '⭐';
        if (slug === 'demo') return '🔍';
        return '📄';
    };

    const getSlugDescription = (slug: string) => {
        if (slug === '12345') return 'This is a rocket-powered slug page demonstrating dynamic routing.';
        if (slug === '54321') return 'A stellar example of Next.js dynamic page generation.';
        if (slug === 'demo') return 'A demonstration page showcasing the MVC-NextJS template capabilities.';
        return `A dynamically generated page for slug: ${slug}`;
    };

    return (
        <div className="container">
            <div className="card">
                <div className="slug-id">
                    {getSlugIcon(slugId)} {slugId}
                </div>
                <p className="slug-description">
                    {getSlugDescription(slugId)}
                </p>
                
                <div style={{ marginBottom: '1.5rem' }}>
                    <p style={{ fontSize: '0.95rem', color: '#666', marginBottom: '1rem' }}>
                        <strong>Route:</strong> <code>/slug/{slugId}</code>
                    </p>
                    <p style={{ fontSize: '0.9rem', color: '#888' }}>
                        This page was dynamically generated using Next.js dynamic routing with the [id].tsx pattern.
                    </p>
                </div>
                
                <Link href="/" className="back-link">
                    ← Back to Home
                </Link>
            </div>
        </div>
    );
};

export default Slug;
