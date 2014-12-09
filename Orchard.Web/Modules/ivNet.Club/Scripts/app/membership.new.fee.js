var ivNetNewMemberFee = angular.module("Membership.New.Fee.App", ['ngResource', 'trNgGrid'])
.filter("dobField", function () {
    return function (combinedFieldValueUnused, item) {
        var d = item.Dob;

        var curr_date = d.substr(8, 2);
        var curr_month = d.substr(5, 2);
        var curr_year = d.substr(0, 4);
        return curr_date + "/" + curr_month + "/" + curr_year;
    };
});

ivNetNewMemberFee.factory('newmemberfee', function ($resource) {
    return $resource('/api/club/fee/:id', null,
    {
        'update': { method: 'PUT' }
    });
});

ivNetNewMemberFee.controller('NewMemberFeeController', function ($scope, newmemberfee) {

    newmemberfee.query(function (data) {
        $scope.items = data;
        calculateTotal();
    },
        function (error) {
            alert(error.data.Message + ' [' + error.data.MessageDetail + ']');
        });

    $.ajax({
        url: '/api/club/configurationclub/fees',
        dataType: 'json',
        success: function (results) {
            $scope.extraItems = results;
        },
        error: function (xmlhttprequest, status, error) {
            alert(error.data.Message + ' [' + error.data.MessageDetail + ']');
        }
    });

    $scope.updateTotal = function () {
        calculateTotal();
    };

    function calculateTotal() {
        $scope.paymentTotal = 0;

        angular.forEach($scope.items, function (item, index) {
            $scope.paymentTotal += item.Fee * 1;
        });

        $('ul#extraFeeItems').find('input:checked').each(function (index, item) {
            $scope.paymentTotal += item.value * 1;
        });

    };

});