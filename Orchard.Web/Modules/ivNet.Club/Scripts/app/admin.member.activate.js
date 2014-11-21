﻿var ivNetAdminMemberActivate = angular.module("ivNet.Admin.Member.Activate.App", ['ngResource', 'trNgGrid'])
 .filter("dateField", function () {
     return function (combinedFieldValueUnused, item) {
         var d = item.Dob;
         
         var curr_date = d.substr(8, 2);
         var curr_month = d.substr(5, 2);
         var curr_year = d.substr(0, 4);

         return curr_date + "/" + curr_month + "/" + curr_year;
     };
 }).filter("guardiansField", function () {
     return function (combinedFieldValueUnused, junior) {
         var returnHtml = "";
         angular.forEach(junior.Guardians, function (guardian, index) {
             returnHtml = returnHtml + guardian.Firstname + " " + guardian.Surname + ", ";
             returnHtml = returnHtml + guardian.Email + ", " + guardian.Telephone + ". ";
         });
         
         return returnHtml;
     };
 });


ivNetAdminMemberActivate.factory('adminMemberActivate', function ($resource) {
    return $resource('/api/club/admin/member/:id/list', null,
    {
       
    });
});

ivNetAdminMemberActivate.factory('adminMemberActivateUpdate', function ($resource) {
    return $resource('/api/club/admin/member/:id', null,
    {
        'update': { method: 'PUT' }
    });
});

ivNetAdminMemberActivate.controller('AdminMemberController', function ($scope, adminMemberActivate, adminMemberActivateUpdate) {
    adminMemberActivate.query({ id: "new" },
       function (data) {
           $scope.myItems = data;
       },
       function (error) {
           alert(error.data.Message + ' [' + error.data.MessageDetail + ']');
       });

    $scope.activateJunior = function (item) {

        $('div#AdminMemberActivate table tr').each(function (index, tr) {
            if (tr.children[0].innerText == item.JuniorId) {
               
                item.IsVetted = $(tr).find('td[field-name="IsVetted"]').find('input:checked').length;

                adminMemberActivateUpdate.update({ id: item.JuniorId }, item,
                    function() {
                        window.location.reload();
                    },
                    function(error) {
                        alert(error.data.Message + ' [' + error.data.MessageDetail + ']');
                    }
                );
            }
        });
    };
});