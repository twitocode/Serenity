import Image from 'next/image'
import { Button } from "../components/ui/button";
import Link from "next/link";

export default function Home() {
  return (
    <main className="w-full pt-20">
      <div className="grid grid-cols-2 px-40 pt-10">
        <div className="flex flex-col space-y-8 justify-center">
          <h1 className="scroll-m-20 font-extrabold tracking-tight lg:text-8xl">
            Become Comfortable
          </h1>
          <h3 className="lg:text-4xl">A cozy journal</h3>
          <div className="flex space-x-4">
            <Link href="/">
              <Button>Join Now</Button>
            </Link>
            <Link href="/">
              <Button variant="ghost">Or Login</Button>
            </Link>
          </div>
        </div>
        <div className="flex items-center">
          <Image src="/undraw_hiking.svg" alt="" width="500" height="500" />
        </div>
      </div>
      <div>
        <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 1440 320">
          <path
            fill="#292929"
            fill-opacity="1"
            d="M0,192L60,202.7C120,213,240,235,360,208C480,181,600,107,720,85.3C840,64,960,96,1080,117.3C1200,139,1320,149,1380,154.7L1440,160L1440,320L1380,320C1320,320,1200,320,1080,320C960,320,840,320,720,320C600,320,480,320,360,320C240,320,120,320,60,320L0,320Z"
          ></path>
        </svg>
        <div className="px-40 py-10 grid grid-cols-2 bg-[#292929] text-white">
          <div className="flex justify-between space-x-4">Brother</div>
          <div className="flex justify-between space-x-4">Brother</div>
        </div>
      </div>
    </main>
  );
}
