import { Podcast } from "./Podcast";
import { PodcastPersona } from "./PodcastPersona";

export interface PodcastSegment {
    podcastSegmentId: string;
    order: number;
    textContent: string;
    audioFileUrl: string;
    openAiRole: string;
    podcast: Podcast;
    podcastPersonaId: string;
    podcastPersona: PodcastPersona;
}