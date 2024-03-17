import { Podcast } from '@/models/dtos/Podcast';
import React from 'react';

import {
    Table,
    TableBody,
    TableCaption,
    TableCell,
    TableHead,
    TableHeader,
    TableRow,
} from "@/components/Base/Table"

import { Button } from "@/components/Base/Button"

interface PodcastSegmentsTableProps {
    podcast: Podcast;
}

const PodcastSegmentsTable: React.FC<PodcastSegmentsTableProps> = ({ podcast }) => {

    return (
        <Table className='border border-grey-200'>
            <TableHeader>
                <TableRow>
                    <TableHead>Segment Index</TableHead>
                    <TableHead>Segment Content</TableHead>
                    <TableHead className='sr-only'>edit</TableHead>
                    <TableHead className='sr-only'>delete</TableHead>
                </TableRow>
            </TableHeader>
            <TableBody>
                {podcast.podcastSegments && podcast.podcastSegments.map((podcastSegment) => (
                    <TableRow
                        key={`${podcast.slug}_${podcast.status}`}>
                        <TableCell>{podcastSegment.order}</TableCell>
                        <TableCell>{podcastSegment.textContent}</TableCell>
                        <TableCell>
                            <Button variant="outline"
                            // disabled={podcast.status !== "ready"}
                            // onClick={() => {
                            //     showDeleteModal(podcast.podcastId);
                            // }}>
                            >
                                Edit
                            </Button>
                        </TableCell>
                        <TableCell>
                            <Button variant="outline"
                            // disabled={podcast.status !== "ready"}
                            // onClick={() => {
                            //     showDeleteModal(podcast.podcastId);
                            // }}>
                            >
                                Delete
                            </Button>
                        </TableCell>
                    </TableRow>
                ))}
            </TableBody>
        </Table>
    );
}

export default PodcastSegmentsTable;


