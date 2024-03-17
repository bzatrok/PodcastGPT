"use client"

import { useTheme } from "next-themes"

import { Button } from "@/components/Base/Button"
import { Icons } from "@/components/Icons"
import { useEffect, useState } from "react"

export function ThemeToggle() {
  const { setTheme, theme } = useTheme()
  const [isMounted, setIsMounted] = useState(false);

  useEffect(() => {
    setIsMounted(true);
  }, []);

  return (
    <Button
      variant="ghost"
      size="icon"
      onClick={() => setTheme(theme === "light" ? "dark" : "light")}
    >
      {isMounted && (
        <>
          {theme === "light" ? (
            <Icons.moon className="h-5 w-5" />
          ) : (
            <Icons.sun className="h-5 w-5" />
          )}
        </>
      )}
      <span className="sr-only">Toggle theme</span>
    </Button>
  )
}
