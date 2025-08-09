import Link from "next/link";

export default function Home() {
  return (
    <>
      <h1>Home</h1>
      <Link href="/slug/12345">Slug/12345</Link><br />
      <Link href="/slug/54321">Slug/54321</Link>
    </>
  );
}
