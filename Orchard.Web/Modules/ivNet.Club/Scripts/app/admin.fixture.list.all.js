var ivNetAdminFixtureList = angular.module('Admin.Fixture.List.All.App', ['ngResource', 'trNgGrid', 'ngCsv'])
    .filter("dateField", function () {
        return function (combinedFieldValueUnused, item) {
            var d = item.Date;

            var curr_date = d.substr(8, 2);
            var curr_month = d.substr(5, 2);
            var curr_year = d.substr(0, 4);

            return curr_date + "/" + curr_month + "/" + curr_year;
        };
    });

ivNetAdminFixtureList.factory('adminFixture', function ($resource) {
    return $resource('/api/club/adminfixture/:id', null,
    {
        'query': { method: 'GET', isArray: false },
        'update': { method: 'PUT' }
    });
});

ivNetAdminFixtureList.factory('adminPaginatedFixtureList', function ($resource) {
    return $resource('/api/club/adminfixture/:id/:gridOptions', null,
    {
        'query': { method: 'GET', isArray: false },
    });
});

ivNetAdminFixtureList.controller('AdminFixtureListController', function ($scope, adminFixture, adminPaginatedFixtureList) {

    init();

    $scope.editFixture = function (fixture) {

        adminFixture.query({ Id: fixture.Id },
              function (data) {
                  $scope.data = data;

                  $scope.selectedFixture = data.Fixtures[0];                  

                  $scope.teams = data.Teams;
                  $scope.team = data.Fixtures[0].TeamId;
                
                  $scope.opponents = data.Opponents;
                  $scope.opponent = data.Fixtures[0].OpponentId;

                  $scope.homeoraway = data.HomeOrAway;
                  $scope.homeaway = data.Fixtures[0].HomeAway;

                  $scope.fixturetypes = data.FixtureTypes;
                  $scope.fixturetype = data.Fixtures[0].FixtureTypeId;

                  $scope.resulttypes = data.ResultTypes;
                  $scope.resulttype = data.Fixtures[0].ResultTypeId;

                  $scope.score = data.Fixtures[0].Score;

                  $scope.locations = data.Locations;
                  
              },
              function (error) {
                  alert(error.data.Message + ' [' + error.data.MessageDetail + ']');
              });

        $('div.selected-fixture').show();
        $('div.fixture-list').hide();
       
    };

    $scope.saveChanges = function() {
        var data = {TeamId:$scope.team,OpponentId:$scope.opponent };
        adminFixture.update({ id: $scope.selectedFixture.Id }, data,
            function() {
                window.location.reload();
            },
            function(error) {
                alert(error.data.Message + ' [' + error.data.MessageDetail + ']');
            }
        );
    };

    $scope.backToList = function () {
        $('div#fixtureDetail').hide("blind", { direction: "up" }, 500, function () {
            $('div.selected-fixture').hide();
            $('div.fixture-list').show();
        });
    };
    $scope.onServerSideItemsRequested = function (currentPage, pageItems, filterBy, filterByFields, orderBy, orderByReverse) {

        $('#loading-indicator').show();

        $('#loading-indicator').show();

        if (filterBy == null) {
            filterBy = "";
        }

        if (orderBy == null) {
            orderBy = "Date";
            orderByReverse = true;
        }

        adminPaginatedFixtureList.query({ CurrentPage: currentPage, OrderBy: orderBy, OrderByReverse: orderByReverse, PageItems: pageItems, FilterBy: filterBy, FilterByFields: angular.toJson(filterByFields) },
            function (data) {
                $scope.fixtures = data.results;
                $scope.itemsTotalCount = data.totalCount;
                $('#loading-indicator').hide();
            },
            function (error) {
                $('#loading-indicator').hide();
                alert(error.data.Message + ' [' + error.data.MessageDetail + ']');
            });
    };

    function init() {
        $scope.pageItemsCount = 20;
        $scope.itemsTotalCount = 0;
        $('div.selected-fixture').hide();
        $('#loading-indicator').show();
    }
});