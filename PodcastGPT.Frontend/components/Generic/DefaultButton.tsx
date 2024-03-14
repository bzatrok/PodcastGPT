



import React from 'react';

interface DefaultButtonProps {
    title: string;
    disabled: boolean;
    onClick: () => void;
}

const DefaultButton: React.FC<DefaultButtonProps> = ({ title, disabled, onClick }) => {
    return (
        <button
            className="text-white bg-blue-700 hover:bg-blue-800 focus:ring-4 focus:outline-none focus:ring-blue-300 font-medium rounded-lg text-sm px-5 py-2.5 text-center inline-flex items-center dark:bg-blue-600 dark:hover:bg-blue-700 dark:focus:ring-blue-800"
            disabled={disabled}
            onClick={onClick}>
            {title}
        </button>
    );
}

export default DefaultButton;
