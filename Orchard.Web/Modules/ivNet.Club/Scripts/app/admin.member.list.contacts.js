var ivNetAdminContactList = angular.module("Admin.Member.List.Contacts.App", ['ngResource', 'trNgGrid', 'ngCsv'])
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

ivNetAdminContactList.factory('adminPaginatedContactList', function ($resource) {
    return $resource('/api/club/admincontact/:id/:gridOptions', null,
    {
        'query': { method: 'GET', isArray: false },
    });
});


ivNetAdminContactList.controller('AdminContactListController', function ($scope, adminContactList, adminPaginatedContactList) {
   
    init();

    $scope.onServerSideItemsRequested = function (currentPage, pageItems, filterBy, filterByFields, orderBy, orderByReverse) {        

        $('#loading-indicator').show();

        if (filterBy == null) {
            filterBy = "";
        }

        if (orderBy == null) {
            orderBy = "";
        }

        var ageGroup = window.location.search.split('=')[1];

        adminPaginatedContactList.query({ CurrentPage: currentPage, OrderBy: orderBy, OrderByReverse: orderByReverse, PageItems: pageItems, FilterBy: filterBy, FilterByFields: angular.toJson(filterByFields), ageGroup: ageGroup },
            function (data) {
                $scope.contacts = data.contacts;
                $scope.itemsTotalCount = data.totalItems;
         
                $('#loading-indicator').hide();       
            },
        function (error) {
            $('#loading-indicator').hide();
            alert(error.data.Message + ' [' + error.data.MessageDetail + ']');
        });      
    };

    function init() {
   
        $scope.pageItemsCount = 20;
        $scope.itemsTotalCount = 0;

        //$('#loading-indicator').show();
        //adminContactList.query(
        //    function(data) {
        //        $scope.contacts = data.JuniorContacts;
        //        $('#loading-indicator').hide();

        //    },
        //    function(error) {
        //        alert(error.data.Message + ' [' + error.data.MessageDetail + ']');
        //    });

    }

});