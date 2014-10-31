var ivNetMemberRegistrationPayment = angular.module("ivNet.Member.Registration.Payment.App", []);

ivNetMemberRegistrationPayment.controller('PaymentController', function ($scope, $http) {

    init();

    function init() {

        $scope.registrationPaymentUrl = '/api/club/payment';

        setTimeout(function () {
            $http.get($scope.registrationPaymentUrl)
                .success(function (data) {
                    
                })
                .error(function (data) {

                });
        }, 100);

    }

});