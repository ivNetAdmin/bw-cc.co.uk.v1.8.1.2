﻿var ivNetClubStats = angular.module('Admin.Stats.New.App', ['ngResource', 'trNgGrid', 'ui.bootstrap'])
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

ivNetClubStats.factory('fixturePaginatedList', function ($resource) {
    return $resource('/api/club/adminfixture/:id/:gridOptions', null,
    {
        'query': { method: 'GET', isArray: true },
    });
});

ivNetClubStats.factory('fixturestat', function ($resource) {
    return $resource('/api/club/admin/adminfixturestat/:id', null,
    {
        'query': { method: 'GET', isArray: false },
        'update': { method: 'PUT' }
    });
});

ivNetClubStats.controller('AdminStatsController', function ($scope, fixture, fixturePaginatedList, fixturestat) {
    
    init();

    $scope.onServerSideItemsRequested = function (currentPage, pageItems, filterBy, filterByFields, orderBy, orderByReverse) {

        $('#loading-indicator').show();

        if (filterBy == null) {
            filterBy = "";
        }

        if (orderBy == null) {
            orderBy = "Date";
            orderByReverse = true;
        }

        fixturePaginatedList.query({ CurrentPage: currentPage, OrderBy: orderBy, OrderByReverse: orderByReverse, PageItems: pageItems, FilterBy: filterBy, FilterByFields: angular.toJson(filterByFields) },
            function (data) {
                $scope.fixtures = data;
                $scope.itemsTotalCount = 200;
                $('#loading-indicator').hide();
            },
            function (error) {
                $('#loading-indicator').hide();
                alert(error.data.Message + ' [' + error.data.MessageDetail + ']');
            });
    };

    $scope.$watch("selectedFixtures.length", function (newLength) {
        if (newLength > 0) {
            $scope.selectedFixture = $scope.selectedFixtures[newLength - 1];

            fixturestat.query({ Id: $scope.selectedFixture.Id },
              function (data) {
                  $scope.playerStats = data.PlayerStats;
                  $scope.howout = data.HowOut;
                  $scope.yesno = data.YesNo;
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
                $scope.playerStats[index - 2].Overs = $(tr).find('td[field-name="Overs"]').find('input').val();
                $scope.playerStats[index - 2].Maidens = $(tr).find('td[field-name="Maidens"]').find('input').val();
                $scope.playerStats[index - 2].Wickets = $(tr).find('td[field-name="Wickets"]').find('input').val();
                $scope.playerStats[index - 2].RunsConceeded = $(tr).find('td[field-name="RunsConceeded"]').find('input').val();
                $scope.playerStats[index - 2].Catches = $(tr).find('td[field-name="Catches"]').find('input').val();
                $scope.playerStats[index - 2].Captain = $(tr).find('td[field-name="Captain"]').find('select').val();
                $scope.playerStats[index - 2].Keeper = $(tr).find('td[field-name="Keeper"]').find('select').val();

                if ($scope.playerStats[index - 2].HowOut == "") $scope.playerStats[index - 2].HowOut = "0";
                if ($scope.playerStats[index - 2].Captain == "") $scope.playerStats[index - 2].Captain = "0";
                if ($scope.playerStats[index - 2].Keeper == "") $scope.playerStats[index - 2].Keeper = "0";
            }
        });
        //alert(JSON.stringify($scope.playerStats));
        fixturestat.update({ id: $scope.selectedFixture.Id }, JSON.stringify($scope.playerStats),
            function () {
                window.location.reload();
            },
            function (error) {
                alert(error.data.Message + ' [' + error.data.MessageDetail + ']');
            }
        );
    };

    function init() {
       
        //fixture.query(
        //    function (data) {
        //        $scope.data = data;
        //        $scope.fixtures = data.Fixtures;              
        //    },
        //    function (error) {
        //        alert(error.data.Message + ' [' + error.data.MessageDetail + ']');
        //    });

        $scope.pageItemsCount = 20;
        $scope.itemsTotalCount = 0;
        $('div#memberDetail').hide();
        $('#loading-indicator').show();

        $scope.selectedFixtures = [];
    }

   
});