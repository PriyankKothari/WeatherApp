let apiKeys = [
    "Authentication_Api_Key_One",
    "Authentication_Api_Key_Two",
    "Authentication_Api_Key_Three",
    "Authentication_Api_Key_Four",
    "Authentication_Api_Key_Five"
];

export async function getCurrentWeather(cityname, countryname) {
    let url = `v1.0/weather/current?city=` + (cityname || "") + '&country=' + (countryname || "");
    let apiKey = apiKeys[Math.floor(Math.random() * apiKeys.length)];
    let response = await fetch(url, { method: 'GET', headers: { 'x-api-key': apiKey } });

    return await response.json();
}