var ivNetMemberRegistrationPayment = angular.module("ivNet.Member.Registration.Payment.App", ['ngResource', 'trNgGrid'])

ivNetMemberRegistrationPayment.factory('registrationpayment', function ($resource) {
    return $resource('/api/club/payment/:id', null,
    {
        'update': { method: 'PUT' }
    });
});

ivNetMemberRegistrationPayment.controller('PaymentController', function($scope, registrationpayment) {

    registrationpayment.query(function(data) {
        $scope.items = data;
    });

});