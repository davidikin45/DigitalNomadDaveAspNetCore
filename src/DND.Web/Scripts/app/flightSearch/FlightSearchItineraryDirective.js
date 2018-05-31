(function () {
    "use strict";

    window.app.directive('flightSearchItinerary', flightSearchItinerary);

    function flightSearchItinerary() {
        return {
            scope: {
                itinerary: '='
            },
            replace: true,
            templateUrl: '/flightSearch/template/flightSearchItinerary.tmpl.cshtml',
            controller: controller,
            controllerAs: 'vm'
        }
    }

    controller.$inject = ['$scope', '$modal', 'flightSearchSvc'];
    function controller($scope, $modal,flightSearchSvc) {
        var vm = this;
  
        vm.itinerary = $scope.itinerary;
        vm.selectedView = 'details';
        vm.setView = setView;
        vm.map = map;

        function setView(view) {
            vm.selectedView = view;
        }

        function map() {
            $modal.open({
                template: '<itinerary-map itinerary="itinerary" />',
                scope: angular.extend($scope.$new(true), { itinerary: vm.itinerary })
            });
        }


       
    }
})();