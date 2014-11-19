var ivNetMemberRegistrationDetails = angular.module("ivNet.Member.Registration.Details.App", ['ngResource', 'trNgGrid'])
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
        $('table#newGuardianTable').hide();
        $('table#newJuniorTable').hide();                

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

        $('table#newGuardianTable').find('input').each(function(index, item) {
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
        $('table#newGuardianTable').show();
    };

    $scope.addJunior = function () {
        $('table#newJuniorTable').find('input').each(function (index, item) {
            switch ($(item).attr('name')) {
                case "NewJuniorFirstname":
                case "NewJuniorSurname":
                case "NewJuniorDob":

                    $(item).attr('required', '');
                    break;
            }
        });
        $('table').find('tfoot').hide();
        $('table#newJuniorTable').show();
    };

    $scope.blurEmailCallback = function (evt) {
        alert("cakes");
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
});