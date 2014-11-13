var ivNetMyMemberRegistration = angular.module("ivNet.My.Member.Registration.App", ['ngResource', 'trNgGrid']);

ivNetMyMemberRegistration.factory('member', function ($resource) {
    return $resource('/api/club/mymember/:id', null,
    {
        'update': { method: 'PUT' }
    });
});

ivNetMyMemberRegistration.controller('RegistrationController', function ($scope, member) {
    alert($scope.username);
    member.query(
       function (data) {
           $scope.members = data;
       },
       function (error) {
           alert(error.data);
       });

});
