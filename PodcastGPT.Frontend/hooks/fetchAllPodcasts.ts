

// hooks/useFetchCurrentPodcast.ts
import { useEffect, useState } from 'react';
import { Podcast } from '@/models/dtos/Podcast';

const useFetchAllPodcasts = () => {
    const [podcasts, setPodcasts] = useState<Podcast[]>([]);

    useEffect(() => {
        fetchPodcasts();
    }, []);

    const fetchPodcasts = async () => {

        const baseUrl = process.env.NEXT_PUBLIC_BASE_API_URL;
        const requestUrl = `${baseUrl}/api/podcasts`;

        const response: any = await fetch(requestUrl);
        const responseData = await response.json();

        setPodcasts(responseData.podcasts);
    };

    return podcasts;
};

export default useFetchAllPodcasts;
