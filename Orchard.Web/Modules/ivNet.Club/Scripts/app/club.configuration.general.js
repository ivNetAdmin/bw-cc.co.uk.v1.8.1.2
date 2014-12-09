var ivNetClubConfiguration = angular.module('Club.Configuration.General.App', ['ngResource', 'trNgGrid'])
    .filter("dateField", function() {
        return function(combinedFieldValueUnused, item) {
            var d = item.Date;
         
            var curr_date = d.substr(8, 2);
            var curr_month = d.substr(5, 2);
            var curr_year = d.substr(0, 4);

            return curr_year + "-" + curr_month + "-" + curr_date;
        };
    });

ivNetClubConfiguration.factory('configuration', function ($resource) {
    return $resource('/api/club/admin/configuration/:id', null,
    {
        'update': { method: 'PUT' }
    });
});

ivNetClubConfiguration.controller('ConfigurationController', function($scope, configuration) {
    configuration.query(
        function (data) {
            $scope.myItems = data;
        },
        function(error) {
            alert(error.data.Message + ' [' + error.data.MessageDetail + ']');
        });

    $scope.saveItem = function(item) {

        var newItem = item.Id == 0;

        $('div#ClubConfiguration table tr').each(function(index, tr) {
            if (tr.children[0].innerText == item.Id) {
                item.Name = $(tr).find('td[field-name="Name"]').find('input').val();
                item.ItemGroup = $(tr).find('td[field-name="ItemGroup"]').find('input').val();
                item.Text = $(tr).find('td[field-name="Text"]').find('input').val();
                item.Date = $(tr).find('td[field-name="Date"]').find('input').val();
                item.Number = $(tr).find('td[field-name="Number"]').find('input').val();
                item.IsActive = $(tr).find('td[field-name="IsActive"]').find('input:checked').length;

                configuration.update({ id: item.Id }, item,
                    function() {
                        if (newItem) {                            
                            window.location.reload();
                        } else {
                            alert("Saved OK");
                        }
                    },
                    function(error) {
                        alert(error.data.Message + ' [' + error.data.MessageDetail + ']');
                    }
                );
            }
        });
    };
});
