L.GeoIP = L.extend({

    getPosition: function (ip) {
        console.log('Inside' + ip);
        var url = "http://api.ipstack.com/";
        var result = new Array();
        var ofRecord = L.latLng(0, 0);

        if (ip !== undefined) {
            url = url + ip;
        } else {
            //lookup our own ip address
        }
        url = url + "?access_key=e1300561ae6abf72371ba4b79d867450";
        console.log(url);
        var xhr = new XMLHttpRequest();
        xhr.open("GET", url, false);
        xhr.onload = function () {
            var status = xhr.status;
            if (status === 200) {
                var geoip_response = JSON.parse(xhr.responseText);
                console.log(geoip_response);
                for (var i = 0; i < geoip_response.length; i++) {
                    ofRecord.lat = geoip_response.latitude;
                    ofRecord.lng = geoip_response.longitude;
                    result.push(ofRecord);
                }
                
            } else {
                console.log("Leaflet.GeoIP.getPosition failed because its XMLHttpRequest got this response: " + xhr.status);
            }
        };
        xhr.send();
        return result;
    },

    centerMapOnPosition: function (map, zoom, ip) {
        var position = L.GeoIP.getPosition(ip);
        map.setView(position, zoom);
    }
});