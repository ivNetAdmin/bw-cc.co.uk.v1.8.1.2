﻿var ivNetShoppingCart = angular.module("ivNet.Member.Registration.App", []);
var invalidCaptcha = true;
ivNetShoppingCart.controller('RegistrationController', function ($scope, $http) {

    init();

    function init() {
        $('div#NewMemberRegistration').find('div.reg-form').hide();
        $('p#error').hide();

        $.ajax({
            url: 'http://www.google.com/recaptcha/api/js/recaptcha_ajax.js',
            dataType: 'script',
            success: function (result) {
                CreateCaptcha();
                newCaptcha = true;
            },
            error: function (xmlhttprequest, status, error) {
                $('#captcha').html('Cannot create captcha');
            }
        });
    }

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
    }

    $scope.adjustJuniorCount = function () {
        $scope.juniors = [];
        for (var i = 0; i < $scope.juniorcount; i++) {
            $scope.juniors.push(i);
        }
    };

    $scope.adjustMemberCount = function () {
        $scope.members = [];
        for (var i = 0; i < $scope.membercount; i++) {
            $scope.members.push(i);
        }
    };

    $('form#newMemberForm').submit(function (event) {

        if (invalidCaptcha) {
            event.preventDefault();

            $('input[name="Challenge"]').val(Recaptcha.get_challenge());
            $('input[name="Response"]').val(Recaptcha.get_response());

            $.ajax({
                url: '/club/member/validate',
                type: 'POST',
                data: $('form#newMemberForm').serialize(),
                success: function (data) {
                    if (!data.Success) {

                        $('p#error').show();
                        CreateCaptcha();
                    } else {
                        invalidCaptcha = false;
                        $('p#error').hide();
                        $('form#newMemberForm').submit();
                    }
                },
                error: function (jqXhr, textStatus, errorThrown) {
                    alert("Error '" + jqXhr.status + "' (textStatus: '" + textStatus + "', errorThrown: '" + errorThrown + "')");
                }
            });
        }
    });

    function CreateCaptcha() {
        Recaptcha.create("6LfU2fASAAAAAIbgxLxe3BjwRXA6xEbjCVq7iJke",
            "captcha",
            {
                theme: "clean",
                callback: Recaptcha.focus_response_field
            }
        );
    }
});