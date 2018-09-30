(function () {
    "use strict";


    window.app.directive('flightSearch', flightSearch);

    function flightSearch() {
        return {
            templateUrl: '/flightSearch/template/flightSearch.tmpl.cshtml',
            controller: FlightSearchController,
            bindToController: true,
            scope: true,
            controllerAs: 'vm'
        }
    }

    window.app.controller('FlightSearchController', FlightSearchController);

    FlightSearchController.$inject = ['$window', '$filter', '$rootScope', '$scope', '$uibModal', 'flightSearchSvc'];

    function FlightSearchController($window, $filter, $rootScope, $scope, $uibModal, flightSearchSvc) {
        var vm = this;
        vm.search = search;
        vm.isLoading = false;
        vm.openFilterModal = openFilterModal;
        vm.closeFilterModal = closeFilterModal;
        vm.openSortModal = openSortModal;
        vm.closeSortModal = closeSortModal;
        vm.resetSort = resetSort;
        vm.resetFilters = resetFilters;
        vm.selectedView = 'advancedSearch';
        vm.setView = setView;

        vm.originLocation = '';
        vm.destinationLocation = '';

        
        function init() {
            if (SearchOccured()) {

                //flightSearchSvc.location({ id: vm.flightSearch.originLocation.placeId }).then(function (response) {
                //    vm.originLocation = response.data.placeName;
                //});

                //flightSearchSvc.location({ id: vm.flightSearch.destinationLocation.placeId }).then(function (response) {
                //    vm.destinationLocation = response.data.placeName;
                //});

                vm.selectedView = 'simpleSearch';
            }
            else {
                vm.selectedView = 'advancedSearch';
            }
        }

        function setView(view) {
            vm.selectedView = view;
        }

        function SearchOccured() {
            var pairs = location.hash.slice(1).split('&');

            return pairs.length > 1;
        }

        resetSearch();
        init();
        function resetSearch() {
            vm.flightSearch = {
                originLocation: { placeId: 'nearest', placeName: 'Nearest Airport' },
                returnFlight: true,
                adults: '1',
                children: '0',
                infants: '0',
                flightClass: 'Economy',
                directFlightsOnly: true,
                outboundDate: new Date(),
                inboundDate: new Date(Date.now() + 1 * 24 * 60 * 60 * 1000)
            };
            resetFilters();
            resetSort();
        }

        function resetFilters() {


        }

        function resetSort() {
            vm.flightSearch.sortType = 'Price';
            vm.flightSearch.sortOrder = 'Asc';
        }

        function openFilterModal() {
            var modalInstance = $uibModal.open({
                templateUrl: '/flightsearch/template/flightSearchFilters.tmpl.cshtml',
                scope: $scope,
                controller: FlightSearchController,
                controllerAs: vm
            });
            modalInstance.rendered.then(function () {
                $rootScope.$broadcast('rzSliderForceRender'); //Force refresh sliders on render. Otherwise bullets are aligned at left side.
            });

        }

        function closeFilterModal() {
            if (SearchOccured()) {
                search();
            }
        }

        function openSortModal() {
            var modalInstance = $uibModal.open({
                templateUrl: '/flightsearch/template/flightSearchSort.tmpl.cshtml',
                scope: $scope,
                controller: FlightSearchController,
                controllerAs: vm
            });
        }

        function closeSortModal() {
            if (SearchOccured()) {
                search();
            }          
        }

        vm.getLocation = function (val) {
            return flightSearchSvc.locationAutoSuggest({ search: val }).then(function (response) {
                return response.data.locations;
            });
        };

        vm.errorMessage = null;

        $rootScope.$on('search', function (event, data) {
            if (SearchOccured())
            {
                search();
            }
        });

        function search() {
            vm.saving = true;

            var now = new Date(),
            exp = new Date(now.getFullYear(), now.getMonth(), now.getDate() + 1);

            var clientSearch = {};

            clientSearch.returnFlight = vm.flightSearch.returnFlight;

            clientSearch.originLocation = vm.flightSearch.originLocation.placeId;
            clientSearch.destinationLocation = vm.flightSearch.destinationLocation.placeId;

            clientSearch.outboundDate = $filter('date')(new Date(vm.flightSearch.outboundDate), 'yyyy-MM-dd');

            if (clientSearch.returnFlight) {
                clientSearch.inboundDate = $filter('date')(new Date(vm.flightSearch.inboundDate), 'yyyy-MM-dd');

            }

            clientSearch.adults = vm.flightSearch.adults;
            clientSearch.children = vm.flightSearch.children;
            clientSearch.infants = vm.flightSearch.infants;

            clientSearch.flightClass = vm.flightSearch.flightClass;

            clientSearch.directFlightsOnly = vm.flightSearch.directFlightsOnly;

            clientSearch.sortType = vm.flightSearch.sortType;
            clientSearch.sortOrder = vm.flightSearch.sortOrder;

            flightSearchSvc.searchRedirect(clientSearch);
            init();
        }
    }
})();