var ivNetMemberRegistrationDetails = angular.module("ivNet.Member.Registration.Details.App", ['ngResource', 'trNgGrid'])

ivNetMemberRegistrationDetails.factory('memberRegistrationDetails', function ($resource) {
    return $resource('/api/club/admin/member/:id', null,
    {
        'update': { method: 'PUT' }
    });
});

ivNetMemberRegistrationDetails.controller('MemberRegistrationDetailsController', function ($scope, memberRegistrationDetails) {

    alert($scope.username);

    memberRegistrationDetails.query({ id: "new" },
        function(data) {
            $scope.myItems = data;
        },
        function(error) {
            alert(error.data.Message + ' [' + error.data.MessageDetail + ']');
        });
});