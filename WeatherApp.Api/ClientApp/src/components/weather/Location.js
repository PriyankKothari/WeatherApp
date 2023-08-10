import React from 'react'

export const Location = ({ onChangeForm, getCurrentWeather }) => {

return (
    <div className="container">
        <div className="row">
            <div className="col-md-7 mrgnbtm">
                <h2>Get Weather Report</h2>
                <form>
                    <div className="row">
                        <div className="form-group col-md-6">
                            <input type="text" onChange={(e) => onChangeForm(e)} className="form-control" name="cityname" placeholder="City Name" required />
                        </div>
                        <div className="form-group col-md-6">
                            <input type="text" onChange={(e) => onChangeForm(e)} className="form-control" name="countryname" placeholder="Country Name" required />
                        </div>
                    </div>
                    <div className="row">
                        <div className="form-group col-md-12">
                            <label htmlFor="exampleInputEmail1"></label>
                        </div>
                    </div>
                    <button type="button" onClick={(e) => getCurrentWeather()} className="btn btn-success">Generate Weather Report</button>
                </form>
            </div>
        </div>
    </div>
)
}