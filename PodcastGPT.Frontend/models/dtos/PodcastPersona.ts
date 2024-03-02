import { Podcast } from "./Podcast";

export interface PodcastPersona {
    podcastPersonaId: string;
    name: string;
    voiceId: string;
    type: string;
    podcasts: Podcast[];
}