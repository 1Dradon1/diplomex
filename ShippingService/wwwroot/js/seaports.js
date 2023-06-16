renderSeaports();
limitValue(-90, 90, "degrees")
limitValue(0, 59, "minutes")
limitValue(0, 59, "seconds")

async function renderSeaports()
{
    let seaports = await sendPostRequest('', `https://localhost:7135/api/seaports/`)
    let parentElement = document.getElementById('seaportsContent');
    parentElement.innerHTML = '';
    for(let i = 0; i < seaports.length; i++)
    {
        let seaport = seaports[i];
        await renderSeaport(parentElement, seaport);
    }
}

async function renderSeaport(parentElement, seaport)
{
    console.log(seaport)
    let seaportElement = document.createElement('a');

    seaportElement.onclick = () => RedirectToSeaport(seaport.id);
    let latitude = getFormattedLocationString(seaport.positionX + 0.00001);
    let longitude = getFormattedLocationString(seaport.positionY + 0.00001);
    seaportElement.innerHTML = `Название: ${seaport.name} широта: ${latitude} долгота: ${longitude}`;

    let button = document.createElement('button');
    button.onclick = () => removeSeaport(seaport.id);
    button.innerHTML = 'X';
    button.style.color = 'red';
    let portDiv = document.createElement('div');
    portDiv.append(button);
    portDiv.append(seaportElement);
    parentElement.append(portDiv);
}

function RedirectToSeaport(seaportId)
{
    window.location.replace(`/html/seaportManager.html?id=${seaportId}`);
}

async function removeSeaport(seaportId)
{
    let result = await sendPostRequestWithoutParse('', `https://localhost:7135/api/seaports/remove?id=${seaportId}`);
    if (result === "OK")
    {

    }
    else
    {
        alert("Невозможно удалить морской порт");
    }
    await renderSeaports();
}

async function createSeaport()
{

    let latitude = getGeoNumber(getGeoFromInputs('latitudeDegrees', 'latitudeMinutes', 'latitudeSeconds'));
    let longitude = getGeoNumber(getGeoFromInputs('longitudeDegrees', 'longitudeMinutes', 'longitudeSeconds'));

    let options = {
        name: getValueByElementId('name'),
        positionX: latitude,
        positionY: longitude,
    };
    let optionsJSON = JSON.stringify(options);

    let result = await sendPostRequestWithoutParse(optionsJSON, `https://localhost:7135/api/seaports/add`);
    if(result === "OK")
    {

    }
    else if(result === "PositionAlreadyOccupiedException")
    {
        alert("Данная позиция уже занята другим морским портом");
    }
    else
    {
        alert("Неизвестная ошибка");
    }
    await renderSeaports();
}

function getGeoFromInputs(degreesElementId, minutesElementId, secondsElementId)
{
    let degrees = getValueByElementId(degreesElementId);
    let minutes = getValueByElementId(minutesElementId);
    let seconds = getValueByElementId(secondsElementId);

    return {
        degrees: AddNumberIfNumberIsDigit(degrees),
        minutes: AddNumberIfNumberIsDigit(minutes),
        seconds: AddNumberIfNumberIsDigit(seconds)
    }
}

function AddNumberIfNumberIsDigit(number)
{
    return Math.floor(number / 10) == 0 ? `0${number}` : number
}

function getGeoNumber(geo)
{
    return Number(`${geo.degrees}.${geo.minutes}${geo.seconds}`);
}

function limitValue(minValue, maxValue, elementId){
    let elements = document.getElementsByClassName(elementId)
    for (let i = 0; i<elements.length; i++){
        let element = elements[i]
        element.addEventListener("change", function() {
        console.log('1')
        let v = parseInt(this.value);
        if (v < minValue) this.value = minValue;
        if (v > maxValue) this.value = maxValue;
      });
    }
}
