﻿app.controller('HotelController', ['$http', '$scope', '$rootScope', function ($http, $scope, $rootScope) {


    // **********
    // variables
    $scope.PageConfig = $rootScope.PageConfig;
    $scope.FlightSearchForm = $rootScope.FlightSearchForm;

    $scope.PageConfig.Loaded = true;


}]);