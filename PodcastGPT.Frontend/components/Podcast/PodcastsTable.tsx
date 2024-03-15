import { Podcast } from '@/models/dtos/Podcast';
import React from 'react';
import LabeledSpinner from '../Generic/LabeledSpinner';

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

interface PodcastsTableProps {
    podcasts: Podcast[];
    showDeleteModal: (podcastId: string) => void;
}

const PodcastsTable: React.FC<PodcastsTableProps> = ({ podcasts, showDeleteModal }) => {

    return (
        <Table className='border border-grey-200'>
            <TableCaption>Your Podcasts</TableCaption>
            <TableHeader>
                <TableRow>
                    <TableHead>Podcast Title</TableHead>
                    <TableHead>Podcast Topic</TableHead>
                    <TableHead>Slug</TableHead>
                    <TableHead>Creation Date</TableHead>
                    <TableHead>Generation Status</TableHead>
                    <TableHead>Listen</TableHead>
                    <TableHead className='sr-only'>edit</TableHead>
                    <TableHead className='sr-only'>delete</TableHead>
                </TableRow>
            </TableHeader>
            <TableBody>
                {podcasts && podcasts.map((podcast) => (
                    <TableRow
                        key={`${podcast.podcastId}_${podcast.status}`}>
                        <TableCell>{podcast.title}</TableCell>
                        <TableCell>{podcast.topic}</TableCell>
                        <TableCell>{podcast.slug}</TableCell>
                        <TableCell>{podcast.date.toString()}</TableCell>
                        <TableCell>
                            <div className='flex flex-col items-center'>
                                {podcast.status !== "ready" ? (
                                    <LabeledSpinner label={podcast.status} />
                                ) : (
                                <p>
                                    {podcast.status}
                                </p>)
                                }
                            </div>
                        </TableCell>
                        {podcast.status === "ready" ? (
                            <>
                                <TableCell>
                                    <audio controls src={`http://localhost:8080/api/file/podcast-audio/${podcast.podcastId}`} />
                                </TableCell>
                                <TableCell>
                                    <a href={`http://localhost:3000/${podcast.slug}`} >
                                        <Button variant="outline">Edit</Button>
                                    </a>
                                </TableCell>
                            </>
                        ) : (
                            <>
                                <TableCell>-</TableCell>
                                <TableCell>-</TableCell>
                                <TableCell>-</TableCell>
                            </>
                        )
                        }

                        <TableCell>
                            <Button variant="outline"
                                // disabled={podcast.status !== "ready"}
                                onClick={() => {
                                    showDeleteModal(podcast.podcastId);
                                }}>
                                Delete
                            </Button>
                        </TableCell>
                    </TableRow>
                ))}
            </TableBody>
        </Table>
    );

    // return (
    //     <div className="flex flex-col gap-2 grow m-10 p-4 rounded border border-gray-200 box-shadow-xl bg-white h-96 overflow-y-auto bg-white">
    //         <div className="flex flex-row gap-2 text-xl text-center font-medium">
    //             <div className="flex-1">title</div>
    //             <div className="flex-1">topic</div>
    //             <div className="flex-1">slug</div>
    //             <div className="flex-1">date</div>
    //             <div className="flex-1">status</div>
    //             <div className="flex-1 bg-red-100">play</div>
    //             <div className="flex-1">edit</div>
    //             <div className="flex-1">delete</div>
    //         </div>
    //         {podcasts.map((podcast) => (
    //             <div
    //                 key={`${podcast.podcastId}_${podcast.status}`}
    //                 className="flex flex-row gap-2 p-2 border-b border-gray-200">
    //                 <div className="flex-1">{podcast.title}</div>
    //                 <div className="flex-1">{podcast.topic}</div>
    //                 <div className="flex-1">{podcast.slug}</div>
    //                 <div className="flex-1">{podcast.date.toString()}</div>
    //                 <div className="flex-1">{podcast.status}
    //                     {podcast.status !== "ready" && (
    //                         <Spinner />
    //                     )
    //                     }
    //                 </div>
    //                 {podcast.status === "ready" ? (
    //                     <>
    //                         <div className="flex-3">
    //                             <audio controls src={`http://localhost:8080/api/stream/${podcast.podcastId}`} />
    //                         </div>
    //                         <div className="flex-1">
    //                             <a href={`http://localhost:3000/${podcast.slug}`} className="bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded">
    //                                 Edit
    //                             </a>
    //                         </div>
    //                     </>
    //                 ) : (
    //                     <>
    //                         <div className="flex-1">-</div>
    //                         <div className="flex-1">-</div>
    //                         <div className="flex-1">-</div>
    //                     </>
    //                 )
    //                 }

    //                 <div className="flex-1">
    //                     <button
    //                         className="bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded"
    //                         disabled={podcast.status !== "ready"}
    //                         onClick={() => {
    //                             showDeleteModal(podcast.podcastId);
    //                         }}>
    //                         Delete
    //                     </button>
    //                 </div>
    //             </div>
    //         ))}
    //     </div>
    // );
}

export default PodcastsTable;


