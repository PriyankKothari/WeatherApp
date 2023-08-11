# WeatherApp
A ReactJS front-end and .NET 6 API that retrieves the current weather of the provided city and country using the [Open Weather Map APIs](https://p[enweathermap.org).

## Source Code
[GitHub](https://github.com/PriyankKothari/WeatherApp/)

## API Info
- API: [Open Weather APIs](https://openweathermap.org/current)
- Method: GET
- URL: https://api.openweathermap.org/data/2.5/weather?q={city_name}&appid={api_key}

## Getting Started
- Clone the repository:
git clone https://github.com/PriyankKothari/WeatherApp.git
  - Visual Studio
    - Open solution (weatherapp.sln) in Visual Studio
    - Press F5 or Debug solution
    - It should open a web application on the default browser and API running in the background
    - Press Shift + F5 at anytime to stop debugging
  - Visual Studio Code
    - Open Source Control --> Select Folder --> Select WeatherApp.Api folder from the repository
    - Open Split Terminal, one for API and one for UI
      - API
        - Run `dotnet run` command
        - Press `ctrl + c` to stop debugging
      - UI
        - Change directory to `ClientApp\src`
        - Run `npm run start` command
        - Press `ctrl + c` to stop debugging

## URLs
- API: https://localhost:7029
- UI: https://localhost:44415
- Swagger: https://localhost:7029/swagger
