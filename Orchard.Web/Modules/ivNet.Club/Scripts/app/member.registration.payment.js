var ivNetMemberRegistrationPayment = angular.module("ivNet.Member.Registration.Payment.App", ['ngResource', 'trNgGrid'])
.filter("dobField", function () {
    return function (combinedFieldValueUnused, item) {
        var d = item.Dob;

        var curr_date = d.substr(8, 2);
        var curr_month = d.substr(5, 2);
        var curr_year = d.substr(0, 4);
        return curr_date + "/" + curr_month + "/" + curr_year;
    };
});

ivNetMemberRegistrationPayment.factory('registrationpayment', function ($resource) {
    return $resource('/api/club/payment/:id', null,
    {
        'update': { method: 'PUT' }
    });
});

ivNetMemberRegistrationPayment.controller('PaymentController', function ($scope, registrationpayment) {

    registrationpayment.query(function(data) {
            $scope.items = data;            
            calculateTotal();
        },
        function(error) {
            alert(error.data);
        });

    $.ajax({
        url: '/api/club/configuration/extrareg',
        dataType: 'json',
        success: function (results) {
            $scope.extraItems = results;          
        },
        error: function(xmlhttprequest, status, error) {
            alert(error);
        }
    });

    $scope.updateTotal = function () {        
        calculateTotal();
    };

    function calculateTotal() {
        $scope.paymentTotal = 0;

        angular.forEach($scope.items, function(item, index) {
            $scope.paymentTotal += item.Fee * 1;
        });

        $('ul#extraFeeItems').find('input:checked').each(function (index, item) {
            $scope.paymentTotal += item.value * 1;
        });
  
    };

});