'use strict';

angular.module('placeToBe')
  .controller('statisticController', function ($scope, $resource, $filter, $mdDialog, loginService, toastNotifyService, configService, eventService, ngTableParams) {
    $scope.loginService = loginService;
    //default values
    $scope.query = {
      place: {},
      startDate: new Date(),
      startHour: 18
    };

    $scope.fetchEvents = function (query) {
      if(!query.place || !query.place.place_id || !query.startDate || !query.startHour) return;
      eventService.fetchEvents(query)
        .success(function (data) {
          //place the data from the server into a variable and make the heatmap visible
          $scope.eventData = data;
          $scope.tableParams.reload();
        })
        .error(function(data, status){
          toastNotifyService.showNotifyToast(configService.STRINGS.backend_error);
        })
    };


    var setupTable = function(){
      $scope.tableParams = new ngTableParams({
        page: 1,            // show first page
        count: 10,
        sorting: {
          name: 'asc'     // initial sorting
        }
      }, {
        total: $scope.eventData.length, // length of data
        getData: function ($defer, params) {
          var orderedData = params.sorting() ?
            $filter('orderBy')($scope.eventData, params.orderBy()) : $scope.eventData;
          $defer.resolve(orderedData);
        }
      });
    };


    var initialize = function () {

      var City = $resource(configService.BASE_URL + '/city');
      var cities  = City.query({}, function() {
        $scope.cities = cities;
      });

      var lastQuery = eventService.getLastQuery();
      if (lastQuery) {
        $scope.query = lastQuery;
        eventService.fetchEvents($scope.query)
          .success(function(data, status){
            $scope.eventData = data;
            setupTable();
          })

      }
    };
    initialize()
  })


  .directive("statDetails", function ($mdDialog) {
    return {
      link: function (scope, element, attr) {
        scope.showStatDetailsDialog = function () {
          $mdDialog.show({
            controller: statDetailsController,
            templateUrl: 'src/statistic/statDetailsTemplate.html',
            parent: angular.element(document.body)
          })
        };
        function statDetailsController($scope) {
          $scope.cancel = function () {
            $mdDialog.cancel();
          }
        }
      },
      template: "<md-button class='md-raised' ng-click='showStatDetailsDialog()'>Details</md-button>",
      restrict: 'E'
    }
  });
