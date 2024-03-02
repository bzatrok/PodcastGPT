import React, { useState } from 'react';
import LabeledInputField from './LabeledInputField';
import { PodcastCreationRequest } from '@/models/requests/PodcastCreationRequest';
import DefaultButton from './DefaultButton';
import { createPodcast } from '@/utils/podcastService';
import { PodcastDetailResponse } from '@/models/responses/PodcastDetailResponse';
import { Podcast } from '@/models/dtos/Podcast';

type CreatePodcastModalProps = {
    showModal: boolean;
    setShowModal: (showModal: boolean) => void;
    newPodcastCreated: (newPodcast: Podcast) => void;
}

const CreatePodcastModal: React.FC<CreatePodcastModalProps> = ({ showModal, setShowModal, newPodcastCreated }) => {
    const [podcastCreationRequest, setPodcastCreationRequest] = useState<PodcastCreationRequest>({} as PodcastCreationRequest);

    // OVERLAY

    const handleOverlayClick = () => {
        setShowModal(false);
    };

    const handleModalClick = (e: any) => {
        e.stopPropagation();
    };

    const handleCreationFinished = (e: Podcast) => {
        newPodcastCreated(e);
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
                                Create a new podcast
                            </h3>
                        </div>
                        <div className="p-4 md:p-5 space-y-4">
                            {/* <LabeledInputField
                                label="Podcast Title"
                                placeHolder="The future of space exploration"
                                onChange={(e) => setPodcastCreationRequest({
                                    ...podcastCreationRequest, 
                                    PodcastTitle: e
                                })} /> */}
                            <LabeledInputField
                                label="Podcast Topic"
                                placeHolder="Discovery of life on Mars..."
                                onChange={(e) => setPodcastCreationRequest({
                                    ...podcastCreationRequest, 
                                    PodcastTopic: e
                                })} />
                            <LabeledInputField
                                label="Podcast News Article URL"
                                placeHolder="https://article.com/topic1"
                                onChange={(e) => setPodcastCreationRequest({
                                    ...podcastCreationRequest, 
                                    PodcastNewsArticleUrl: e
                                })} />
                            <DefaultButton
                                title="Create Podcast"
                                onClick={() => {
                                    createPodcast(podcastCreationRequest, handleCreationFinished);
                                }}
                            />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
}

export default CreatePodcastModal;
