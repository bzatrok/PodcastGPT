



import React from 'react';

interface LabeledInputFieldProps {
    label: string;
    placeHolder: string;
    onChange: (value: string) => void;
}

const LabeledInputField: React.FC<LabeledInputFieldProps> = ({ label, placeHolder, onChange }) => {
    return (
        <div>
            <label
                className="block mb-2 text-sm font-medium text-gray-900 dark:text-white">
                {label}
            </label>
            <input
                type="text"
                className="bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg focus:ring-blue-500 focus:border-blue-500 block w-full p-2.5 dark:bg-gray-600 dark:border-gray-500 dark:placeholder-gray-400 dark:text-white"
                placeholder={placeHolder}
                onChange={(e) => onChange(e.target.value)}
                required />
        </div>
    );
}

export default LabeledInputField;