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
    });

    return (
        <>
            <h1>{slugId}</h1>
            <Link href="/">Back to Home</Link>
        </>
    );
};

export default Slug;
