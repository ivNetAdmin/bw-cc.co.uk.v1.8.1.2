var ivNetClubConfiguration = angular.module('ivNet.Club.Configuration.App', ['ngResource', 'trNgGrid'])
    .filter("dateField", function() {
        return function(combinedFieldValueUnused, item) {
            var d = item.Date;

            var curr_month = d.substr(8, 2);
            var curr_date = d.substr(5, 2);
            var curr_year = d.substr(0, 4);

            //return curr_date + "/" + curr_month + "/" + curr_year;
            return curr_year + "-" + curr_month + "-" + curr_date;
        };
    });

ivNetClubConfiguration.factory('configuration', function ($resource) {
    return $resource('/api/club/configuration/:id', null,
    {
        'update': { method: 'PUT' }
    });
    //return $resource("/api/posts/:id");  
});

//ivNetClubConfiguration.factory("Items", function ($resource) {
//    return $resource("/api/club/configuration");
//    //return $resource("/api/posts/:id");  
//});

ivNetClubConfiguration.controller('ConfigurationController', function ($scope, configuration) {
    configuration.query(function (data) {
        $scope.myItems = data;
    });

    //configFactory.get({ id: 1 }, function (data) {
    //    $scope.post = data;
    //});

    $scope.saveItem = function (item) {

        var newItem = item.Id == 0;

        $('div#ClubConfiguration table tr').each(function(index,tr) {
            if (tr.children[0].innerText == item.Id) {            
               item.Name = $(tr).find('td[field-name="Name"]').find('input').val();
               item.Date = $(tr).find('td[field-name="Date"]').find('input').val();
               item.Number = $(tr).find('td[field-name="Number"]').find('input').val();

               configuration.update({ id: item.Id }, item,
                   function () {                   
                       if (newItem) {
                           alert("Added OK");
                           window.location.reload();
                       } else {
                           alert("Saved OK");
                       }
                   },
                   function(error) {
                       alert(error.data);
                   }
               );
            }
        });

       
        
    };
});
