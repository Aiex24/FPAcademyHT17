//Google Maps API-key: AIzaSyA9rdRh5jniAD0TYjDIRhSqQP5ZLf6p5P4
//59.209514, 19.107700, nånstans i bottniska viken.
//utvecklings-nyckel: AIzaSyAUJEHjY43pTfNYN8jxgDiZ23HvslS_YH0
var map;
var lineSymbol;
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
        lineSymbol = { path: google.maps.SymbolPath.FORWARD_CLOSED_ARROW };
        bounds = map.getBounds().toJSON();
        var gridX = 8;
        var gridY = 4;
        var gridCoords = new Array(gridX * gridY);

        for (var i = 0; i < gridX; i++) {
            for (var j = 0; j < gridY; j++) {
                gridCoords[i * gridX + j] = new Array(2);

                gridCoords[i * gridX + j][0] = bounds.west + (1 + i * 2) * ((bounds.east - bounds.west) / (2 * gridX));
                gridCoords[i * gridX + j][1] = bounds.south + (1 + j * 2) * ((bounds.north - bounds.south) / (2 * gridY));

                let gridLat = gridCoords[i * gridX + j][1];
                let gridLon = gridCoords[i * gridX + j][0];

                let windDegree = calcWindSpeed(gridLat, gridLon);
                drawArrow(gridLat, gridLon, windDegree, gridX);
            }
        }
    });

    google.maps.event.addListener(map, 'click', function (event) {
        var lat = event.latLng.lat();
        var lng = event.latLng.lng();

        calcWindSpeed(lat, lng);

        drawLine(lat, lng);
        drawArrow(lat, lng, windDirection);
    });
};

function calcWindSpeed(lat, lng) {

    var jsonLink = "http://api.openweathermap.org/data/2.5/weather?lat=" + lat + "&lon=" + lng + "&APPID=cdc3100be90cc854ac6f417f8bccc78b";
    console.log(jsonLink);
    $.ajax({
        url: jsonLink,
        type: 'GET',
        success: function (result) {
            //TODO - fixa om degree blir undefined
            console.log(result);
            let windSpeed = result.wind.speed;
            let windDegree = result.wind.deg;
            console.log(windDegree);
            return windDegree;


            //return {
            //    speed: windSpeed,
            //    degree: windDegree
            //};

            //alert("Windspeed: " + windSpeed + " Degrees: " + windDegree + " Direction: " + windDirection);
        }
    })
};

function drawArrow(latOrigin, longOrigin, windDegree, mapShareForRadius) {
    let latEnd, longEnd;
    let scopeLat = bounds.north - bounds.south;
    let scopeLong = bounds.east - bounds.west;
    let aspectRatioXY = scopeLat / scopeLong;

    longEnd = longOrigin + ((scopeLat / mapShareForRadius) * Math.sin(windDegree * (Math.PI / 180)));
    latEnd = latOrigin + ((scopeLat / mapShareForRadius)) * Math.cos(windDegree * (Math.PI / 180));

    let line = new google.maps.Polyline({
        path: [{ lat: latOrigin, lng: longOrigin }, { lat: latEnd, lng: longEnd }],
        icons: [{
            icon: lineSymbol,
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
            scale: 4
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


