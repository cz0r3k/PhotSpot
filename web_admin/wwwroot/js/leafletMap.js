function initializeLeafletMap(dotNetReference) {
    var map = L.map('map').setView([52.2297, 21.0122], 13);

    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        maxZoom: 19
    }).addTo(map);

    let marker;

    map.on('click', function (e) {
        if (marker) {
            map.removeLayer(marker);
        }

        marker = L.marker(e.latlng).addTo(map);

        dotNetReference.invokeMethodAsync('SetMapLocation', e.latlng.lat, e.latlng.lng);
    });
}
