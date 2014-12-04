var ivNetNewMember = angular.module("Membership.New.Registration.App", ['ui.event']);
var invalidCaptcha = true; // debug : false
ivNetNewMember.controller('MembershipNewRegistrationController', function ($scope, $http) {

    init();

    $scope.registrationTypeSelection = function () {
        $('div.guardian-count').hide();
        $('div.junior-count').hide();
        $('div.button').hide();
        $('div.captcha').hide();
        $('div.other').hide();

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
            $('div.captcha').show();
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

    $scope.adjustMemberCount = function () {
        $scope.members = [];
        for (var i = 0; i < $scope.membercount; i++) {
            $scope.members.push(i);
        }
    };

    $scope.adjustJuniorCount = function () {
        $scope.juniors = [];
        for (var i = 0; i < $scope.juniorcount; i++) {
            $scope.juniors.push(i);
        }
    };

    $scope.blurSurnameCallback = function (evt) {
        if (evt.target.name.substr(0, 6) == "Junior") {
            _checkJuniorNameDuplicates(evt);
        }      
    };

    $scope.blurEmailCallback = function (evt) {
        if (evt.target.value.length > 0) {
            _checkEmailDuplicates(evt.target.value);
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

    $('form#newMemberForm').submit(function () {

        // do captcha check
        _checkCaptcha();
    });

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

    function _checkCaptcha() {
        if (invalidCaptcha) {
            event.preventDefault();

            $('input[name="Challenge"]').val(Recaptcha.get_challenge());
            $('input[name="Response"]').val(Recaptcha.get_response());

            $.ajax({
                url: '/api/club/captcha',
                type: 'POST',
                data: { captchachallenge: $('input#recaptcha_challenge_field').val(), captcharesponse: $('input#recaptcha_response_field').val() },
                success: function (success) {
                  
                    if (!success) {
                        $('p#error').show();
                        _createCaptcha();
                    } else {
                        invalidCaptcha = false;
                        $('p#error').hide();
                       // $('form#newMemberForm').submit();
                    }
                },
                error: function (jqXhr, textStatus, errorThrown) {
                    alert("Error '" + jqXhr.status + "' (textStatus: '" + textStatus + "', errorThrown: '" + errorThrown + "')");
                }
            });
        }
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

    function _checkJuniorNameDuplicates(evt) {
        var surnameField = evt.target.name;
        var firstnameField = surnameField.replace("Surname", "Firstname");
        var errorField = surnameField.replace("Surname", "NameCheck");
        var firstname = ($('input[name="' + firstnameField + '"]').val());
        if (firstname.length == 0) {
            $('div#' + errorField + ' p.message').html("You must enter a Firstname before a Surname.");
            $('div#' + errorField + ' p.message').show();
            $('input[name="' + surnameField + '"]').val('');
        } else {
            var surname = $('input[name="' + surnameField + '"]').val();

            $.ajax({
                url: '/api/club/member/' + surname + firstname + '/junior-key',
                type: 'GET',
                success: function (data) {
                    if (data.length > 0) {
                        $('div#' + errorField + ' p.message').html(data);
                        $('div#' + errorField + ' p.message').show();
                    } else {
                        $('div#' + errorField + ' p.message').hide();
                    }
                },
                error: function (jqXhr, textStatus, errorThrown) {
                    alert("Error '" + jqXhr.status + "' (textStatus: '" + textStatus + "', errorThrown: '" + errorThrown + "')");
                }
            });
        }
    }

    function _checkEmailDuplicates(email) {

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