

function getMarker(id) {
    for (var i = 0; i < markers.length; i++) {
        if (markers[i].getTitle().toString() == id.toString()) {
            return markers[i];
        }
    }
    return null;
}

