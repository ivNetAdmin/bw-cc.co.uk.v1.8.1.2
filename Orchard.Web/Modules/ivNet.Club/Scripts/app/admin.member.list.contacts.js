var ivNetAdminContactList = angular.module("Admin.Member.List.Contacts.App", ['ngResource', 'trNgGrid'])
    .filter("dateField", function () {
        return function (combinedFieldValueUnused, item) {
            var d = item.Dob;

            var curr_date = d.substr(8, 2);
            var curr_month = d.substr(5, 2);
            var curr_year = d.substr(0, 4);

            return curr_date + "/" + curr_month + "/" + curr_year;
        };
    })
    .filter("editDateField", function () {
        return function (combinedFieldValueUnused, item) {
            var d = item.Dob;

            var curr_date = d.substr(8, 2);
            var curr_month = d.substr(5, 2);
            var curr_year = d.substr(0, 4);

            return curr_year + "-" + curr_month + "-" + curr_date;
        };
    });

ivNetAdminContactList.factory('adminContactList', function ($resource) {
    return $resource('/api/club/admincontact/:id', null,
    {
        'query': { method: 'GET', isArray: false },
    });
});

ivNetAdminContactList.controller('AdminContactListController', function ($scope, adminContactList) {
   
    init();

    function init() {
        $('div#contactDetail').hide();

        adminContactList.query(
            function(data) {
                $scope.members = data;
            },
            function(error) {
                alert(error.data.Message + ' [' + error.data.MessageDetail + ']');
            });

    }

});