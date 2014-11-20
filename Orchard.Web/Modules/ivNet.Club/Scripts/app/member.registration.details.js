var ivNetMemberRegistrationDetails = angular.module("ivNet.Member.Registration.Details.App", ['ngResource', 'trNgGrid', 'ui.event'])
.filter("editDateField", function () {
    return function (combinedFieldValueUnused, item) {
        var d = item.Dob;

        var curr_date = d.substr(8, 2);
        var curr_month = d.substr(5, 2);
        var curr_year = d.substr(0, 4);

        return curr_year + "-" + curr_month + "-" + curr_date;
    };
});

ivNetMemberRegistrationDetails.factory('memberRegistrationDetails', function ($resource) {
    return $resource('/api/club/member/:id/:type', null,
    {
        'query': { method: 'GET', isArray: false },
        'update': { method: 'PUT' }
    });
});

ivNetMemberRegistrationDetails.factory('guardianRegistrationDetails', function ($resource) {
    return $resource('/api/club/guardian/:id/:type', null,
    {
        'query': { method: 'GET', isArray: false },
        'update': { method: 'PUT' }
    });
});

ivNetMemberRegistrationDetails.controller('MemberRegistrationDetailsController', function ($scope, memberRegistrationDetails, guardianRegistrationDetails) {
    init();

    function init() {      
        $('div#newGuardianTable').hide();
        $('div#newJuniorTable').hide();

        guardianRegistrationDetails.query({ id: "registered", type: "user" },
            function(data) {
                $scope.details = data;
                $scope.authenticatedUserName = data.MemberDetails[0].Email;
                $scope.memberDetails = data.MemberDetails;
                $scope.juniors = data.JuniorList;
                $scope.newMemberDetails = data.NewMemberDetails;
                $scope.newJuniors = data.NewJuniorList;

                $('table').find('tfoot').hide();
            },
            function(error) {
                alert(error.data.Message + ' [' + error.data.MessageDetail + ']');
            });
    }

    $scope.addGuardian = function () {        

        $('div#newGuardianTable table').find('input').each(function(index, item) {
            switch($(item).attr('name')){
                case "NewGuardianFirstname":
                case "NewGuardianSurname":
                case "NewGuardianMobile":
                case "NewGuardianEmail":
                case "NewGuardianAddress":
                case "NewGuardianPostcode":

                    $(item).attr('required','');
                    break;
            }
        });

        $('table').find('tfoot').hide();
        $('div#newGuardianTable').show();
    };

    $scope.addJunior = function () {
        $('div#newJuniorTable table').find('input').each(function (index, item) {
            switch ($(item).attr('name')) {
                case "NewJuniorFirstname":
                case "NewJuniorSurname":
                case "NewJuniorDob":

                    $(item).attr('required', '');
                    break;
            }
        });
        $('table').find('tfoot').hide();
        $('div#newJuniorTable').show();
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
            url: '/api/club/member/' + email + '/email-check',
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