//Google Maps API-key: AIzaSyA9rdRh5jniAD0TYjDIRhSqQP5ZLf6p5P4
//59.209514, 19.107700, nånstans i bottniska viken.
//utvecklings-nyckel: AIzaSyAUJEHjY43pTfNYN8jxgDiZ23HvslS_YH0
var map;
var bounds;
var clickCounter = 0;
var clickStart = [];
var clickEnd = [];
const hr = (new Date()).getHours();
const isDayTime = hours > 8 && hours < 15;

function initiateMap() {
    // Create a new StyledMapType object, passing it an array of styles,
    // and the name to be displayed on the map type control.

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
        gestureHandling: 'none',
        zoomControl: false,
        streetViewControl: false,
        mapTypeControlOptions: {
            mapTypeIds: ['roadmap',
                'satellite',
                'hybrid',
                'terrain',
                'Dark map']
        }
    });

    // Sets mapstyle depending on night/day 
    if (!isDayTime)
    {
       //Associate the styled map with the MapTypeId and set it to display.
       map.mapTypes.set('Dark map', darkMapType);
       map.setMapTypeId('Dark map');
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

    // Eventhandler for map clicks
    google.maps.event.addListener(map, 'click', function (event) {
        let lat = event.latLng.lat();
        let lng = event.latLng.lng();

        //Registrera klick för att rita ut linje mellan två punkter
        if (clickCounter % 2 == 0) {
            clickStart[0] = lat;
            clickStart[1] = lng;
        }
        else if (clickCounter % 2 == 1) {
            clickEnd[0] = lat;
            clickEnd[1] = lng;
        }
        clickCounter++;

        drawLine(clickStart[0], clickStart[1], clickEnd[0], clickEnd[1]);
    });
};

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
            var windSpeed = result.timeSeries[0].parameters[4].values[0];
            var windDegree = result.timeSeries[0].parameters[3].values[0];
            drawArrow(lat, lng, windDegree - 180, 15, windSpeed);
            

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
    let arrowColor;

    longEnd = longOrigin + ((scopeLat / mapShareForRadius) * Math.sin(windDegree * (Math.PI / 180)));
    latEnd = latOrigin + ((scopeLat / mapShareForRadius)) * Math.cos(windDegree * (Math.PI / 180));

    // Arrow coloring based on windspeed
    if (windSpeed < 5)
        arrowColor = 'green';
    else if (5 < windSpeed < 10)
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


