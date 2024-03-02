import React, { useState, useEffect, useRef } from 'react';

type SearchModalProps = {
    showModal: boolean;
    setShowModal: (showModal: boolean) => void;
}

const SearchModal: React.FC<SearchModalProps> = ({ showModal, setShowModal }) => {
    const [products, setProducts] = useState<any[]>([]);

    const inputRef = useRef<HTMLInputElement>(null);

    const [searchTerm, setSearchTerm] = useState('');
    const debouncedSearchTerm = useDebounce(searchTerm, 200);

    const handleOverlayClick = () => {
        setShowModal(false);
    };

    const handleModalClick = (e: any) => {
        e.stopPropagation();
    };

    function useDebounce(value: string, delay: number) {
        const [debouncedValue, setDebouncedValue] = useState(value);

        useEffect(() => {
            const handler = setTimeout(() => {
                setDebouncedValue(value);
            }, delay);

            return () => {
                clearTimeout(handler);
            };
        }, [value, delay]);

        return debouncedValue;
    }

    useEffect(() => {
        if (debouncedSearchTerm && debouncedSearchTerm.length > 1) {
            runSearch(debouncedSearchTerm);
        } else {
            setProducts([]);
        }
    }, [debouncedSearchTerm]);

    useEffect(() => {
        if (showModal && inputRef.current) {
            inputRef.current.focus();
        }
    }, [showModal, inputRef]);

    const runSearch = async (searchTerm: string) => {
        // Example HTTP request
        const response = await fetch(`/api/products/search?searchTerm=${searchTerm}`);
        const data = await response.json();
        setProducts(data.products);
    };

    return (
        <div onClick={handleOverlayClick} className={`${showModal ? "" : "hidden"} fixed inset-0 z-40 bg-black bg-opacity-50 flex justify-center items-center`}>
            <div onClick={handleModalClick} id="default-modal" aria-hidden={showModal ? "true" : "false"}
                className={(showModal ? "" : "hidden ") + "absolute top-0 md:top-20 p-4 w-full max-w-5xl"}>
                <div className="relative md:p-4 w-full max-w-5xl max-h-full">
                    <div className="relative bg-white rounded-lg shadow dark:bg-gray-700">
                        <div className="flex items-center justify-between p-4 md:p-5 border-b rounded-t dark:border-gray-600">
                            <h3 className="text-xl font-semibold text-gray-900 dark:text-white">
                                Search
                            </h3>
                            <button onClick={() => setShowModal(false)} type="button" className="text-gray-400 bg-transparent hover:bg-gray-200 hover:text-gray-900 rounded-lg text-sm w-8 h-8 ms-auto inline-flex justify-center items-center dark:hover:bg-gray-600 dark:hover:text-white">
                                <svg className="w-3 h-3" aria-hidden="true" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 14 14">
                                    <path stroke="currentColor" strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="m1 1 6 6m0 0 6 6M7 7l6-6M7 7l-6 6" />
                                </svg>
                                <span className="sr-only">Close modal</span>
                            </button>
                        </div>
                        <div className="p-4 md:p-5 space-y-4">
                            <div>
                                <form>
                                    <label className="mb-2 text-sm font-medium text-gray-900 sr-only dark:text-white">Search</label>
                                    <div className="relative">
                                        <div className="absolute inset-y-0 start-0 flex items-center ps-3 pointer-events-none">
                                            <svg className="w-4 h-4 text-gray-500 dark:text-gray-400" aria-hidden="true" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 20 20">
                                                <path stroke="currentColor" strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="m19 19-4-4m0-7A7 7 0 1 1 1 8a7 7 0 0 1 14 0Z" />
                                            </svg>
                                        </div>
                                        <input
                                            ref={inputRef}
                                            onChange={(e) => setSearchTerm(e.target.value)}
                                            type="search" id="default-search" className="block w-full p-4 ps-10 text-sm text-gray-900 border border-gray-300 rounded-lg bg-gray-50 focus:ring-blue-500 focus:border-blue-500 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-blue-500 dark:focus:border-blue-500" placeholder="Search Mockups, Logos..." required />
                                    </div>
                                </form>

                                <div className="mt-2 relative overflow-x-auto shadow-md sm:rounded-lg" style={{ maxHeight: '300px', overflowY: 'auto' }}>
                                    <table className="w-full text-sm text-left rtl:text-right text-gray-500 dark:text-gray-400">
                                        <thead className="text-xs text-gray-700 uppercase bg-gray-50 dark:bg-gray-700 dark:text-gray-400">
                                            <tr>
                                                <th scope="col" className="hidden md:block px-16 py-3">
                                                    <span className="sr-only">Image</span>
                                                </th>
                                                <th scope="col" className="px-6 py-3">
                                                    Products
                                                </th>
                                                <th scope="col" className="px-6 py-3">
                                                    <span className="sr-only">Description</span>
                                                </th>
                                                <th scope="col" className="px-6 py-3">
                                                    <span className="sr-only">Price</span>
                                                </th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            {products && products.length > 0 ? (
                                                products.map((product, index) => (
                                                    <tr key={index} className="bg-white border-b dark:bg-gray-800 dark:border-gray-700 hover:bg-gray-50 dark:hover:bg-gray-600">
                                                        <td className="hidden md:block p-4">
                                                            <img src={product.imageUrl} className="max-w-16 max-h-12" alt={product.name} />
                                                        </td>
                                                        <td className="px-6 py-4 font-semibold text-gray-900 dark:text-white">
                                                            {product.name}
                                                        </td>
                                                        <td className="px-6 py-4">
                                                            {product.description}
                                                        </td>
                                                        <td className="w-40 px-6 py-4 font-semibold text-gray-900 dark:text-white">
                                                            {product.price}
                                                        </td>
                                                    </tr>
                                                ))
                                            ) : (
                                                <tr>
                                                    <td className="text-center py-4">
                                                        No products found.
                                                    </td>
                                                </tr>
                                            )}
                                        </tbody>
                                    </table>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
}

export default SearchModal;
