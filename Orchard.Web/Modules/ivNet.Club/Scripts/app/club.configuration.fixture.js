var ivNetClubConfiguration = angular.module('Club.Configuration.Fixture.App', ['ngResource', 'trNgGrid']);

ivNetClubConfiguration.factory('configuration', function ($resource) {
    return $resource('/api/club/admin/configuration/:id', null,
    {
        'update': { method: 'PUT' }
    });
});

ivNetClubConfiguration.controller('ConfigurationController', function ($scope, configuration) {
    alert("cakes");
    configuration.query({id: "team"},
        function(data) {
            $scope.teams = data;
        },
        function(error) {
            alert(error.data.Message + ' [' + error.data.MessageDetail + ']');
        });

    configuration.query({ id: "opponent" },
      function (data) {
          $scope.opponents = data;
      },
      function (error) {
          alert(error.data.Message + ' [' + error.data.MessageDetail + ']');
      });

    configuration.query({ id: "fixturetype" },
     function (data) {
         $scope.fixturetypes = data;
     },
     function (error) {
         alert(error.data.Message + ' [' + error.data.MessageDetail + ']');
     });

    configuration.query({ id: "location" },
    function (data) {
        $scope.locations = data;
    },
    function (error) {
        alert(error.data.Message + ' [' + error.data.MessageDetail + ']');
    });
});