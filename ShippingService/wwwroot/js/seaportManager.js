"use strict";


const size = 4;
generateStorageTable(size);

let seaportId = getSeaportId();
let currentPage = 0;
let itemsCountPerPage = 16;
let selectedCargoIds = new Set();
let cargoByPoint = { };

renderSeaportInfo();
updateTableContent(currentPage, itemsCountPerPage);
setAvailableSeaports();
updateMooredShips();

function generateStorageTable(size)
{
    let tableElement = document.getElementById('storageTable');
    for(let i = 0; i < size; i++)
    {
        let row = document.createElement('tr');
        for(let j = 0; j < size; j++)
        {
            let cell = document.createElement('td');
            cell.className = 'storageItem';
            cell.innerHTML = 'asdasdsaddas';
            cell.style.visibility = 'hidden';
            cell.id = `${j}|${i}`;
            cell.onclick = () => handleCargoClick(j, i);
            row.append(cell);
        }
        tableElement.append(row);
    }
}

async function renderSeaportInfo()
{
    let seaport = await sendPostRequest('', `https://localhost:7135/api/seaports/${seaportId}`);
    setInnerHtmlById('seaportName', `Порт "${seaport.name}"`);
    let latitude = seaport.positionX;
    let formattedLatitude = getFormattedLocationString(latitude)
    setInnerHtmlById('positionX', `Широта: "${formattedLatitude}"`);

    let longitude = seaport.positionY;
    let formattedLongitude = getFormattedLocationString(longitude);
    setInnerHtmlById('positionY', `Долгота: "${formattedLongitude}"`);
}

function setInnerHtmlById(id, innerHTML)
{
    let element = document.getElementById(id);
    element.innerHTML = innerHTML;
}

function handleCargoClick(x, y)
{
    let key = `${x}|${y}`;
    if(! key in cargoByPoint)
        return;

    let cargo = cargoByPoint[key];
    if(selectedCargoIds.has(cargo.id))
        selectedCargoIds.delete(cargo.id);
    else
        selectedCargoIds.add(cargo.id);
    
    updateTableContent(currentPage, itemsCountPerPage);
}

async function updateTableContent(currentPage, itemsCountPerPage)
{
    cargoByPoint = {};
    updateButtonStates();
    let pagingOptionsJSON = getPagingOptionsJSON(currentPage, itemsCountPerPage);
    let cargos = await sendPostRequest(pagingOptionsJSON, `https://localhost:7135/api/cargos/${seaportId}`);
    for(let i = 0; i < cargos.length; i++)
    {
        let cargo = cargos[i];
        let x = Math.floor(i % size);
        let y = Math.floor(i / size);

        let cargoElementId = `${x}|${y}`;
        cargoByPoint[cargoElementId] = cargo;
    }

    for(let y = 0; y < size; y++)
    {
        for(let x = 0; x < size; x++)
        {
            let cargoElementId = `${x}|${y}`;
            let cargoElement = document.getElementById(cargoElementId);
            
            if(!(cargoElementId in cargoByPoint))
            {
                cargoElement.style.visibility = 'hidden';
                continue;
            }
            
            let cargo = cargoByPoint[cargoElementId];
            cargoElement.style.visibility = 'visible';
            cargoElement.innerHTML = cargo.title;

            cargoElement.style.backgroundColor = (selectedCargoIds.has(cargo.id)) ? 'grey' : 'white';
        }
    }

    let currentPageElement = document.getElementById("currentPage")
    currentPageElement.innerHTML = currentPage+1;
}

async function updateButtonStates()
{
    let pagesCount = await GetStoragePagesCount();
    let leftButton = document.getElementById('leftButton');
    leftButton.disabled = currentPage <= 0;

    let rightButton = document.getElementById('rightButton');
    rightButton.disabled = currentPage >= pagesCount - 1;
}


async function moveLeft()
{
    currentPage--;
    updateTableContent(currentPage, itemsCountPerPage);
}

async function moveRight()
{
    currentPage++;
    updateTableContent(currentPage, itemsCountPerPage);
}

async function GetStoragePagesCount()
{
    return await sendPostRequest('', `https://localhost:7135/api/cargos/storage/${seaportId}?itemsCountPerPage=${itemsCountPerPage}`);
}

function getPagingOptionsJSON(currentPage, itemsCountPerPage)
{
    let pagingOptions = {
        "page": currentPage,
        "itemsCountPerPage": itemsCountPerPage
    };

    return JSON.stringify(pagingOptions);
}

function getSeaportId()
{
    const params = new Proxy(new URLSearchParams(window.location.search), {
        get: (searchParams, prop) => searchParams.get(prop),
      });
      let value = params.id;
      return value;
}

async function setAvailableSeaports()
{
    let availableSeaports = await sendPostRequest('', `https://localhost:7135/api/seaports`);
    
    let element = document.getElementById('availableSeaports');
    for(let i = 0; i < availableSeaports.length; i++)
    {
        let seaport = availableSeaports[i];
        if(seaport.id == seaportId) continue;
        let option = document.createElement('option');
        option.value = seaport.id;
        option.innerHTML = `${seaport.name}.${seaport.id}`;
        element.append(option);
    }
}

async function sendCargos()
{
    let toSeaportSelection = document.getElementById('availableSeaports');
    let toSeaportId = Number(toSeaportSelection.value);
    let options = {
        cargoIds: Array.from(selectedCargoIds),
        fromSeaportId: seaportId,
        toSeaportId: toSeaportId
    }
    let optionsJSON = JSON.stringify(options);
    
    let result = await sendPostRequestWithoutParse(optionsJSON, `https://localhost:7135/api/cargos/send`);
    if(result === 'OK')
    {
        
    }
    else if(result === 'BestShipFindingException')
    {
        alert('Не удается найти подходящий корабль');
    }
    else
    {
        alert('Ошибка во время отправки груза');
    }
    await hotUpdateCargosAndMooredShips(true);
}

async function hotUpdateCargosAndMooredShips(unselectCargos)
{
    currentPage = 0;
    cargoByPoint = {};

    if(unselectCargos)
        selectedCargoIds = new Set();
    
    await updateTableContent(currentPage, itemsCountPerPage);
    await updateMooredShips();
}

async function updateMooredShips()
{
    let mooredShipsElement = document.getElementById('mooredShips');
    mooredShipsElement.innerHTML = '<h1>Moored ships</h1>';

    let mooredShipOverviews = await sendPostRequest('', `https://localhost:7135/api/ships/moored?seaportId=${seaportId}`);
    for(let i = 0; i < mooredShipOverviews.length; i++)
    {
        let mooredShipOverview = mooredShipOverviews[i];
        let mooredShip = mooredShipOverview.mooredShip;
        let ship = mooredShip.ship;
        let cargoCount = mooredShipOverview.cargoCount;
        
        let mooredShipElement = document.createElement('p');
        mooredShipElement.innerHTML = `${ship.name} груза:${cargoCount}`
        
        let button = document.createElement('button');
        button.innerHTML = 'Разгрузить';
        button.onclick = async () => await HandleUnloadButton(ship.id);
        button.disabled = cargoCount == 0;

        let removeButton = document.createElement('button');
        removeButton.onclick = () => removeShip(ship.id);
        removeButton.innerHTML = 'X';
        removeButton.style.color = 'red';

        let shipContainerElement = document.createElement('div')
        let shipButtonsElement = document.createElement('div')
        
        shipContainerElement.append(mooredShipElement, shipButtonsElement);
        shipButtonsElement.append(button, removeButton);
        
        mooredShipsElement.append(shipContainerElement)
    }
}

async function HandleUnloadButton(shipId)
{
    let options = {
        shipId: shipId,
        seaportId: seaportId
    }
    let optionsJSON = JSON.stringify(options);
    let result = await sendPostRequestWithoutParse(optionsJSON, 'https://localhost:7135/api/cargos/unload');
    if(result === 'OK')
    {
        
    }
    else
    {
        alert('Ошибка при разгрузке корабля');
    }
    await hotUpdateCargosAndMooredShips(false);
}

async function removeCargos()
{
    let options = {
        cargoIds: Array.from(selectedCargoIds),
        seaportId: seaportId
    }
    let optionsJSON = JSON.stringify(options);
    
    let result = await sendPostRequestWithoutParse(optionsJSON, `https://localhost:7135/api/cargos/`);
    if(result === 'OK')
    {
        
    }
    else
    {
        alert('Ошибка при удалении груза');
    }
    hotUpdateCargosAndMooredShips(true);
}

async function removeShip(shipId)
{
    let result = await sendPostRequestWithoutParse('', `https://localhost:7135/api/ships/remove?shipId=${shipId}&seaportId=${seaportId}`);
    if(result === 'OK')
    {
        
    }
    else
    {
        alert('Ошибка при удалении корабля');
    }
    await updateMooredShips();
}

async function createCargo()
{
    let seaport = await sendPostRequest('', `https://localhost:7135/api/seaports/${seaportId}`);
    let options = 
    {
        title: getValueByElementId('title'),
        description: getValueByElementId('description'),
        weight: getValueByElementId('weight'),
        length: getValueByElementId('length'),
        width: getValueByElementId('width'),
        height: getValueByElementId('height'),
        storageId: seaport.storageId
    };
    console.log(options);
    let optionsJSON = JSON.stringify(options);

    let result = await sendPostRequestWithoutParse(optionsJSON,`https://localhost:7135/api/cargos/add`);
    if(result === 'OK')
    {
        
    }
    else
    {
        alert('Ошибка при создании груза');
    }
    await hotUpdateCargosAndMooredShips(false);
}

async function createShip()
{
    let seaport = await sendPostRequest('', `https://localhost:7135/api/seaports/${seaportId}`);
    let options = {
        cargoCompartmentOptions:
        {
            height: getValueByElementId('cargoCompartmentHeight'),
            width: getValueByElementId('cargoCompartmentWidth'),
            length: getValueByElementId('cargoCompartmentLength'),
            loadCapacity: getValueByElementId('cargoCompartmentLoadCapacity')
        },
        name: getValueByElementId('name'),
        maxSpeedInMetersPerSecond: getValueByElementId('maxSpeedInMetersPerSecond'),
        seaportId: seaport.id
    };
    let optionsJSON = JSON.stringify(options);

    let result = await sendPostRequestWithoutParse(optionsJSON, `https://localhost:7135/api/ships/add`);
    if(result === 'OK')
    {
        
    }
    else
    {
        alert('Ошибка при создании корабля');
    }
    await updateMooredShips();
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