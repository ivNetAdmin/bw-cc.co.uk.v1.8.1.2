var ivNetClubFixture = angular.module('Admin.Fixture.New.App', ['ngResource', 'trNgGrid'])
.filter("dateField", function () {
    return function (combinedFieldValueUnused, item) {
        var d = item.Date;

        var curr_date = d.substr(8, 2);
        var curr_month = d.substr(5, 2);
        var curr_year = d.substr(0, 4);

        return curr_year + "-" + curr_month + "-" + curr_date;
    };
});

ivNetClubFixture.factory('fixture', function ($resource) {
    return $resource('/api/club/adminfixture/fixture/:id', null,
    {
        'update': { method: 'PUT' }
    });
});

ivNetClubFixture.controller('AdminFixtureController', function ($scope, fixture) {

    init();

    function init() {

        fixture.query(
        function (data) {
            $scope.dataItems = data;
        },
        function (error) {
            alert(error.data.Message + ' [' + error.data.MessageDetail + ']');
        });
    }

});