var ivNetClubReport = angular.module('Admin.Report.New.App', ['ngResource', 'trNgGrid', 'ui.bootstrap'])
 .filter("dateField", function () {
 	return function (combinedFieldValueUnused, item) {
 		var d = item.Date;

 		var curr_date = d.substr(8, 2);
 		var curr_month = d.substr(5, 2);
 		var curr_year = d.substr(0, 4);

 		return curr_date + "/" + curr_month + "/" + curr_year;
 	};
 });

ivNetClubReport.factory('fixture', function ($resource) {
	return $resource('/api/club/fixture/:id', null,
    {
    	'query': { method: 'GET', isArray: false },
    });
});

ivNetClubReport.factory('fixturereport', function ($resource) {
	return $resource('/api/club/admin/adminfixturereport/:id', null,
    {
    	'query': { method: 'GET', isArray: false },
    	'update': { method: 'PUT' }
    });
});

ivNetClubReport.controller('AdminReportController', function ($scope, fixture, fixturereport) {

	init();

	function init() {

	    CKEDITOR.replace('MatchReport');

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

		    fixturereport.query({ Id: $scope.selectedFixture.Id },
		        function (data) {
		            $scope.score = data.FixtureScore;
		            $scope.fixtureResult = data.FixtureResult;
		            $scope.results = data.Results;
		            CKEDITOR.instances.MatchReport.setData(data.MatchReport);
		        },
		        function(error) {
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
	    var report = CKEDITOR.instances.MatchReport.getData();

	    fixturereport.update({ id: $scope.selectedFixture.Id }, { FixtureScore: $scope.score, FixtureResult: $scope.fixtureResult, MatchReport: report },
           function () {
               window.location.reload();
           },
           function (error) {
               alert(error.data.Message + ' [' + error.data.MessageDetail + ']');
           }
       );

	};

    //$scope.saveItem = function () {

    //	$('table#playerStatsTable tr').each(function (index, tr) {

    //		if ($(tr).find('td[field-name="PlayerName"]').length > 0) {
    //			$scope.playerStats[index - 2].Runs = $(tr).find('td[field-name="Runs"]').find('input').val();
    //			$scope.playerStats[index - 2].HowOut = $(tr).find('td[field-name="HowOut"]').find('select').val();
    //			$scope.playerStats[index - 2].Overs = $(tr).find('td[field-name="Overs"]').find('input').val();
    //			$scope.playerStats[index - 2].Maidens = $(tr).find('td[field-name="Maidens"]').find('input').val();
    //			$scope.playerStats[index - 2].Wickets = $(tr).find('td[field-name="Wickets"]').find('input').val();
    //			$scope.playerStats[index - 2].RunsConceeded = $(tr).find('td[field-name="RunsConceeded"]').find('input').val();
    //			$scope.playerStats[index - 2].Catches = $(tr).find('td[field-name="Catches"]').find('input').val();
    //			$scope.playerStats[index - 2].Captain = $(tr).find('td[field-name="Captain"]').find('select').val();
    //			$scope.playerStats[index - 2].Keeper = $(tr).find('td[field-name="Keeper"]').find('select').val();

    //			if ($scope.playerStats[index - 2].HowOut == "") $scope.playerStats[index - 2].HowOut = "0";
    //			if ($scope.playerStats[index - 2].Captain == "") $scope.playerStats[index - 2].Captain = "0";
    //			if ($scope.playerStats[index - 2].Keeper == "") $scope.playerStats[index - 2].Keeper = "0";
    //		}
    //	});
    //	//alert(JSON.stringify($scope.playerStats));
    //	fixturestat.update({ id: $scope.selectedFixture.Id }, JSON.stringify($scope.playerStats),
    //        function () {
    //        	window.location.reload();
    //        },
    //        function (error) {
    //        	alert(error.data.Message + ' [' + error.data.MessageDetail + ']');
    //        }
    //    );
    //};
});