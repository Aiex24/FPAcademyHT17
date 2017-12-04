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
            path: [{ lat: lat, lng: lng }, { lat: lat, lng: lng }]
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
    var jsonLink = "http://opendata-download-metanalys.smhi.se/api/category/mesan1g/version/1/geotype/point/lon/" + Math.round(lng) + "/lat/" + Math.round(lat) + "/data.json";

    $.ajax({
        url: jsonLink,
        type: 'GET',
        success: function (result) {
            alert(result.timeSeries[0].parameters[4].values[0] + " m/s");
        }
    })
}

;


