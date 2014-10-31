var ivNetClubConfiguration = angular.module('ivNet.Club.Configuration.App', ['ngResource', 'trNgGrid'])
    .filter("dateField", function() {
        return function (combinedFieldValueUnused, item) {
            var d = item.Date;
            
            var curr_date = d.substr(8, 2);
            var curr_month = d.substr(5, 2);
            var curr_year = d.substr(0, 4);
          
            return curr_date + "/" + curr_month + "/" + curr_year;
        };
});

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

    $scope.editItem = function(item) {
        alert(item.Id);
    };
});
