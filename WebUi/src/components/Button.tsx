import * as React from 'react';
import './Button.css';

export interface Props {
    label: string;
    count: number;
    onClick?: () => void;
}

function Button({ label, count = 1, onClick }: Props) {
    return (
        <div className="button noselect" onClick={onClick}>
            {label} - {count}
        </div>
    );
}

export default Button;
