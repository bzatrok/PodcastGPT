"use client"

import * as React from "react"
import { useTheme } from "next-themes"

import { Button } from "@/components/Base/Button"

export function ThemeToggle() {
  const { setTheme, theme } = useTheme()

  return (
    <Button
      variant="ghost"
      size="icon"
      onClick={() => setTheme(theme === "light" ? "dark" : "light")}
    >
      <span className="sr-only">Toggle theme</span>
    </Button>
  )
}
