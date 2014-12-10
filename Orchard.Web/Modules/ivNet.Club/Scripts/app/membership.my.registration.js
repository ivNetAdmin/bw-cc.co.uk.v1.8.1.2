var ivNetMemberRegistrationDetails = angular.module("Membership.My.Registration.App", ['ngResource', 'trNgGrid', 'ui.event'])
    .filter("editDateField", function() {
        return function(combinedFieldValueUnused, item) {
            var d = item.Dob;

            var curr_date = d.substr(8, 2);
            var curr_month = d.substr(5, 2);
            var curr_year = d.substr(0, 4);

            return curr_year + "-" + curr_month + "-" + curr_date;
        };
    })
    .directive('myPostRepeatDirective', function() {
    return function(scope, element, attrs) {
        if (scope.$last) {

            $('div#guardianRepeatDetails').find('input.disabled').each(function(index, item) {
                $(item).attr('disabled', '');
            });
           
            $('div#juniorRepeatDetails').find('input.disabled').each(function(index, item) {
                $(item).attr('disabled', '');
            });
        }
    };
});

ivNetMemberRegistrationDetails.factory('memberRegistrationDetails', function ($resource) {
    return $resource('/api/club/registration/:id/:type', null,
    {
        'query': { method: 'GET', isArray: false },
        'update': { method: 'PUT' }
    });
});

ivNetMemberRegistrationDetails.factory('guardianRegistrationDetails', function ($resource) {
    return $resource('/api/club/registration/:id/:type', null,
    {
        'query': { method: 'GET', isArray: false },
        'update': { method: 'PUT' }
    });
});

ivNetMemberRegistrationDetails.controller('MembershipMyRegistrationController', function ($scope, memberRegistrationDetails, guardianRegistrationDetails) {

    angular.element(document).ready(function () {        

    });
    
    guardianRegistrationDetails.query({ id: "registered", type: "user" },
        function(data) {
            $('div#ErrorNoData').hide();
            $('div.memberDetails').hide();      
            $('div#guardianRepeatDetails').show();
            $('div#juniorRepeatDetails').show();
            $('input.btn').hide();
            $('.hide-me').hide();

            //$scope.$apply(function() {
                $scope.data = data;
                
                if (data.Guardians.length > 0) {
                    $scope.authenticatedUserName = data.Guardians[0].Email;
                    $scope.authenticatedMemberId = data.Guardians[0].MemberId;

                    $scope.guardianCount = data.Guardians.length;
                    $scope.juniorCount = data.Juniors.length;

                    $scope.guardians = data.Guardians;
                    $scope.juniors = data.Juniors;

                    $scope.guardian = data.NewGuardian;
                    $scope.junior = data.NewJunior;
                    $('input.btn').show();
                } else {
                    $('div#ErrorNoData').html($('div#ErrorNoData').html().replace('[[USER]]', data.AuthenticatedUser));
                    $('div#ErrorNoData').show();
                }
            //});

        },
        function(error) {
            alert(error.data.Message + ' [' + error.data.MessageDetail + ']');
        });

    $scope.saveChanges = function () {

        memberRegistrationDetails.update({ id: $scope.authenticatedMemberId }, $scope.data,
                   function () {
                       window.location.reload();
                   },
                   function (error) {
                       alert(error.data.Message + ' [' + error.data.MessageDetail + ']');
                   }
               );
    };

    $scope.addGuardian = function () {

        $('div#guardianDetails').find('input.required').each(function(index, item) {
            $(item).attr('required', '');
        });

        $('.hide-me').show();

        $('div#guardianDetails').show();
    };

    $scope.cancelGuardian = function () {

        $('div#guardianDetails').find('input.required').each(function (index, item) {
            $(item).removeAttr('required', '');
        });

        $('div#guardianDetails').hide();
    };

    $scope.addJunior = function () {
        $('div#juniorDetails').find('input.required').each(function (index, item) {
            $(item).attr('required', '');
        });

        $('div#juniorDetails').show();

    };

    $scope.cancelJunior = function () {
        $('div#juniorDetails').find('input.required').each(function (index, item) {
            $(item).removeAttr('required', '');
        });

        $('div#juniorDetails').hide();

    };

    $scope.clearGuardianSearch = function () {    
        clearSearch();
    };

    $scope.searchGuardian = function () {

        var searchTerm = $('input[name="GuardianSearchEmail"]').val();
        if (searchTerm.length > 0) {
            guardianSearch(searchTerm);
        }

    };

    $scope.blurEmailCallback = function (evt) {

        if (evt.target.value.length > 0) {
            checkEmailDuplicates(evt.target.value);
        }
    };

    function checkEmailDuplicates(email) {

        $.ajax({
            url: '/api/club/registration/' + email + '?action=duplicate&type=email',
            type: 'GET',
            success: function (data) {
                if (data.length > 0) {

                    $('div#dupError p.error').html(data);
                    $('div#dupError').show();
                    $('input#singlebutton').hide();

                } else {
                    $('div#dupError').hide();
                    $('input#singlebutton').show();
                }
            },
            error: function (jqXhr, textStatus, errorThrown) {
                alert("Error '" + jqXhr.status + "' (textStatus: '" + textStatus + "', errorThrown: '" + errorThrown + "')");
            }
        });
    }

    function guardianSearch(email) {

        $.ajax({
            url: '/api/club/guardian/' + email + '/email-search',
            type: 'GET',
            success: function (data) {
                if (data != null) {

                    $scope.$apply(function() {

                        $scope.NewGuardianId = data.GuardianId;
                        $scope.NewGuardianFirstname = data.Firstname;
                        $scope.NewGuardianSurname = data.Surname;
                        $scope.NewGuardianNickname = data.Nickname;
                        $scope.NewGuardianEmail = data.Email;
                        $scope.NewGuardianMobile = data.Mobile;
                        $scope.NewGuardianOtherTelephone = data.OtherTelephone;
                        $scope.NewGuardianAddress = data.Address;
                        $scope.NewGuardianTown = data.Town;
                        $scope.NewGuardianPostcode = data.Postcode;

                        $scope.SearchResult = "Guardian found...";
                    });

                    $('input[name="NewGuardianSurname"]').attr('disabled', '');
                    $('input[name="NewGuardianFirstname"]').attr('disabled', '');
                    $('input[name="NewGuardianEmail"]').attr('disabled', '');
                    $('input[name="NewGuardianEmailAgain"]').hide();

                } else {
                    $scope.$apply(function() {
                        $scope.SearchResult = "No guardian found...";
                    });
                }
            },
            error: function (jqXhr, textStatus, errorThrown) {
                alert("Error '" + jqXhr.status + "' (textStatus: '" + textStatus + "', errorThrown: '" + errorThrown + "')");
            }
        });
    }

    function clearSearch() {

        $scope.$apply(function () {           
            $scope.NewGuardianFirstname = "";
            $scope.NewGuardianSurname = "";
            $scope.NewGuardianNickname = "";
            $scope.NewGuardianEmail = "";
            $scope.NewGuardianMobile = "";
            $scope.NewGuardianOtherTelephone = "";
            $scope.NewGuardianAddress = "";
            $scope.NewGuardianTown = "";
            $scope.NewGuardianPostcode = "";
           
        });

        $('input[name="NewGuardianSurname"]').removeAttr('disabled');
        $('input[name="NewGuardianFirstname"]').removeAttr('disabled');
        $('input[name="NewGuardianEmail"]').removeAttr('disabled');
        $('input[name="NewGuardianId"]').val('');
        $('input[name="GuardianSearchEmail"]').val('');
        $('span.search-result').html('');
        
        $('input[name="NewGuardianEmailAgain"]').show();

    };

});