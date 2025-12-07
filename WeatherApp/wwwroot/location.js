// wwwroot/location.js
window.weatherLocation = {
    getCurrentPosition: function () {
        return new Promise(function (resolve, reject) {
            if (!navigator.geolocation) {
                reject("Geolocation is not supported by this browser.");
            } else {
                navigator.geolocation.getCurrentPosition(
                    function (pos) {
                        resolve({
                            latitude: pos.coords.latitude,
                            longitude: pos.coords.longitude
                        });
                    },
                    function (err) {
                        reject(err.message || "Unable to get location");
                    });
            }
        });
    }
};
