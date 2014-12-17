var ivNetClubTeam = angular.module('Admin.Team.New.App', ['ngResource', 'trNgGrid', 'ui.bootstrap'])
    .filter("dateField", function () {
        return function (combinedFieldValueUnused, item) {
            var d = item.Date;

            var curr_date = d.substr(8, 2);
            var curr_month = d.substr(5, 2);
            var curr_year = d.substr(0, 4);

            return curr_date + "/" + curr_month + "/" + curr_year;
        };
    });

ivNetClubTeam.factory('team', function ($resource) {
    return $resource('/api/club/admin/adminTeam/:id', null,
    {
        'query': { method: 'GET', isArray: false },
        'update': { method: 'PUT' }
    });
});

ivNetClubTeam.controller('AdminTeamController', function($scope, team) {
    init();

    function init() {

        team.query(
            function(data) {
                $scope.data = data;
                $scope.fixtures = data.AdminFixtureViewModel.Fixtures;
                $scope.teams = data.AdminFixtureViewModel.Teams;
                $scope.opponents = data.AdminFixtureViewModel.Opponents;
                $scope.fixturetypes = data.AdminFixtureViewModel.FixtureTypes;
                $scope.locations = data.AdminFixtureViewModel.Locations;
                $scope.homeoraway = data.AdminFixtureViewModel.HomeOrAway;
                $scope.onMediaPlayerStateChange = data.onMediaPlayerStateChange;

                $scope.players = data.Players;
                $scope.teamSelection = data.TeamSelection;
            },
            function(error) {
                alert(error.data.Message + ' [' + error.data.MessageDetail + ']');
            });

        $scope.selectedFixtures = [];
    }

    $scope.$watch("selectedFixtures.length", function(newLength) {
        if (newLength > 0) {
            $scope.selectedFixture = $scope.selectedFixtures[newLength - 1];
            $('div.selected-fixture').show();
        } else {
            $('div.selected-fixture').hide();
            $scope.selectedFixture = null;
        }
    });

    $scope.onSelect = function($item, $model, $label) {
        $scope.teamSelection[$model].PlayerNumber = $item.PlayerNumber;
        $scope.teamSelection[$model].Name = $item.Name;
    };

    $scope.saveTeamSelection = function() {

        team.update({ id: $scope.selectedFixture.Id }, { TeamSelection: $scope.teamSelection },
            function() {
                window.location.reload();
            },
            function(error) {
                alert(error.data.Message + ' [' + error.data.MessageDetail + ']');
            }
        );
    };
});