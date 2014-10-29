var ivNetShoppingCart = angular.module("ivNet.Member.Registration.App", []);

ivNetShoppingCart.controller('RegistrationController', function($scope, $http) {

    init();

    function init() {
        $('div#NewMemberRegistration').find('div.reg-form').hide();
        //reg-form
        //$('div.guardian-count').hide();
        //$('div.junior-count').hide();

        //$('div.member').hide();
        //$('div.junior').hide();
        //$('div.officer').hide();

        //$('div.button').hide();

        //$('div.instruction').show();
        //$('div.registration-type').show();

        $scope.members = [];
        $scope.juniors = [];
    }

    $scope.registrationTypeSelection = function() {

        init();

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

        if (selection.indexOf("Guardian")!=-1) {
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


    //switch (element.value) {});
        //    case "Guardian":
        //        selection.push("guardian");

        //        //$scope.membercount = 1;
        //        //$scope.members = [];
        //        //$scope.members.push(1);

        //        $scope.juniorcount = 1;                   
        //        $scope.juniors = [];
        //        $scope.juniors.push(1);

        //        $('div.member-count').show();


        //        $('div.junior-count').show();

        //        break;
        //    default:
        //        $('div.member-count').hide();
        //       // $('div.name').show();
        //       // $('div.address').show();
        //       // $('div.contact').show();

        //        break;
        //}

        // $('div.junior').show();
        // $('div.button').show();

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

    
});
