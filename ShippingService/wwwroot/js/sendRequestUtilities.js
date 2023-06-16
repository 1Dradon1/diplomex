"use strict";

function sendPostRequestWithoutParse(json, uri) {
    const myHeaders = new Headers();
    myHeaders.append('Content-Type', 'application/json');
    const request = new Request(uri, {
        method: 'POST',
        body: json,
        headers:myHeaders
    });
    
    let search_result = fetch(request)
        .then((response) => {
            return response.text()
        });

    return search_result;
}

function sendGetRequestWithoutParse(uri) {
    const myHeaders = new Headers();
    myHeaders.append('Content-Type', 'application/json');
    const request = new Request(uri, {
        method: 'GET',
        headers:myHeaders
    });
    
    let search_result = fetch(request)
        .then((response) => {
            return response.text()
        })

    return search_result;
}


function sendPostRequest(json, uri) {
    const myHeaders = new Headers()
    myHeaders.append('Content-Type', 'application/json')
    const request = new Request(uri, {
        method: 'POST',
        body: json,
        headers:myHeaders
    });
    
    let search_result = fetch(request)
        .then((response) => {
            return response.json()
        })

    return search_result;
}

function sendGetRequest(uri) {
    const myHeaders = new Headers()
    myHeaders.append('Content-Type', 'application/json')
    const request = new Request(uri, {
        method: 'GET',
        headers:myHeaders
    });
    
    let search_result = fetch(request)
        .then((response) => {
            return response.json()
        })

    return search_result;
}

function getValueByElementId(elementId)
{
    let element = document.getElementById(elementId);
    return element.value;
}

function redirectToHome()
{
    window.location.replace("/html/seaports.html");
}

function numberToGeo(value)
{
    value += 0.0000000000001;

    let valueString = value.toString();
    let parts = valueString.split('.');
    if (parts.length == 1)
        return value;
    let secondPart = parts[1];

    let degrees = parts[0];
    let minutes = secondPart.substring(0, 2);
    let seconds = secondPart.substring(2, 4);

    return { 
        degrees: degrees,
        minutes: minutes,
        seconds: seconds,
    }
}

function getFormattedLocationStringFromGeo(geo)
{
    return `${geo.degrees}°${geo.minutes}′${geo.seconds}″`;
}

function getFormattedLocationString(number)
{
    let geo = numberToGeo(number);
    let formattedLocaitonString = getFormattedLocationStringFromGeo(geo);

    return formattedLocaitonString;
}