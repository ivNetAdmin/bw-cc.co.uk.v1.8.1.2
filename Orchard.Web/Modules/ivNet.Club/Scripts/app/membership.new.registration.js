var ivNetNewMember = angular.module("Membership.New.Registration.App", ['ui.event']);
var invalidCaptcha = false; // debug : false
ivNetNewMember.controller('MembershipNewRegistrationController', function ($scope, $http) {

    init();

    $scope.registrationTypeSelection = function () {

        $scope.members = [];
        $scope.juniors = [];

        var selection = [];

        $('div.registration-type input:checked').each(function (index, element) {
            selection.push(element.value);
        });

        if (selection.length > 0) {
            $scope.membercount = 1;
            $scope.members = [];
            $scope.members.push(1);
            $('div.member').show();
            $('div.button').show();
        }

        if (selection.indexOf("Guardian") != -1) {
            $scope.juniorcount = 1;
            $scope.juniors = [];
            $scope.juniors.push(1);
            $('div.junior').show();
            if (selection.length == 1) {
                $('div.guardian-count').show();
            }
            $('div.junior-count').show();
        }

        if (selection.indexOf("Other") != -1) {
            $('div.other').show();
        }
    };

    function init() {

        $('div#NewMember').find('div.reg-form').hide();
        $('p#error').hide();

        if (invalidCaptcha) {
            _setupCaptcha();
        }

        _setupSeasons();
    }

    function _setupCaptcha() {
        $.ajax({
            url: 'http://www.google.com/recaptcha/api/js/recaptcha_ajax.js',
            dataType: 'script',
            success: function (result) {
                _createCaptcha();
                newCaptcha = true;
            },
            error: function (xmlhttprequest, status, error) {
                $('#captcha').html('Cannot create captcha');
            }
        });
    }

    function _createCaptcha() {
        Recaptcha.create("6LfU2fASAAAAAIbgxLxe3BjwRXA6xEbjCVq7iJke",
            "captcha",
            {
                theme: "clean",
                callback: Recaptcha.focus_response_field
            }
        );
    }

    function _setupSeasons() {

        $.ajax({
            url: '/api/club/configuration/seasons',
            type: 'GET',
            success: function (data) {
                $scope.seasons = data;
                $scope.season = $scope.seasons[0].Text;
            },
            error: function (jqXhr, textStatus, errorThrown) {
                alert("Error '" + jqXhr.status + "' (textStatus: '" + textStatus + "', errorThrown: '" + errorThrown + "')");
            }
        });
    }
});