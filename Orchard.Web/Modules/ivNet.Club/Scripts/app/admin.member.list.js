var ivNetAdminMemberList = angular.module("ivNet.Admin.Member.List.App", ['ngResource', 'trNgGrid'])
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

ivNetAdminMemberList.factory('adminMemberList', function ($resource) {
    return $resource('/api/club/admin/member/:id', null,
    {
        'query': { method: 'GET', isArray: true },
        'update': { method: 'PUT' }

    });
});

ivNetAdminMemberList.controller('AdminMemberListController', function ($scope, adminMemberList) {

    $('div#memberDetail').hide();

    adminMemberList.query(
        function(data) {
            $scope.members = data;
        },
        function(error) {
            alert(error.data.Message + ' [' + error.data.MessageDetail + ']');
        });

    $scope.editMemberList = function(member) {

        adminMemberList.query({ id: member.MemberId },
            function(data) {

                $scope.memberDetails = data;

                if (data[0].MemberType != "Guardian") {
                    $scope.guardians = data[0].Guardians;
                }

                $('div#memberList').hide("blind", { direction: "up" }, 500, function () {                    
                    $('div#memberDetail').show("blind", { direction: "down" }, 1000);
                });

                $('div#memberDetail').find('tfoot').hide();

            },
            function(error) {
                alert(error.data.Message + ' [' + error.data.MessageDetail + ']');
            });
    };

    $scope.editMemberDetail = function (member) {

        $('div#memberDetail').hide("blind", { direction: "up" },1000, function () {
            $('div#memberList').show("blind", { direction: "down" }, 500);
        });

    };
});