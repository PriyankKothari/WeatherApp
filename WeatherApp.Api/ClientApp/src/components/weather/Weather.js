import React, { Component } from 'react';
import 'bootstrap/dist/css/bootstrap.min.css';
import { Location } from './Location'
import { Report } from './Report'
import { getCurrentWeather } from '../../services/WeatherService'
import { Message } from '../../components/common/Message';

export class Weather extends Component {
    static displayName = Weather.name;

    state = {
        location: {},
        weatherData: {},
        message: {}
    }

    getCurrentWeather = (e) => {
        let location = this.state.location
        getCurrentWeather(location.cityname, location.countryname)
            .then(response => {
                if (response) {
                    if (response.data) {
                        this.setState({ message: { errorMessage: null } });
                        this.setState({ weatherData: response.data });
                    }
                    else if (response.errors) {
                        this.setState({ message: { errorMessage: response.errors.join(" ") } });
                        this.setState({ weatherData: null });
                    }
                    else {
                        this.setState({ message: { errorMessage: response } });
                    }
                }
            })
    }

    onChangeForm = (e) => {
        let location = this.state.location

        if (e.target.name === 'cityname') {
            location.cityname = e.target.value;
        }
        if (e.target.name === 'countryname') {
            location.countryname = e.target.value;
        }

        this.setState({ location })
    }

    render() {
        return (
            <div className="App">
                <div className="container mrgnbtm">
                    <div className="row">
                        <div className="col-md-8">
                            <Location onChangeForm={this.onChangeForm} getCurrentWeather={this.getCurrentWeather}></Location>
                        </div>
                        <div className="col-md-8">
                            <Message message={this.state.message}></Message>
                        </div>
                        <div className="col-md-8">
                            <Report weatherData={this.state.weatherData}></Report>
                        </div>
                    </div>
                </div>
                <div className="row mrgnbtm"></div>
            </div>
        );
    }
}