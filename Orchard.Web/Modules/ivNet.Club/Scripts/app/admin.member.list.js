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
    });
});

ivNetAdminMemberList.factory('adminMember', function ($resource) {
    return $resource('/api/club/admin/member/:id', null,
    {
        'query': { method: 'GET', isArray: false },
        'update': { method: 'PUT' }

    });
});

ivNetAdminMemberList.controller('AdminMemberListController', function ($scope, adminMemberList, adminMember) {

    $('div#memberDetail').hide();

    adminMemberList.query(
        function(data) {
            $scope.members = data;
        },
        function(error) {
            alert(error.data.Message + ' [' + error.data.MessageDetail + ']');
        });

    $scope.editMemberList = function(member) {
       
        $('div.memberDetails').hide();

        adminMember.query({ id: member.MemberId },
            function(data) {
                $scope.memberDetails = data;

                $scope.memberType = data.MemberType;

                $('div#memberList').hide("blind", { direction: "up" }, 500, function() {
                    // guardian returned
                    if (data.MemberType == 1) {
                        $scope.$apply(function() {
                            $scope.guardian = data.Guardians[0];
                            $scope.juniors = data.Juniors;
                        });
                        $('div#guardianDetails').show();
                        $('div#juniorRepeatDetails').show();
                    } else if (data.MemberType == 2) {
                        $scope.$apply(function() {
                            $scope.junior = data.Juniors[0];
                            $scope.guardians = data.Guardians;
                        });
                        $('div#juniorDetails').show();
                        $('div#guardianRepeatDetails').show();
                    }

                    $('div#memberDetail').show();
                    
                });
              

                $('div#memberDetail').find('tfoot').hide();

            },
            function(error) {
                alert(error.data.Message + ' [' + error.data.MessageDetail + ']');
            });
    };

    //$scope.editMemberDetail = function (member) {

    //    $('div#memberDetail').hide("blind", { direction: "up" },1000, function () {
    //        $('div#memberList').show("blind", { direction: "down" }, 500);
    //    });

    //};

    $scope.saveChanges = function() {
        adminMember.update({ id: 1 }, $scope.memberDetails,
                   function () {
                      
                           alert("Saved OK");
                       
                   },
                   function (error) {
                       alert(error.data.Message + ' [' + error.data.MessageDetail + ']');
                   }
               );
    };

    $scope.backToList = function() {
        $('div#memberDetail').hide("blind", { direction: "up" }, 500, function () {
            $('div.memberDetails').hide();
            $('div#memberList').show();
        });
    };
});