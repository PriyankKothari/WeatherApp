import React from 'react'
import './../style.css';

export const Message = ({ message }) => {    
    return (
        <div className="main-error">
            <div className="flex-error error"><div>{message.errorMessage || ""}</div></div>
        </div>
    )
}