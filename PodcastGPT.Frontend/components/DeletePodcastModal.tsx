import React from 'react';
import DefaultButton from './DefaultButton';
import { deletePodcastById } from '@/utils/podcastService';

type DeletePodcastModalProps = {
    showModal: boolean;
    setShowModal: (showModal: boolean) => void;
    podcastToDeleteId: string;
}

const DeletePodcastModal: React.FC<DeletePodcastModalProps> = ({ showModal, setShowModal, podcastToDeleteId }) => {

    const handleOverlayClick = () => {
        setShowModal(false);
    };

    const handleModalClick = (e: any) => {
        e.stopPropagation();
    };

    const handleDeletionFinished = () => {
        setShowModal(false);
    }

    return (
        <div onClick={handleOverlayClick} className={`${showModal ? "" : "hidden"} fixed inset-0 z-40 bg-black bg-opacity-50 flex justify-center items-center`}>
            <div onClick={handleModalClick} id="default-modal" aria-hidden={showModal ? "true" : "false"}
                className={(showModal ? "" : "hidden ") + "absolute top-0 md:top-20 p-4 w-full max-w-3xl"}>
                <div className="relative md:p-4 w-full max-w-5xl max-h-full">
                    <div className="relative bg-white rounded-lg shadow dark:bg-gray-700">
                        <div className="flex items-center justify-between p-4 md:p-5 border-b rounded-t dark:border-gray-600">
                            <h3 className="text-xl font-semibold text-gray-900 dark:text-white">
                                Delete podcast
                            </h3>
                        </div>
                        <div className="p-4 md:p-5 space-y-4">
                            <p>Are you sure you want to delete this podcast?</p>
                            <DefaultButton
                                title="Delete Podcast"
                                onClick={async () => {
                                    deletePodcastById(podcastToDeleteId);
                                    handleDeletionFinished();
                                }}
                            />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
}

export default DeletePodcastModal;
