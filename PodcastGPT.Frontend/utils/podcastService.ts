import { Podcast } from "@/models/dtos/Podcast";
import { PodcastCreationRequest } from "@/models/requests/PodcastCreationRequest";
import { PodcastDetailResponse } from "@/models/responses/PodcastDetailResponse";

export const createPodcast = async (creationRequest: PodcastCreationRequest, onCreationFinished: (e: Podcast) => void) => {

    const baseUrl = process.env.NEXT_PUBLIC_BASE_API_URL;
    const requestUrl = `${baseUrl}/api/podcasts/generate`;

    const response: any = await fetch(requestUrl,
        {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify({
                // podcastTitle: creationRequest.PodcastTitle,
                podcastTopic: creationRequest.PodcastTopic,
                podcastNewsArticleUrl: creationRequest.PodcastNewsArticleUrl,
                shouldGenerate: creationRequest.ShouldGenerate
            })
        });
    const jsonResponse : PodcastDetailResponse = await response.json();
    onCreationFinished(jsonResponse.podcast);
};

export const deletePodcastById = async (podcastId: string) => {

    const baseUrl = process.env.NEXT_PUBLIC_BASE_API_URL;
    const requestUrl = `${baseUrl}/api/podcasts/${podcastId}`;

    await fetch(requestUrl,
        {
            method: "DELETE",
            headers: {
                "Content-Type": "application/json",
            }
        });
    // const jsonResponse : any = await response();
    // return jsonResponse;
}
