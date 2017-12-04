//Google Maps API-key: AIzaSyA9rdRh5jniAD0TYjDIRhSqQP5ZLf6p5P4
//59.209514, 19.107700, nånstans i bottniska viken.
var map;
var lineSymbol = { path: google.maps.SymbolPath.FORWARD_CLOSED_ARROW }
var bounds;


function initiateMap() {
    map = new google.maps.Map(document.getElementById('Map'), {
        center: { lat: 59.2, lng: 19.1 },
        zoom: 9,
        gestureHandling: 'none',
        zoomControl: false,
        streetViewControl: false
    });
    google.maps.event.addListener(map, 'click', function (event) {
        var lat = event.latLng.lat();
        var lng = event.latLng.lng();
        var line = new google.maps.Polyline({
            path: [{ lat: lat, lng: lng }, { lat: lat, lng: lng }],
            icons: [{
                icon: lineSymbol,
                offset: '100%'
            }],
            map: map
        });

        calcWindSpeed(lat, lng);
    });
    google.maps.event.addListener(map, 'tilesloaded', function (event) {
        bounds = map.getBounds().toJSON();       
    });

};

function calcWindSpeed(lat, lng) {
    //SMHI
    //var jsonLink = "http://opendata-download-metanalys.smhi.se/api/category/mesan1g/version/1/geotype/point/lon/" + Math.round(lng) + "/lat/" + Math.round(lat) + "/data.json";
    //Openweathermap
    var jsonLink = "http://api.openweathermap.org/data/2.5/weather?lat=" + lat + "&lon=" + lng + "&APPID=cdc3100be90cc854ac6f417f8bccc78b";
    console.log(jsonLink);
    $.ajax({
        url: jsonLink,
        type: 'GET',
        success: function (result) {
            let windSpeed = result.wind.speed;
            let windDegree = result.wind.deg;
            let windDirection;
            if (windDegree <= 45) {windDirection = 1;}
            else if (windDegree > 45 && windDegree <= 90) { windDirection = 2; }
            else if (windDegree > 90 && windDegree <= 135) { windDirection = 3; }
            else if (windDegree > 135 && windDegree <= 180) { windDirection = 4; }
            else if (windDegree > 180 && windDegree <= 225) { windDirection = 5; }
            else if (windDegree > 225 && windDegree <= 270) { windDirection = 6; }
            else if (windDegree > 270 && windDegree <= 315) { windDirection = 7; }
            else if (windDegree > 315 && windDegree <= 360) { windDirection = 8; }

            alert("Windspeed: " + windSpeed + " Degrees: " + windDegree + " Direction: " + windDirection);
        }
    })
}

;


