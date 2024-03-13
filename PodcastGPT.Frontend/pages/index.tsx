'use client'
import { useEffect, useState } from 'react'

import { Podcast } from '@/models/dtos/Podcast'
import PodcastsTable from '@/components/Podcast/PodcastsTable'
import CreatePodcastModal from '@/components/Podcast/CreatePodcastModal'
import DeletePodcastModal from '@/components/Podcast/DeletePodcastModal'
import { Button } from '@/components/Base/Button'

const Home: React.FC = () => {

  const [showModalType, setShowModalType] = useState<string | null>(null);
  const [podcastToDeleteId, setPodcastToDeleteId] = useState<string | null>(null);

  const [podcasts, setPodcasts] = useState<Podcast[]>([]);

  useEffect(() => {
    const fetchPodcasts = async () => {

      const baseUrl = process.env.NEXT_PUBLIC_BASE_API_URL;
      const requestUrl = `${baseUrl}/api/podcasts`;

      const response: any = await fetch(requestUrl);
      const responseData = await response.json();

      setPodcasts(responseData.podcasts);
    };

    fetchPodcasts();
  }, []);

  const newPodcastCreated = (newPodcast: Podcast) => {
    setPodcasts([...podcasts, newPodcast]);
  }

  const showDeleteModal = (podcastId: string) => {
    setShowModalType("delete");
    setPodcastToDeleteId(podcastId);
  }

  const showCreateModal = () => {
    setShowModalType("create");
  }

  const setShowModal = (show: boolean) => {
    setShowModalType(null);
  }

  return (
    <section className="container grid items-center gap-6 pb-8 pt-6 md:py-10">
      <div className="flex max-w-[980px] flex-col items-start gap-2">
        <h1 className="text-3xl font-extrabold leading-tight tracking-tighter md:text-4xl">
          Podcast creationtoo <br className="hidden sm:inline" />
          built with Radix UI and Tailwind CSS.
        </h1>
        <p className="max-w-[700px] text-lg text-muted-foreground">
          Accessible and customizable components that you can copy and paste
          into your apps. Free. Open Source. And Next.js 13 Ready.
        </p>
      </div>
      <div className="flex gap-4">
        <Button variant="outline"
          onClick={() => {
            showCreateModal();
          }}>
          Create Podcast
        </Button>
      </div>
      <div className="flex gap-4">
        {/* <Navbar
          setShowModal={showCreateModal} /> */}
        <PodcastsTable
          podcasts={podcasts}
          showDeleteModal={showDeleteModal} />
        <CreatePodcastModal
          showModal={showModalType === "create"}
          setShowModal={setShowModal}
          newPodcastCreated={newPodcastCreated} />
        <DeletePodcastModal
          showModal={showModalType === "delete"}
          setShowModal={setShowModal}
          podcastToDeleteId={podcastToDeleteId as string} />
      </div>
    </section>
  )
}

export default Home;