import Footer from '@/components/Generic/Footer';
import { SiteHeader } from '@/components/Generic/SiteHeader';
import { TailwindIndicator } from '@/components/Generic/TailwindIndicator';
import { ThemeProvider } from '@/components/Generic/ThemeProvider';
import '@/styles/globals.css'
import type { AppProps } from 'next/app'

export default function App({ Component, pageProps }: AppProps) {
  return (
    <ThemeProvider attribute="class" defaultTheme="system" enableSystem>
      <div className="relative flex min-h-screen flex-col">
        <SiteHeader />
        <div className="flex-1">
          <Component {...pageProps} />
        </div>
      </div>
      <TailwindIndicator />
      <Footer />
    </ThemeProvider>
  );
}
