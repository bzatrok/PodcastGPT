import { Podcast } from '@/models/dtos/Podcast';
import React from 'react';

interface PodcastSegmentsTableProps {
    podcast: Podcast;
}

const PodcastSegmentsTable: React.FC<PodcastSegmentsTableProps> = ({ podcast }) => {

    return (
        <div className="flex flex-col gap-2 grow m-10 p-4 rounded border border-gray-200 box-shadow-xl bg-white h-96 overflow-y-auto bg-white">
            <div className="flex flex-row gap-2 text-lg font-medium">
                <div className="flex-1">order</div>
                <div className="flex-1">content</div>
            </div>
            {podcast.podcastSegments && podcast.podcastSegments.map((podcastSegment) => (
                <div 
                key={`${podcastSegment.podcastSegmentId}`}
                className="flex flex-row gap-2 p-2 border-b border-gray-200">
                    <div className="flex-1">{podcastSegment.order}</div>
                    <div className="flex-1">{podcastSegment.textContent}</div>
                </div>
            ))}
        </div>
    );
}

export default PodcastSegmentsTable;


