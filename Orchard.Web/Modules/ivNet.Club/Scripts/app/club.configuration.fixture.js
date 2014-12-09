var ivNetClubConfiguration = angular.module('Club.Configuration.Fixture.App', ['ngResource', 'trNgGrid']);

ivNetClubConfiguration.factory('configuration', function ($resource) {
    return $resource('/api/club/admin/configurationfixture/:id', null,
    {
        'query': { method: 'GET', isArray: false },
        'update': { method: 'PUT' }
    });
});

ivNetClubConfiguration.controller('ConfigurationController', function ($scope, configuration) {

    configuration.query({id: "fixture"},
        function (data) {
            $scope.data = data;
            $scope.teams = data.Teams;
            $scope.opponents = data.Opponents;
            $scope.fixturetypes = data.FixtureTypes;
            $scope.locations = data.Locations;
        },
        function(error) {
            alert(error.data.Message + ' [' + error.data.MessageDetail + ']');
        });

    $scope.saveItem = function (item, type) {
        $scope.data.Type = type;

        $('table#' + type + ' tr').each(function (index, tr) {
            if (tr.children[0].innerText == item.Id) {
                item.Name = $(tr).find('td[field-name="Name"]').find('input').val();
                item.Postcode = $(tr).find('td[field-name="Postcode"]').find('input').val();
                item.Longitude = $(tr).find('td[field-name="Longitude"]').find('input').val();
                item.Latitude = $(tr).find('td[field-name="Latitude"]').find('input').val();
                item.IsActive = $(tr).find('td[field-name="IsActive"]').find('input:checked').length;
            }
        });
        var configData = {};

        switch (type) {
            case "teamconfig":
            case "opponentconfig":
            case "fixturetypeconfig":
                configData = { Type: type, Name: item.Name, IsActive: item.IsActive };            
                break;
            case "locationconfig":
                configData = { Type: type, Name: item.Name, Postcode: item.Postcode, Longitude: item.Longitude, Latitude: item.Latitude, IsActive: item.IsActive };
                break;

        }

        configuration.update({ id: item.Id }, configData,
                   function () {
                       window.location.reload();
                   },
                   function (error) {
                       alert(error.data.Message + ' [' + error.data.MessageDetail + ']');
                   }
               );

    };
});