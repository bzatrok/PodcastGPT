import { Podcast } from '@/models/dtos/Podcast';
import React from 'react';
import Spinner from './Spinner';

interface PodcastsTableProps {
    podcasts: Podcast[];
    showDeleteModal: (podcastId: string) => void;
}

const PodcastsTable: React.FC<PodcastsTableProps> = ({ podcasts, showDeleteModal }) => {

    return (
        <div className="flex flex-col gap-2 grow m-10 p-4 rounded border border-gray-200 box-shadow-xl bg-white h-96 overflow-y-auto bg-white">
            <div className="flex flex-row gap-2 text-lg font-medium">
                <div className="flex-1">podcastId</div>
                <div className="flex-1">title</div>
                <div className="flex-1">slug</div>
                <div className="flex-1">date</div>
                <div className="flex-1">status</div>
                <div className="flex-1">audio url</div>
                <div className="flex-1">play</div>
                <div className="flex-1">delete</div>
            </div>
            {podcasts.map((podcast) => (
                <div className="flex flex-row gap-2 p-2 border-b border-gray-200">
                    <div className="flex-1">{podcast.podcastId}</div>
                    <div className="flex-1">{podcast.title}</div>
                    <div className="flex-1">{podcast.slug}</div>
                    <div className="flex-1">{podcast.date.toString()}</div>
                    <div className="flex-1">{podcast.status}
                        {podcast.status !== "ready" && (
                            <Spinner />
                        )
                        }
                    </div>
                    {podcast.status === "ready" ? (
                        <>
                            <div className="flex-1">{`http://localhost:8080/api/stream/${podcast.podcastId}`}</div>
                            <div className="flex-1">
                                <audio controls src={`http://localhost:8080/api/stream/${podcast.podcastId}`} />
                            </div>
                        </>
                    ) : (
                        <>
                            <div className="flex-1">-</div>
                            <div className="flex-1">-</div>
                        </>
                    )
                    }
                    <div className="flex-1">
                        <button className="bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded" onClick={() => {
                            showDeleteModal(podcast.podcastId);
                        }}>
                            Delete
                        </button>
                    </div>
                </div>
            ))}
        </div>
    );
}

export default PodcastsTable;


