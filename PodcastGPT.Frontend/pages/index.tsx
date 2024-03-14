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
    fetchPodcasts();
  }, []);

  const fetchPodcasts = async () => {

    const baseUrl = process.env.NEXT_PUBLIC_BASE_API_URL;
    const requestUrl = `${baseUrl}/api/podcasts`;

    const response: any = await fetch(requestUrl);
    const responseData = await response.json();

    setPodcasts(responseData.podcasts);
  };

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

  const onDelete = () => {
    fetchPodcasts();
  };

  return (
    <section className="container grid items-center gap-6 pb-8 pt-6 md:py-10">
      <div className="flex max-w-[980px] flex-col items-start gap-2">
        <h1 className="text-3xl font-extrabold leading-tight tracking-tighter md:text-4xl">
          PodcastGPT is a podcast creation tool
        </h1>
        <p className="max-w-[700px] text-lg text-muted-foreground">
          Give it a topic and a link to a news article and it'll create a podcast between two hosts, who'll discuss it for you.
          Click Create podcast to generate a podcast from a title & topic of your choice!
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
          podcastToDeleteId={podcastToDeleteId as string} 
          onDelete={onDelete} />
      </div>
    </section>
  )
}

export default Home;