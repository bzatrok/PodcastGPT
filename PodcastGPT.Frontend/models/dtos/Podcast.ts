import { NewsSiteArticle } from "./NewsSiteArticle";
import { PodcastPersona } from "./PodcastPersona";
import { PodcastSegment } from "./PodcastSegment";

export interface Podcast {
    podcastId: string;
    slug: string;
    status: string;
    title: string;
    topic: string;
    date: Date;
    fullAudioFileUrl: string;
    podcastSegments: PodcastSegment[];
    podcastPersonas: PodcastPersona[];
    newsSiteArticles: NewsSiteArticle[];
}