//Google Maps API-key: AIzaSyA9rdRh5jniAD0TYjDIRhSqQP5ZLf6p5P4
//59.209514, 19.107700, nånstans i bottniska viken.
var map;

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
    google.maps.event.addListener(map, 'tilesloaded', function (event) {
        let boundsstring = map.getBounds().isEmpty();
        alert(boundsstring);
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