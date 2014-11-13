var ivNetMemberRegistration = angular.module("ivNet.Member.Registration.App", ['ui.event']);
var invalidCaptcha = true; // debug : false
ivNetMemberRegistration.controller('RegistrationController', function($scope, $http) {

    init();

    function init() {

        $('div#NewMemberRegistration').find('div.reg-form').hide();
        $('p#error').hide();

        if (invalidCaptcha) {
            setupCaptcha();
        }

        setupSeasons();
    }

    $scope.registrationTypeSelection = function() {

        $scope.members = [];
        $scope.juniors = [];

        var selection = [];

        $('div.registration-type input:checked').each(function(index, element) {
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

    $scope.adjustJuniorCount = function() {
        $scope.juniors = [];
        for (var i = 0; i < $scope.juniorcount; i++) {
            $scope.juniors.push(i);
        }
    };

    $scope.adjustMemberCount = function() {
        $scope.members = [];
        for (var i = 0; i < $scope.membercount; i++) {
            $scope.members.push(i);
        }
    };

    $scope.blurEmailCallback = function (evt) {
        if (evt.target.value.length > 0) {
            checkDuplicates(evt.target.value);
        }
    };

    $scope.blurSurnameCallback = function (evt) {
        var junior = evt.target.name.substr(0, 6) == "Junior";
        alert(junior);
    };

    $('form#newMemberForm').submit(function() {

        // do captcha check
        checkCaptcha();
    });

    function setupCaptcha() {
        $.ajax({
            url: 'http://www.google.com/recaptcha/api/js/recaptcha_ajax.js',
            dataType: 'script',
            success: function(result) {
                createCaptcha();
                newCaptcha = true;
            },
            error: function(xmlhttprequest, status, error) {
                $('#captcha').html('Cannot create captcha');
            }
        });
    }

    function createCaptcha() {
        Recaptcha.create("6LfU2fASAAAAAIbgxLxe3BjwRXA6xEbjCVq7iJke",
            "captcha",
            {
                theme: "clean",
                callback: Recaptcha.focus_response_field
            }
        );
    }

    function checkCaptcha() {
        if (invalidCaptcha) {
            event.preventDefault();

            $('input[name="Challenge"]').val(Recaptcha.get_challenge());
            $('input[name="Response"]').val(Recaptcha.get_response());

            $.ajax({
                url: '/club/member/validate',
                type: 'POST',
                data: $('form#newMemberForm').serialize(),
                success: function(data) {
                    if (!data.Success) {

                        $('p#error').show();
                        createCaptcha();
                    } else {
                        invalidCaptcha = false;
                        $('p#error').hide();
                        $('form#newMemberForm').submit();
                    }
                },
                error: function(jqXhr, textStatus, errorThrown) {
                    alert("Error '" + jqXhr.status + "' (textStatus: '" + textStatus + "', errorThrown: '" + errorThrown + "')");
                }
            });
        }
    }

    function checkDuplicates(email) {

        $.ajax({
            url: '/api/club/members/' + email + '/email',
            type: 'GET',
            success: function(data) {
                if (data.length > 0) {

                    $('div#dupError').find('p').html(data);
                    $('div#dupError').show();
                    $('input#singlebutton').hide();

                } else {
                    $('div#dupError').hide();
                    $('input#singlebutton').show();
                }
            },
            error: function(jqXhr, textStatus, errorThrown) {
                alert("Error '" + jqXhr.status + "' (textStatus: '" + textStatus + "', errorThrown: '" + errorThrown + "')");
            }
        });
    }

    function setupSeasons() {

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