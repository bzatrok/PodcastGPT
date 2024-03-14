export type SiteConfig = typeof siteConfig

export const siteConfig = {
  name: "PodcastGPT",
  description:
    "PodcastGPT is a podcast creation tool. Give it a topic and a link to a news article and it'll create a podcast between two hosts, who'll discuss it for you.",
  mainNav: [
    {
      title: "Home",
      href: "/",
    },
  ],
  links: {
    github: "https://github.com/bzatrok/PodcastGPT"
  },
}
