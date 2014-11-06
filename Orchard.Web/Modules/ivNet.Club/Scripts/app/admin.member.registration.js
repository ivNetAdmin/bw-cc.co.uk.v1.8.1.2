var ivNetAdminMemberRegistration = angular.module("ivNet.Admin.Member.Registration.App", ['ngResource', 'trNgGrid'])
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
             returnHtml = returnHtml + guardian.Email + ", " + guardian.Telephone;
         });
         

         return returnHtml;
     };
 });


ivNetAdminMemberRegistration.factory('adminMemberRegistration', function ($resource) {
    return $resource('/api/club/admin/registration/:id', null,
    {
        'update': { method: 'PUT' }
    });
});

ivNetAdminMemberRegistration.controller('AdminMemberController', function ($scope, adminMemberRegistration) {
    adminMemberRegistration.query(
       function (data) {
           $scope.myItems = data;
       },
       function (error) {
           alert(error);
       });

    $scope.activateJunior = function (item) {

        $('div#AdminMemberRegistration table tr').each(function (index, tr) {
            if (tr.children[0].innerText == item.JuniorId) {
               
                item.IsVetted = $(tr).find('td[field-name="IsVetted"]').find('input:checked').length;

                adminMemberRegistration.update({ id: item.JuniorId }, item,
                    function() {
                        window.location.reload();
                    },
                    function(error) {
                        alert(error.data);
                    }
                );
            }
        });
    };
});