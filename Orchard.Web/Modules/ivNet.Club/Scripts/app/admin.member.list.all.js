var ivNetAdminMemberList = angular.module("Admin.Member.List.All.App", ['ngResource', 'trNgGrid'])
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
    return $resource('/api/club/adminmember/:id/:type', null,
    {
        'query': { method: 'GET', isArray: true },
    });
});

ivNetAdminMemberList.factory('adminPaginatedMemberList', function ($resource) {
    return $resource('/api/club/adminmember/:id/:gridOptions', null,
    {
        'query': { method: 'GET', isArray: true },
    });
});

ivNetAdminMemberList.factory('adminMember', function ($resource) {
    return $resource('/api/club/adminmember/:id', null,
    {
        'query': { method: 'GET', isArray: false },
        'update': { method: 'PUT' }

    });
});

ivNetAdminMemberList.controller('AdminMemberListController', function ($scope, adminMemberList, adminMember, adminPaginatedMemberList) {

    init();
    
    $scope.editMember = function(member) {
       
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

                    $('div#AdminMembers').find('input.disabled').each(function (index, item) {
                        $(item).attr('disabled', '');
                    });

                    $('div#AdminMembers').find('input.required').each(function (index, item) {
                        $(item).attr('required', '');
                    });

                    $('div#AdminMembers').find('input.hide').each(function (index, item) {
                        $(item).hide();
                    });


                    $('div#memberDetail').show();
                    
                });
              

                $('div#memberDetail').find('tfoot').hide();

            },
            function(error) {
                alert(error.data.Message + ' [' + error.data.MessageDetail + ']');
            });
    };

    $scope.saveChanges = function () {
        $scope.memberDetails.Type = 'admin';
        adminMember.update({ id: 1 }, $scope.memberDetails,
            function() {
                window.location.reload();
            },
            function(error) {
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

    $scope.toggleGuardianActivity = function (guardian) {
        if (!guardian.MemberIsActive) {
            alert("If you continue then all of this guardians junior wards will also be deactivated, unless the junior has another 'active' guardian");
        }
    };

    $scope.onServerSideItemsRequested = function (currentPage, pageItems, filterBy, filterByFields, orderBy, orderByReverse) {
        
        $('#loading-indicator').show();

        if (filterBy == null) {
            filterBy = "";
        }

        if (orderBy == null) {
            orderBy = "Surname";
        }

        adminPaginatedMemberList.query({ CurrentPage: currentPage, OrderBy: orderBy, OrderByReverse: orderByReverse, PageItems: pageItems, FilterBy: filterBy, FilterByFields: angular.toJson(filterByFields) },
            function (data) {
                $scope.members = data;
                $('#loading-indicator').hide();
            },
        function (error) {
            $('#loading-indicator').hide();
            alert(error.data.Message + ' [' + error.data.MessageDetail + ']');
        });

        //adminMemberList.query({ id: "all", type: "list" },
        //function (data) {
        //    $scope.members = data;
        //    $('#loading-indicator').hide();
        //},
        //function (error) {
        //    alert(error.data.Message + ' [' + error.data.MessageDetail + ']');
        //});

    };

    function init() {      
        $('div#memberDetail').hide();
        $('#loading-indicator').show();    
    }
});