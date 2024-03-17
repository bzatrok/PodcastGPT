import { useEffect, useState } from 'react'
import { GetServerSideProps } from 'next';

import { Podcast } from '@/models/dtos/Podcast'
import PodcastSegmentsTable from '@/components/Podcast/PodcastSegmentsTable';
import { Button } from '@/components/Base/Button'
import LabeledSpinner from '@/components/Generic/LabeledSpinner';

interface PodcastDetailPageProps {
  podcastSlug: string;
}

const PodcastDetailPage: React.FC<PodcastDetailPageProps> = ({ podcastSlug }) => {

  const [isLoading, setIsLoading] = useState<boolean>(true);
  const [podcast, setPodcast] = useState<Podcast>({} as Podcast);
  const [isEditingMetadata, setIsEditingMetadata] = useState<boolean>(false);

  useEffect(() => {
    const fetchPodcast = async () => {

      setIsLoading(true);

      const baseUrl = process.env.NEXT_PUBLIC_BASE_API_URL;
      const requestUrl = `${baseUrl}/api/podcasts/slug/${podcastSlug}`;

      const response: any = await fetch(requestUrl);
      const responseData = await response.json();

      setPodcast(responseData.podcast);

      setIsLoading(false);
    };

    fetchPodcast();
  }, []);

  const savePodcastMetadata = async () => {
    const baseUrl = process.env.NEXT_PUBLIC_BASE_API_URL;
    const requestUrl = `${baseUrl}/api/podcasts/${podcast.podcastId}`;

    const response: any = await fetch(requestUrl, {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(podcast),
    });

    const responseData = await response.json();
  }

  return (
    <section className="container grid items-center gap-6 pb-8 pt-6 md:py-10">
      {podcast && !isLoading ? (
        <>
          <div className="flex max-w-[980px] flex-col items-start gap-2">

            <h1 className="text-3xl font-extrabold leading-tight tracking-tighter md:text-4xl">
              <>
                {isEditingMetadata ? (
                  <p>test</p>
                ) : (
                  <p>{podcast.title ? podcast.title.replaceAll('"', '') : 'nul'}</p>
                )}
              </>
            </h1>
            <p className="max-w-[700px] text-lg text-muted-foreground">
              {podcast.topic}
            </p>
          </div>
          <div className="flex gap-4">
            <Button variant="outline"
              onClick={() => {
                if (isEditingMetadata) {
                  savePodcastMetadata();
                  setIsEditingMetadata(false)
                } else {
                  setIsEditingMetadata(true);
                }
              }}>
              Edit title
            </Button>
          </div>
          <div className="flex gap-4">
            <PodcastSegmentsTable podcast={podcast} />
          </div>
        </>
      ) : (
        <div className='flex w-full border border-gray-100 rounded-md'>
          <LabeledSpinner label='Loading Podcast' />
        </div>
      )}
    </section>
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