(function () {
    window.app.factory('flightSearchSvc', flightSearchSvc);

    flightSearchSvc.$inject = ['$window', '$httpParamSerializer', '$rootScope', '$http'];
    function flightSearchSvc($window, $httpParamSerializer, $rootScope, $http) {

        var svc = {
            search: search,
            searchRedirect: searchRedirect,
            locationAutoSuggest: locationAutoSuggest,
            location: location
        };

        return svc;

        function locationAutoSuggest(request) {

            request.locale = 'en-GB';
            request.market = ['AU'];
            request.currency = 'GBP';

            return $http.post('/api/flight-search-location/auto-suggest', request);
        }

        function location(request) {

            request.locale = 'en-GB';
            request.market = ['AU'];
            request.currency = 'GBP';

            return $http.post('/api/flight-search-location', request);
        }

        function search(request) {

            request.locale = 'en-GB';
            request.market = ['AU'];
            request.currency = 'GBP';
          
            return $http.post('/api/flight-search', request);
        }

        function searchRedirect(request) {

            request.locale = 'en-GB';
            request.market = ['AU'];
            request.currency = 'GBP';
           
            request.skip = 0;
            request.take = 10;

            var qs = $httpParamSerializer(request);

            var url = "http://" + $window.location.host + "/flight-search-results#" + qs;
            window.location.href = url;

            //window.location.href = "http://www.google.com";

            //return $http.post('/api/v1.0/flightsearch/search', request);
        }

        function currentLocation() {
            var options = {
                enableHighAccuracy: true
            };

            navigator.geolocation.getCurrentPosition(function (pos) {
                var position = new google.maps.LatLng(pos.coords.latitude, pos.coords.longitude);
                return position;
            },
                    function (error) {
                        console.log('Unable to get location: ' + error.message);
                    }, options);
        }
    }
})();