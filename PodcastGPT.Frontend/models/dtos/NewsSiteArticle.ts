import { Podcast } from "./Podcast";

export interface NewsSiteArticle {
	newsSiteArticleId: string;
	publishDate: Date;
	title: string;
	summary: string;
	url: string;
	podcast: Podcast;
	podcastId: string;
}