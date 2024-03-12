import { useEffect, useState } from 'react'

import Navbar from '@/components/Generic/Navbar'
import { Podcast } from '@/models/dtos/Podcast'
import PodcastsTable from '@/components/Podcast/PodcastsTable'
import CreatePodcastModal from '@/components/Podcast/CreatePodcastModal'
import DeletePodcastModal from '@/components/Podcast/DeletePodcastModal'
 
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
    <>
      <Navbar
        setShowModal={showCreateModal} />
      <PodcastsTable 
      podcasts={podcasts} 
      showDeleteModal={showDeleteModal}/>
      <CreatePodcastModal
        showModal={showModalType === "create"}
        setShowModal={setShowModal} 
        newPodcastCreated={newPodcastCreated} />
      <DeletePodcastModal
        showModal={showModalType === "delete"}
        setShowModal={setShowModal} 
        podcastToDeleteId={podcastToDeleteId as string} />
    </>
  )
}

export default Home;