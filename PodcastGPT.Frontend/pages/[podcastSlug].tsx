import { useEffect, useState } from 'react'
import { GetServerSideProps } from 'next';

import Navbar from '@/components/Generic/Navbar'
import { Podcast } from '@/models/dtos/Podcast'
import PodcastSegmentsTable from '@/components/Podcast/PodcastSegmentsTable';

interface PodcastDetailPageProps {
  podcastSlug: string;
}

const PodcastDetailPage: React.FC<PodcastDetailPageProps> = ({ podcastSlug }) => {

  const [podcast, setPodcast] = useState<Podcast>({} as Podcast);

  useEffect(() => {
    const fetchPodcast = async () => {

      const baseUrl = process.env.NEXT_PUBLIC_BASE_API_URL;
      const requestUrl = `${baseUrl}/api/podcasts/${podcastSlug}`;

      const response: any = await fetch(requestUrl);
      const responseData = await response.json();

      setPodcast(responseData.podcast);
    };

    fetchPodcast();
  }, []);

  return (
    <>
      <Navbar />
      <PodcastSegmentsTable podcast={podcast} />
    </>
  )
}

export default PodcastDetailPage;

export const getServerSideProps: GetServerSideProps = (async (context) => {
  const podcastSlug = context.query['podcastSlug'] as string;

  return {
    props: {
      podcastSlug: podcastSlug,
    },
  };
});