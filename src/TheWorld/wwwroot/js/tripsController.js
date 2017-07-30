(function () {
    "use strict";
    angular.module("app-trips")
    .controller("tripsController", tripsController);

    function tripsController($http) {
        var vm = this;
        vm.trips = [];

        vm.newTrip = {};

        vm.errorMessage = "";
        vm.isBusy = true;

        $http.get("/api/trips")
            .then(function (response) {
                //Success
                //to debug angular code : console.log(response.data);
                angular.copy(response.data, vm.trips);
            }, function (error) {
                // Failure
                vm.errorMessage = "Failed to load the data: " + error;
            })
            .finally(function () {
                vm.isBusy = false;
            });

        vm.addTrip = function () {
            // how to add items to collections
            //vm.trips.push({ name: vm.newTrip.name, created: new Date() });
            //vm.newTrip = {};
            $http.post("/api/trips", vm.newTrip)
            .then(function (response) {
                //Success
                // Add the new created trip to the collection to show th client
                // the data return here is the created response of our service
                vm.trips.push(response.data);
                vm.newTrip = {};
                
            }, function () {
                // Failure
                vm.errorMessage = "Failed to save new trip ";
            })
            .finally(function () {
                vm.isBusy = false;
            });

        };
    }
})();