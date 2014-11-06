var ivNetAdminMemberRegistration = angular.module("ivNet.Admin.Member.Members.App", ['ngResource', 'trNgGrid']);


ivNetAdminMemberRegistration.factory('adminMemberMembers', function ($resource) {
    return $resource('/api/club/admin/members/:id', null,
    {
        'update': { method: 'PUT' }
    });
});

ivNetAdminMemberRegistration.controller('AdminMemberController', function ($scope, adminMemberMembers) {
    adminMemberMembers.query(
        function(data) {
            $scope.myItems = data;
        },
        function(error) {
            alert(error);
        });
});