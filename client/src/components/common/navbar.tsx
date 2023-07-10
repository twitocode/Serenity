import Link from "next/link";
import { Button } from "../ui/button";
import Logo from "./logo";

export default function Navbar() {
  return (
    <nav className="flex justify-between px-40 pt-10">
      <div>
        <Logo />
      </div>
      <div className="space-x-4">
        <Link href="/">
          <Button variant="ghost">Login</Button>
        </Link>
        <Link href="/">
          <Button>Register</Button>
        </Link>
      </div>
    </nav>
  );
}
