//Google Maps API-key: AIzaSyA9rdRh5jniAD0TYjDIRhSqQP5ZLf6p5P4
//59.209514, 19.107700, nånstans i bottniska viken.
//utvecklings-nyckel: AIzaSyAUJEHjY43pTfNYN8jxgDiZ23HvslS_YH0
var map, bounds, infoWindow;
var markers = []; // array för att hålla koll på kartmarkörer
var arrows = []; // en array för att hålla koll på våra pilar
var clickCounter = 0;
var clickStart = [];
var clickEnd = [];
var time = 0;
var hr = (new Date()).getHours();
var isDayTime = hr > 8 && hr < 15;


function CenterControl(controlDiv, map) {

    // Set CSS for the control border.
    var controlUI = document.createElement('div');
    controlUI.style.backgroundColor = '#fff';
    controlUI.style.border = '2px solid #fff';
    controlUI.style.borderRadius = '3px';
    controlUI.style.boxShadow = '0 2px 6px rgba(0,0,0,.3)';
    controlUI.style.cursor = 'pointer';
    controlUI.style.marginBottom = '22px';
    controlUI.style.textAlign = 'center';
    controlUI.title = 'Get most recent wind data';
    controlDiv.appendChild(controlUI);

    // Set CSS for the control interior.
    var controlText = document.createElement('div');
    controlText.style.color = 'rgb(25,25,25)';
    controlText.style.fontFamily = 'Roboto,Arial,sans-serif';
    controlText.style.fontSize = '16px';
    controlText.style.lineHeight = '38px';
    controlText.style.paddingLeft = '5px';
    controlText.style.paddingRight = '5px';
    controlText.innerHTML = 'Most recent';
    controlUI.appendChild(controlText);


    // Set CSS for the control border.
    var controlUI8 = document.createElement('div');
    controlUI8.style.backgroundColor = '#fff';
    controlUI8.style.border = '2px solid #fff';
    controlUI8.style.borderRadius = '3px';
    controlUI8.style.boxShadow = '0 2px 6px rgba(0,0,0,.3)';
    controlUI8.style.cursor = 'pointer';
    controlUI8.style.marginBottom = '22px';
    controlUI8.style.textAlign = 'center';
    controlUI8.title = 'Wind data (8h)';
    controlDiv.appendChild(controlUI8);

    // Set CSS for the control interior.
    var controlText8 = document.createElement('div');
    controlText8.style.color = 'rgb(25,25,25)';
    controlText8.style.fontFamily = 'Roboto,Arial,sans-serif';
    controlText8.style.fontSize = '16px';
    controlText8.style.lineHeight = '38px';
    controlText8.style.paddingLeft = '5px';
    controlText8.style.paddingRight = '5px';
    controlText8.innerHTML = '8 hours';
    controlUI8.appendChild(controlText8);

    // Setup the click event listeners: simply set the map to Chicago.
    controlUI.addEventListener('click', function () {
        time = 0;
        initiateMap();
    });

    // Setup the click event listeners: simply set the map to Chicago.
    controlUI8.addEventListener('click', function () {
        time = 8;
        initiateMap();
    });


}

function initiateMap() {
    // Some map styles
    let darkMapType = new google.maps.StyledMapType(
        [
            {
                "elementType": "geometry",
                "stylers": [
                    {
                        "color": "#212121"
                    }
                ]
            },
            {
                "elementType": "labels.icon",
                "stylers": [
                    {
                        "visibility": "off"
                    }
                ]
            },
            {
                "elementType": "labels.text.fill",
                "stylers": [
                    {
                        "color": "#757575"
                    }
                ]
            },
            {
                "elementType": "labels.text.stroke",
                "stylers": [
                    {
                        "color": "#212121"
                    }
                ]
            },
            {
                "featureType": "administrative",
                "elementType": "geometry",
                "stylers": [
                    {
                        "color": "#757575"
                    }
                ]
            },
            {
                "featureType": "administrative.country",
                "elementType": "labels.text.fill",
                "stylers": [
                    {
                        "color": "#9e9e9e"
                    }
                ]
            },
            {
                "featureType": "administrative.land_parcel",
                "stylers": [
                    {
                        "visibility": "off"
                    }
                ]
            },
            {
                "featureType": "administrative.locality",
                "elementType": "labels.text.fill",
                "stylers": [
                    {
                        "color": "#bdbdbd"
                    }
                ]
            },
            {
                "featureType": "poi",
                "elementType": "labels.text.fill",
                "stylers": [
                    {
                        "color": "#757575"
                    }
                ]
            },
            {
                "featureType": "poi.park",
                "elementType": "geometry",
                "stylers": [
                    {
                        "color": "#181818"
                    }
                ]
            },
            {
                "featureType": "poi.park",
                "elementType": "labels.text.fill",
                "stylers": [
                    {
                        "color": "#616161"
                    }
                ]
            },
            {
                "featureType": "poi.park",
                "elementType": "labels.text.stroke",
                "stylers": [
                    {
                        "color": "#1b1b1b"
                    }
                ]
            },
            {
                "featureType": "road",
                "elementType": "geometry.fill",
                "stylers": [
                    {
                        "color": "#2c2c2c"
                    }
                ]
            },
            {
                "featureType": "road",
                "elementType": "labels.text.fill",
                "stylers": [
                    {
                        "color": "#8a8a8a"
                    }
                ]
            },
            {
                "featureType": "road.arterial",
                "elementType": "geometry",
                "stylers": [
                    {
                        "color": "#373737"
                    }
                ]
            },
            {
                "featureType": "road.highway",
                "elementType": "geometry",
                "stylers": [
                    {
                        "color": "#3c3c3c"
                    }
                ]
            },
            {
                "featureType": "road.highway.controlled_access",
                "elementType": "geometry",
                "stylers": [
                    {
                        "color": "#4e4e4e"
                    }
                ]
            },
            {
                "featureType": "road.local",
                "elementType": "labels.text.fill",
                "stylers": [
                    {
                        "color": "#616161"
                    }
                ]
            },
            {
                "featureType": "transit",
                "elementType": "labels.text.fill",
                "stylers": [
                    {
                        "color": "#757575"
                    }
                ]
            },
            {
                "featureType": "water",
                "elementType": "geometry",
                "stylers": [
                    {
                        "color": "#000000"
                    }
                ]
            },
            {
                "featureType": "water",
                "elementType": "labels.text.fill",
                "stylers": [
                    {
                        "color": "#3d3d3d"
                    }
                ]
            }
        ],
        { name: 'Dark map' });

    map = new google.maps.Map(document.getElementById('Map'), {
        center: { lat: 59.2, lng: 19.1 },
        zoom: 9,
        //gestureHandling: 'none',
        zoomControl: false,
        streetViewControl: false,
        mapTypeControl: false,
        //disableDefaultUI: true,
        mapTypeControlOptions: {
            mapTypeIds: ['roadmap',
                'satellite',
                'hybrid',
                'terrain',
                'Dark map']
        }
    });

    // Create the DIV to hold the control and call the CenterControl()
    // constructor passing in this DIV.
    var centerControlDiv = document.createElement('div');
    var centerControl = new CenterControl(centerControlDiv, map);
    centerControlDiv.index = 1;
    map.controls[google.maps.ControlPosition.LEFT_CENTER].push(centerControlDiv);

    // Sets mapstyle depending on night/day - numera trasig :/
    if (!isDayTime) {
        //Associate the styled map with the MapTypeId and set it to display.
        map.mapTypes.set('Dark map', darkMapType);
        map.setMapTypeId('Dark map');

    }

    // skapa ny inforuta
    infoWindow = new google.maps.InfoWindow;

    // Hitta användarens position (kräver att användaren godkänner geolokalisering)
    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(function (position) {
            // sparar positionen i en variabel
            var pos = {
                lat: position.coords.latitude,
                lng: position.coords.longitude
            };

            // sätter ut en markör på användarens position
            infoWindow.setPosition(pos);
            infoWindow.setContent('This is your position');
            infoWindow.open(map);

            // centrerar kartan på denna position
            map.setCenter(pos);
        }, function () {

            // anropar metoden som hanterar geolokaliseringfel
            handleLocationError(true, infoWindow, map.getCenter());
        });
    } else {
        // Om servern ej kan hantera geolokalisering
        handleLocationError(false, infoWindow, map.getCenter());
    }

    //Rita pilar när kartan är laddad
    google.maps.event.addListener(map, 'tilesloaded', function (event) {
        bounds = map.getBounds().toJSON();
        var gridY = 12;
        var gridX = 12;
        var gridCoords = new Array(gridX * gridY);

        for (var i = 0; i < gridX; i++) {
            for (var j = 0; j < gridY; j++) {
                gridCoords[i * gridX + j] = new Array(2);

                gridCoords[i * gridX + j][0] = bounds.west + (1 + i * 2) * ((bounds.east - bounds.west) / (2 * gridX));
                gridCoords[i * gridX + j][1] = bounds.south + (1 + j * 2) * ((bounds.north - bounds.south) / (2 * gridY));

                let gridLat = gridCoords[i * gridX + j][1];
                let gridLon = gridCoords[i * gridX + j][0];

                calcWindSpeed(gridLat, gridLon);
            }
        }
    });

    // När användaren zoomar in/ut på kartan så tar vi bort de gamla pilarna
    google.maps.event.addListener(map, 'zoom_changed', function (event) {
        deleteArrows();
    });

    // När användaren släppt kartan efter att ha dragit så tar vi bort de gamla pilarna 
    google.maps.event.addListener(map, 'dragend', function (event) {
        deleteArrows();
    });

    // Eventhandler for map clicks
    google.maps.event.addListener(map, 'click', function (event) {
        let lat = event.latLng.lat();
        let lng = event.latLng.lng();

        //Registrera klick för att rita ut linje mellan två punkter
        if (clickCounter % 2 === 0) {
            clickStart[0] = lat;
            clickStart[1] = lng;
        }
        else if (clickCounter % 2 === 1) {
            clickEnd[0] = lat;
            clickEnd[1] = lng;
        }
        clickCounter++;

        //drawLine(clickStart[0], clickStart[1], clickEnd[0], clickEnd[1]);
        calcWindSpeedForPoint(lat, lng);
    });

};

// hämta ut data från valfri tjänst (läs: smhi/OWM)
function calcWindSpeed(lat, lng) {
    let latRound = Math.round(lat * 1000000) / 1000000;
    let lngRound = Math.round(lng * 1000000) / 1000000;
    console.log(latRound);
    console.log(lngRound);

    let jsonLinkSMHI = "http://opendata-download-metanalys.smhi.se/api/category/mesan1g/version/1/geotype/point/lon/" + lngRound + "/lat/" + latRound + "/data.json";
    let jsonLinkOpenWeather = "http://api.openweathermap.org/data/2.5/weather?lat=" + lat + "&lon=" + lng + "&APPID=cdc3100be90cc854ac6f417f8bccc78b";
    $.ajax({
        url: jsonLinkSMHI,
        type: 'GET',
        success: function (result) {
            console.log(result);
            var windSpeed = result.timeSeries[time].parameters[4].values[0];
            var windDegree = result.timeSeries[time].parameters[3].values[0];
            drawArrow(lat, lng, windDegree - 180, 15, windSpeed);


            ////Open Weather
            //windSpeed = result.wind.speed;
            //windDegree = result.wind.deg;
        }
    });
};

// eget anrop för att få info om en punkt
function calcWindSpeedForPoint(lat, lng) {
    let latRound = Math.round(lat * 1000000) / 1000000;
    let lngRound = Math.round(lng * 1000000) / 1000000;
    console.log(latRound);
    console.log(lngRound);
    let windSpeed;
    let windDegree;

    let jsonLinkSMHI = "http://opendata-download-metanalys.smhi.se/api/category/mesan1g/version/1/geotype/point/lon/" + lngRound + "/lat/" + latRound + "/data.json";
    let jsonLinkOpenWeather = "http://api.openweathermap.org/data/2.5/weather?lat=" + lat + "&lon=" + lng + "&APPID=cdc3100be90cc854ac6f417f8bccc78b";
    $.ajax({
        url: jsonLinkSMHI,
        type: 'GET',
        success: function (result) {
            console.log(result);
            windSpeed = result.timeSeries[time].parameters[4].values[0];
            windDegree = result.timeSeries[time].parameters[3].values[0];


            DisplayWindData(latRound, lngRound, windSpeed, windDegree);

            ////Open Weather
            //windSpeed = result.wind.speed;
            //windDegree = result.wind.deg;
        }
    });
};

// Draw supercool arrows
function drawArrow(latOrigin, longOrigin, windDegree, mapShareForRadius, windSpeed) {

    // some variables
    let latEnd, longEnd;
    let scopeLat = bounds.north - bounds.south;
    let scopeLong = bounds.east - bounds.west;
    let aspectRatioXY = scopeLat / scopeLong;
    let arrowColor = 'blue';

    longEnd = longOrigin + ((scopeLat / mapShareForRadius) * Math.sin(windDegree * (Math.PI / 180)));
    latEnd = latOrigin + ((scopeLat / mapShareForRadius)) * Math.cos(windDegree * (Math.PI / 180));

    // Arrow coloring based on windspeed
    if (windSpeed > 1 && windSpeed <= 5)
        arrowColor = 'green';
    else if (5 < windSpeed <= 10)
        arrowColor = 'yellow';
    else if (windSpeed > 10)
        arrowColor = 'red';

    // properties for our wind arrows
    let arrowSymbol = {
        path: google.maps.SymbolPath.FORWARD_OPEN_ARROW,
        strokeOpacity: 1,
        strokeColor: arrowColor,
        scale: 2
    };

    // properties for the arrow paths
    let line = new google.maps.Polyline({
        path: [{ lat: latOrigin, lng: longOrigin }, { lat: latEnd, lng: longEnd }],
        strokeOpacity: 0.1,
        strokeColor: arrowColor,
        scale: 1,
        icons: [{
            icon: arrowSymbol,
            offset: '100%'
        }],
        map: map
    });
    arrows.push(line); // lägg till pilar i arrayen
    animateCircle(line);
    
}

// Drawing lines 
function drawLine(latOrg, lngOrg, latEnd, lngEnd) {


    var startLatLng = { lat: latOrg, lng: lngOrg };
    var endLatLng = { lat: latEnd, lng: lngEnd };

    // 
    let lineSymbol = {
        path: google.maps.SymbolPath.CIRCLE,
        strokeOpacity: 1,
        scale: 2
    };


    let line = new google.maps.Polyline({
        path: [{ lat: latOrg, lng: lngOrg }, { lat: latEnd, lng: lngEnd }],
        strokeOpacity: 0,
        icons: [{
            icon: lineSymbol,
            offset: '0',
            repeat: '20px'
        }],
        map: map
    });

    var startMarker = new google.maps.Marker({
        position: startLatLng,
        map: map,
        title: 'Starting point'
    });

    var endMarker = new google.maps.Marker({
        position: endLatLng,
        map: map,
        title: 'Destination'
    });
}

// function for movig symbols along lines
function animateCircle(line) {
    var count = 0;
    window.setInterval(function () {
        count = (count + 1) % 100;

        var icons = line.get('icons');
        icons[0].offset = (count / 0.5) + '%';
        line.set('icons', icons);
    }, 20);
}

function DisplayWindData(lat, lng, windSpeed, windDegree) {

    // datan vi vill visa i infoboxen
    let contentString =
        '<p><strong>Wind speed:</strong> ' + windSpeed + '</p>' +
        '<p><strong>Wind degree:</strong>' + windDegree + '<p>';

    // position
    let latLng = { lat: lat, lng: lng };
    // brassa in en skön inforuta
    let info = new google.maps.InfoWindow({
        content: contentString
    });

    // tillfällig marker
    let marker = new google.maps.Marker({
        position: latLng,
        map: map,
        title: 'Vinddata för position:' + lat + ',' + lng
    });

    if (markers.length < 1)
        markers.push(marker);
    else {
        deleteMarkers();
        markers.push(marker);
    }

    // lägg till ett klick-event på vår markör
    marker.addListener('click', function () {
        info.open(map, marker);
    });
}

// Sets the map on all markers in the array.
function setMapOnAll(map) {
    for (var i = 0; i < markers.length; i++) {
        markers[i].setMap(map);
    }
}

// Removes the markers from the map, but keeps them in the array.
function clearMarkers() {
    setMapOnAll(null);
}

// Shows any markers currently in the array.
function showMarkers() {
    setMapOnAll(map);
}

// Deletes all markers in the array by removing references to them.
function deleteMarkers() {
    clearMarkers();
    markers = [];
}



// Sets the map on all markers in the array.
function setMapOnAllArrows(map) {
    for (var i = 0; i < arrows.length; i++) {
        arrows[i].setMap(map);
    }
}

// Removes the markers from the map, but keeps them in the array.
function clearArrows() {
    setMapOnAllArrows(null);
}

// Shows any markers currently in the array.
function showArrows() {
    setMapOnAllArrows(map);
}

// Deletes all markers in the array by removing references to them.
function deleteArrows() {
    clearArrows();
    arrows = [];
}

// Hanterar fel vid geolokalisering
function handleLocationError(browserHasGeolocation, infoWindow, pos) {
    infoWindow.setPosition(pos);
    infoWindow.setContent(browserHasGeolocation ?
        'Error: The Geolocation service failed.' :
        'Error: Your browser doesn\'t support geolocation.');
    infoWindow.open(map);
}