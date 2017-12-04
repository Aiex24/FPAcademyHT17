//Google Maps API-key: AIzaSyA9rdRh5jniAD0TYjDIRhSqQP5ZLf6p5P4
//59.209514, 19.107700, nånstans i bottniska viken.
var map;

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
        
        calcWindSpeed(lat, lng);
    });
    //Rita pilar när kartan är laddad
    google.maps.event.addListener(map, 'tilesloaded', function (event) {
        bounds = map.getBounds().toJSON();     
        var lineSymbol = { path: google.maps.SymbolPath.FORWARD_CLOSED_ARROW };
        var line = new google.maps.Polyline({
            path: [{ lat: (bounds.north - ((bounds.north - bounds.south) / 2)), lng: bounds.east }, { lat: bounds.north, lng: bounds.west }],
            icons: [{
                icon: lineSymbol,
                offset: '100%'
            }],
            map: map
        });

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
            if ((windDegree > 337.5 && windDegree <= 360) && (windDegree => 0 && windDegree <= 22.5)) { windDirection = 'N'; }
            else if (windDegree > 22.5 && windDegree <= 67.5) { windDirection = 'NE'; }
            else if (windDegree > 67.5 && windDegree <= 112.5) { windDirection = 'E'; }
            else if (windDegree > 112.5 && windDegree <= 157.5) { windDirection = 'SE'; }
            else if (windDegree > 157.5 && windDegree <= 202.5) { windDirection = 'S'; }
            else if (windDegree > 202.5 && windDegree <= 247.5) { windDirection = 'SW'; }
            else if (windDegree > 247.5 && windDegree <= 292.5) { windDirection = 'W'; }
            else if (windDegree > 292.5 && windDegree <= 337.5) { windDirection = 'NW'; }

            alert("Windspeed: " + windSpeed + " Degrees: " + windDegree + " Direction: " + windDirection);
        }
    })
}

;


