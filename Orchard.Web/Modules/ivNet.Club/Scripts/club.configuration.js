var ivNetClubConfiguration = angular.module('ivNet.Club.Configuration.App', ['ngResource','trNgGrid']);

ivNetClubConfiguration.factory("configFactory", function ($resource) {
    return $resource("/api/club/configuration");
    //return $resource("/api/posts/:id");
});

ivNetClubConfiguration.controller('ConfigurationController', function ($scope, configFactory) {
    configFactory.query(function (data) {
        $scope.myItems = data;
    });

    //configFactory.get({ id: 1 }, function (data) {
    //    $scope.post = data;
    //});
});
