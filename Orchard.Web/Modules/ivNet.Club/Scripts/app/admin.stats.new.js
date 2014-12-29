var ivNetClubStats = angular.module('Admin.Stats.New.App', ['ngResource', 'trNgGrid', 'ui.bootstrap'])
 .filter("dateField", function () {
     return function (combinedFieldValueUnused, item) {
         var d = item.Date;

         var curr_date = d.substr(8, 2);
         var curr_month = d.substr(5, 2);
         var curr_year = d.substr(0, 4);

         return curr_date + "/" + curr_month + "/" + curr_year;
     };
 });

ivNetClubStats.factory('fixture', function ($resource) {
    return $resource('/api/club/fixture/:id', null,
    {
        'query': { method: 'GET', isArray: false },
    });
});

ivNetClubStats.factory('fixturestat', function ($resource) {
    return $resource('/api/club/admin/adminfixturestat/:id', null,
    {
        'query': { method: 'GET', isArray: false },
    });
});

ivNetClubStats.controller('AdminStatsController', function ($scope, fixture, fixturestat) {
    
    init();

    function init() {

        fixture.query(
            function (data) {
                $scope.data = data;
                $scope.fixtures = data.Fixtures;              
            },
            function (error) {
                alert(error.data.Message + ' [' + error.data.MessageDetail + ']');
            });

        $scope.selectedFixtures = [];
    }

    $scope.$watch("selectedFixtures.length", function (newLength) {
        if (newLength > 0) {
            $scope.selectedFixture = $scope.selectedFixtures[newLength - 1];           

            fixturestat.query({ Id: $scope.selectedFixture.Id },
              function (data) {
                  $scope.playerStats = data.PlayerStats;
                  $scope.howout = data.HowOut;
              },
              function (error) {
                  alert(error.data.Message + ' [' + error.data.MessageDetail + ']');
              });

            $('div.selected-fixture').show();
            $('div.fixture-list').hide();
        } else {
            $('div.selected-fixture').hide();
            $scope.selectedFixture = null;
        }
    });

    $scope.saveItem = function () {
        
        $('table#playerStatsTable tr').each(function (index, tr) {

            if ($(tr).find('td[field-name="PlayerName"]').length > 0) {            
                $scope.playerStats[index - 2].Runs = $(tr).find('td[field-name="Runs"]').find('input').val();
                $scope.playerStats[index - 2].HowOut = $(tr).find('td[field-name="HowOut"]').find('select').val();
            }
        });

        alert(JSON.stringify($scope.playerStats));
    };
});