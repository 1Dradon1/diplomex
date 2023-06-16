renderShippings();

async function renderShippings()
{
    let shippings = await sendPostRequest('', `https://localhost:7135/api/shippings/`);

    console.log(shippings[0]);
    let parentElement = document.getElementById('shippingsContent');
    parentElement.innerHTML = '';
    for(let i = 0; i < shippings.length; i++)
    {
        let shipping = shippings[i];
        renderShipping(parentElement, shipping);
    }
}

async function renderShipping(parentElement, shipping)
{
    console.log(shipping);
    let dateHolder = document.createElement('p');
    dateHolder.innerHTML = `Дата и время отбытия: ${shipping.departureTime}`;

    let idHolder = document.createElement('p');
    idHolder.innerHTML = `Id: ${shipping.id}`;

    let fromSeaportHolder = document.createElement('p');
    let fromSeaport = shipping.fromSeaport;
    let fromLatitude = fromSeaport.positionX;
    let fromLongitude = fromSeaport.positionY;
    let fromFormattedLatitude = getFormattedLocationString(fromLatitude);
    let fromFormattedLongitude = getFormattedLocationString(fromLongitude);
    fromSeaportHolder.innerHTML = `Из морского порта "${fromSeaport.name}.${fromSeaport.id}" расположенного по координатам: (широта: ${fromFormattedLatitude} долгота:${fromFormattedLongitude})`;

    let toSeaportHolder = document.createElement('p');
    let toSeaport = shipping.toSeaport;
    let toLatitude = toSeaport.positionX;
    let toLongitude = toSeaport.positionY;
    let toFormattedLatitude = getFormattedLocationString(toLatitude);
    let toFormattedLongitude = getFormattedLocationString(toLongitude);
    toSeaportHolder.innerHTML = `В морской порт "${toSeaport.name}.${toSeaport.id}" расположенный по координатам: (широта: ${toFormattedLatitude} долгота:${toFormattedLongitude})`;

    
    let shipHolder = document.createElement('p');
    let ship = shipping.ship;
    shipHolder.innerHTML = `Название: "${ship.name}" Id: ${ship.id} Скорость: ${ship.maxSpeedInMetersPerSecond}(м/с)`;
    let container = document.createElement('div');
    container.append(idHolder, dateHolder, fromSeaportHolder, toSeaportHolder, shipHolder);
    parentElement.append(container);

}