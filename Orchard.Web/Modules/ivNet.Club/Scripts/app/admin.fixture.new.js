var ivNetClubFixture = angular.module('Admin.Fixture.New.App', ['ngResource', 'trNgGrid', 'ui.bootstrap'])
.filter("dateField", function () {
    return function (combinedFieldValueUnused, item) {
        var d = item.Date;

        var curr_date = d.substr(8, 2);
        var curr_month = d.substr(5, 2);
        var curr_year = d.substr(0, 4);

        return curr_year + "-" + curr_month + "-" + curr_date;
    };
})
.filter("homeAwayField", function () {
    return function (combinedFieldValueUnused, item) {
        var d = item.HomeAway;
        return d == 1 ? "Home" : d == 2 ? "Away" : "";
    };
});

ivNetClubFixture.factory('fixture', function ($resource) {
    return $resource('/api/club/admin/adminfixture/:id', null,
    {
        'query': { method: 'GET', isArray: false },
        'update': { method: 'PUT' }
    });
});

ivNetClubFixture.factory('fixtureconfigurationitem', function ($resource) {
    return $resource('/api/club/admin/configurationfixture/:id?type=:type', null,
    {
    });
});

ivNetClubFixture.controller('AdminFixtureController', function ($scope, fixture, fixtureconfigurationitem) {

    init();

    $scope.homeoraway = ['Home', 'Away'];

    $scope.opponentChange = function (opponent) {

        fixtureconfigurationitem.query({ id: opponent.Id, type: "opponent-locations" },
        function (data) {
            $scope.locations = data;
        },
        function (error) {
            alert(error.data.Message + ' [' + error.data.MessageDetail + ']');
        });

    };

    $scope.saveItem = function (item) {
     
        $('div#ClubConfiguration table tr').each(function (index, tr) {
            if (tr.children[0].innerText == item.Id) {                
                item.Date = $(tr).find('td[field-name="Date"]').find('input').val();
                item.Team = $(tr).find('td[field-name="Team"]').find('input').val();
                item.Opponent = $(tr).find('td[field-name="Opponent"]').find('input').val();
                item.HomeAway = $(tr).find('td[field-name="HomeAway"]').find('input').val();
                item.Location = $(tr).find('td[field-name="Location"]').find('input').val();
                item.FixtureType = $(tr).find('td[field-name="FixtureType"]').find('input').val();
                         
                fixture.update({ id: item.Id }, { Date: item.Date, Team: item.Team, Opponent: item.Opponent, HomeAway: item.HomeAway, Location: item.Location, FixtureType: item.FixtureType },
                    function () {
                        window.location.reload();
                    },
                    function (error) {
                        alert(error.data.Message + ' [' + error.data.MessageDetail + ']');
                    }
                );
            }
        });
    };

    function init() {

        fixture.query(
           function (data) {
               $scope.data = data;
               $scope.fixtures = data.Fixtures;              
               $scope.teams = data.Teams;
               $scope.opponents = data.Opponents;
               $scope.fixturetypes = data.FixtureTypes;
               $scope.locations = data.Locations;
           },
        function (error) {
            alert(error.data.Message + ' [' + error.data.MessageDetail + ']');
        });
    }

});