



import React from 'react';
import { Checkbox } from '../Base/Checkbox';

interface LabeledCheckboxProps {
    label: string;
    subtitle: string;
    onChange: (value: boolean) => void;
}

const LabeledCheckbox: React.FC<LabeledCheckboxProps> = ({ label, subtitle, onChange }) => {
    return (
        <div className="items-top flex space-x-2">
            <Checkbox onCheckedChange={(checked) => onChange(checked.value)} id="terms1" />
            <div className="grid gap-1.5 leading-none">
                <label
                    htmlFor="terms1"
                    className="text-sm font-medium leading-none peer-disabled:cursor-not-allowed peer-disabled:opacity-70"
                >
                    {label}
                </label>
                <p className="text-sm text-muted-foreground">
                    {subtitle}
                </p>
            </div>
        </div>
    )
}

export default LabeledCheckbox;