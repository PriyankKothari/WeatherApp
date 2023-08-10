let apiKeys = [
    "Authentication_Api_Key_One",
    "Authentication_Api_Key_Two",
    "Authentication_Api_Key_Three",
    "Authentication_Api_Key_Four",
    "Authentication_Api_Key_Five"
];

export async function getCurrentWeather(cityname, countryname) {
    let errorMessage = null;

    let url = `v1.0/weather/current?city=` + (cityname || "") + '&country=' + (countryname || "");
    let apiKey = apiKeys[Math.floor(Math.random() * apiKeys.length)];
    let response = await fetch(url, { method: 'GET', headers: { 'x-api-key': apiKey } });

    if (response.status === 400) {
        errorMessage = "City Name is required.";
    }

    if (response.status === 401) {
        errorMessage = "Unauthorized Request. Validate the API KEY.";
    }

    if (response.status === 404) {
        errorMessage = "City Name not found";
    }

    if (response.status === 429) {
        errorMessage = "Too many requests. Please try again with a different API KEY or try again later.";
    }

    return await response.status === 200 ? response.json() : { status: response.status, error: errorMessage || response.statusText, data: response.json() }
}