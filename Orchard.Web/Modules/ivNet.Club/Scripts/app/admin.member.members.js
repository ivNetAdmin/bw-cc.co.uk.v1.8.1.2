var ivNetAdminMemberRegistration = angular.module("ivNet.Admin.Member.Members.App", ['ngResource', 'trNgGrid'])
    .filter("dateField", function() {
        return function(combinedFieldValueUnused, item) {
            var d = item.Dob;

            var curr_date = d.substr(8, 2);
            var curr_month = d.substr(5, 2);
            var curr_year = d.substr(0, 4);

            return curr_date + "/" + curr_month + "/" + curr_year;
        };
    })
    .filter("editDateField", function() {
        return function(combinedFieldValueUnused, item) {
            var d = item.Dob;

            var curr_date = d.substr(8, 2);
            var curr_month = d.substr(5, 2);
            var curr_year = d.substr(0, 4);

            return curr_year + "-" + curr_month + "-" + curr_date;
        };
    });

ivNetAdminMemberRegistration.factory('adminMemberMembers', function ($resource) {
    return $resource('/api/club/admin/members/:id', null,
    {
        'query': { method: 'GET', isArray: true },
        'update': { method: 'PUT' }

    });
});

ivNetAdminMemberRegistration.controller('AdminMemberController', function ($scope, adminMemberMembers) {

    $('table#memberDetail').hide();

    adminMemberMembers.query(
        function(data) {
            $scope.members = data;
        },
        function(error) {
            alert(error);
        });

    $scope.editMemberList = function (member) {

        adminMemberMembers.query({ id: member.MemberId },
            function (data) {
              
                $scope.memberDetails = data;

                $('table#memberList').hide("blind", { direction: "up" }, 500, function () {
                    $('table#memberDetail').show("blind", { direction: "down" }, 1000);
                });

            },
            function(error) {
                alert(error);
            });

       

    };

    $scope.editMemberDetail = function (member) {

        $('table#memberDetail').hide("blind", { direction: "up" },1000, function () {
            $('table#memberList').show("blind", { direction: "down" }, 500);
        });

    };
});