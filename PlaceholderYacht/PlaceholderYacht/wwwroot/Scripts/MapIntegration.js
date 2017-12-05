//Google Maps API-key: AIzaSyA9rdRh5jniAD0TYjDIRhSqQP5ZLf6p5P4
//59.209514, 19.107700, nånstans i bottniska viken.
//utvecklings-nyckel: AIzaSyAUJEHjY43pTfNYN8jxgDiZ23HvslS_YH0
var map;
var bounds;
var clickCounter = 0;
var clickStart = [];
var clickEnd = [];

function initiateMap() {
    map = new google.maps.Map(document.getElementById('Map'), {
        center: { lat: 59.2, lng: 19.1 },
        zoom: 9,
        gestureHandling: 'none',
        zoomControl: false,
        streetViewControl: false
    })
    //Rita pilar när kartan är laddad
    google.maps.event.addListener(map, 'tilesloaded', function (event) {
        bounds = map.getBounds().toJSON();
        var gridY = 4;
        var gridX = 8;
        var gridCoords = new Array(gridX * gridY);

        for (var i = 0; i < gridX; i++) {
            for (var j = 0; j < gridY; j++) {
                gridCoords[i * gridX + j] = new Array(2);

                gridCoords[i * gridX + j][0] = bounds.west + (1 + i * 2) * ((bounds.east - bounds.west) / (2 * gridX));
                gridCoords[i * gridX + j][1] = bounds.south + (1 + j * 2) * ((bounds.north - bounds.south) / (2 * gridY));

                let gridLat = gridCoords[i * gridX + j][1];
                let gridLon = gridCoords[i * gridX + j][0];

                let windDegree = calcWindSpeed(gridLat, gridLon);
            }
        }
    });

    google.maps.event.addListener(map, 'click', function (event) {
        var lat = event.latLng.lat();
        var lng = event.latLng.lng();

        drawLine(lat, lng);
    });
};

function calcWindSpeed(lat, lng) {
    var latRound = Math.round(lat * 1000000) / 1000000;
    var lngRound = Math.round(lng * 1000000) / 1000000;
    console.log(latRound);
    console.log(lngRound);

    var jsonLinkSMHI = "http://opendata-download-metanalys.smhi.se/api/category/mesan1g/version/1/geotype/point/lon/" + lngRound + "/lat/" + latRound + "/data.json";
    console.log(jsonLinkSMHI);
    //var jsonLinkOpenWeather = "http://api.openweathermap.org/data/2.5/weather?lat=" + lat + "&lon=" + lng + "&APPID=cdc3100be90cc854ac6f417f8bccc78b";
    var windSpeed;
    var windDegree;
    $.ajax({
        url: jsonLinkSMHI,
        type: 'GET',
        success: function (result) {
            console.log(result);
            for (var i = 0; i < 10; i++) {
                windSpeed = result.timeSeries[i].parameters[4].values[0];
                windDegree = result.timeSeries[i].parameters[3].values[0];
                await sleep(2000);
                console.log(windSpeed, windDegree);
            }

            drawArrow(lat, lng, windDegree - 180, 15);

            ////Open Weather
            //windSpeed = result.wind.speed;
            //windDegree = result.wind.deg;
            //alert("Windspeed: " + windSpeed + " Degrees: " + windDegree + " Direction: " + windDirection);
        }
    });
};

function drawArrow(latOrigin, longOrigin, windDegree, mapShareForRadius) {
    let latEnd, longEnd;
    let scopeLat = bounds.north - bounds.south;
    let scopeLong = bounds.east - bounds.west;
    let aspectRatioXY = scopeLat / scopeLong;

    longEnd = longOrigin + ((scopeLat / mapShareForRadius) * Math.sin(windDegree * (Math.PI / 180)));
    latEnd = latOrigin + ((scopeLat / mapShareForRadius)) * Math.cos(windDegree * (Math.PI / 180));

    let arrowSymbol = {
        path: google.maps.SymbolPath.FORWARD_CLOSED_ARROW,
        strokeOpacity: 0.5,
        scale: 2
    };
    let line = new google.maps.Polyline({
        path: [{ lat: latOrigin, lng: longOrigin }, { lat: latEnd, lng: longEnd }],
        icons: [{
            icon: arrowSymbol,
            offset: '100%'
        }],
        map: map
    });
}

function drawLine(lat, lng) {

    if (clickCounter % 2 == 0) {
        clickStart[0] = lat;
        clickStart[1] = lng;
    }
    else if (clickCounter % 2 == 1) {
        clickEnd[0] = lat;
        clickEnd[1] = lng;
    }
    clickCounter++;

    if (clickStart.length > 0 && clickEnd.length > 0) {
        var lineSymbol = {
            path: 'M 0,-1 0,1',
            strokeOpacity: 1,
            scale: 2
        };
        var line = new google.maps.Polyline({
            path: [{ lat: clickStart[0], lng: clickStart[1] }, { lat: clickEnd[0], lng: clickEnd[1] }],
            strokeOpacity: 0,
            icons: [{
                icon: lineSymbol,
                offset: '0',
                repeat: '20px'
            }],
            map: map
        });
    }
}


