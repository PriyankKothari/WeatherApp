import React from 'react'
import '../style.css';
import moment from 'moment';

export const Report = ({ weatherData }) => {
    if (weatherData === null) {
        return (
            <div className="main">
                <p className="header">City: </p>
                <div className="flex"><p className="day"> {moment().format('dddd')}, <span>{moment().format('LL')}</span></p></div>
                <div className="flex"></div>
                <div className="flex"></div>
            </div>
        )
    }
    else {
        return (
            <div className="main">
                <p className="header">
                    City: {weatherData !== null ? weatherData.city : ""}
                    {(weatherData !== null && weatherData.city !== undefined) ? " - " : ""}
                    {weatherData !== null ? weatherData.countryCode : ""}
                </p>
                <div className="flex">
                    <p className="day">
                        {moment().format('dddd')},
                        <span>
                            {moment().format('LL')}
                        </span>
                    </p>
                    <p className="description">
                        {weatherData !== null ? weatherData.description : ""}
                    </p>
                </div>
                <div className="flex"></div>
                <div className="flex"></div>
            </div>
        )
    }
}