// wwwroot/location.js
window.weatherLocation = {
    getCurrentPosition: function () {
        return new Promise((resolve, reject) => {
            navigator.geolocation.getCurrentPosition(
                pos => {
                    console.log("Browser latitude:", pos.coords.latitude);
                    console.log("Browser longitude:", pos.coords.longitude);
                    console.log("Accuracy (meters):", pos.coords.accuracy);

                    resolve({
                        latitude: pos.coords.latitude,
                        longitude: pos.coords.longitude,
                        accuracy: pos.coords.accuracy
                    });
                },
                err => reject(err.message),
                {
                    enableHighAccuracy: true,
                    timeout: 15000,
                    maximumAge: 0
                }
            );
        });
    }
};
