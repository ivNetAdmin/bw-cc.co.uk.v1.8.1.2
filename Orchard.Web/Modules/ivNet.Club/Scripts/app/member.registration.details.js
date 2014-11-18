var ivNetMemberRegistrationDetails = angular.module("ivNet.Member.Registration.Details.App", ['ngResource', 'trNgGrid']);

ivNetMemberRegistrationDetails.factory('memberRegistrationDetails', function ($resource) {
    return $resource('/api/club/member/:id/:type', null,
    {
        'query': { method: 'GET', isArray: false },
        'update': { method: 'PUT' }
    });
});

ivNetMemberRegistrationDetails.controller('MemberRegistrationDetailsController', function($scope, memberRegistrationDetails) {

    memberRegistrationDetails.query({ id: "authenticated", type: "user" },
        function(userModel) {
            $scope.authenticatedUserId = userModel.Id;

            memberRegistrationDetails.query({ id: $scope.authenticatedUserId, type: "userid" },
                function(data) {
                    $scope.items = data;
                },
                function(error) {
                    alert(error.data.Message + ' [' + error.data.MessageDetail + ']');
                });

        },
        function(error) {
            alert(error.data.Message + ' [' + error.data.MessageDetail + ']');
        });

});p