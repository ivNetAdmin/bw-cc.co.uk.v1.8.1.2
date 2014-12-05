var AdminMemberActivate = angular.module("Admin.Member.Activate.App", ['ngResource', 'trNgGrid'])
 .filter("dateField", function () {
     return function (combinedFieldValueUnused, item) {
         var d = item.Dob;

         var curr_date = d.substr(8, 2);
         var curr_month = d.substr(5, 2);
         var curr_year = d.substr(0, 4);

         return curr_date + "/" + curr_month + "/" + curr_year;
     };
 }).filter("ageGroup", function () {
     return function (combinedFieldValueUnused, item) {
         if (item.MemberType == 1)
             return "Guradian";

         return "Junior";
     };
 }).filter("related", function () {
     return function (combinedFieldValueUnused, item) {
         var returnHtml = "";
         angular.forEach(item.RelatedMembeList, function (relation, index) {
             returnHtml = returnHtml + relation + " ";            
         });
         
         return returnHtml;
     };
 });

AdminMemberActivate.factory('adminMemberActivate', function ($resource) {
    return $resource('/api/club/adminmember/:id/:type', null,
    {
       
    });
});

AdminMemberActivate.factory('adminMemberActivateUpdate', function ($resource) {
    return $resource('/api/club/adminmember/:id', null,
    {
        'update': { method: 'PUT' }
    });
});

AdminMemberActivate.controller('AdminMemberController', function ($scope, adminMemberActivate, adminMemberActivateUpdate) {

    init();  

    $scope.activateJunior = function (item) {

        adminMemberActivateUpdate.update({ id: item.MemberId }, { MemberType: item.MemberType },
            function() {
                window.location.reload();
            },
            function(error) {
                alert(error.data.Message + ' [' + error.data.MessageDetail + ']');
            }
        );

        //$('div#AdminMemberActivate table tr').each(function (index, tr) {
        //    if (tr.children[0].innerText == item.JuniorId) {

        //        item.IsVetted = $(tr).find('td[field-name="IsVetted"]').find('input:checked').length;

        //        adminMemberActivateUpdate.update({ id: item.JuniorId }, item,
        //            function() {
        //                window.location.reload();
        //            },
        //            function(error) {
        //                alert(error.data.Message + ' [' + error.data.MessageDetail + ']');
        //            }
        //        );
        //    }
        //});
    };

    function init() {
        adminMemberActivate.query({ id: "activate", type: "list" },
     function (data) {
         $scope.myItems = data;
     },
     function (error) {
         alert(error.data.Message + ' [' + error.data.MessageDetail + ']');
     });
    }
});