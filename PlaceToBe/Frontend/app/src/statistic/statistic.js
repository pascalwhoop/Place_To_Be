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

  .directive("statDetails", function ($mdDialog, eventService) {
    return {
      link: function (scope, element, attr) {

        /**
         * fetch the data from the backend (or the supplied supplier) and then show the dialog
         * @param eventId
         */
        scope.showDetails = function (mouseEvent) {
              eventService.getEventById(scope.eventId).then(
                function(data, status){
                  scope.eventData = data;
                  showDialog(data, mouseEvent);
                })

        };

        /**
         * show the actual details dialog
         */
        var showDialog = function(data, mouseEvent){
          $mdDialog.show({
            controller: detailsController,
            templateUrl: 'src/details/detailView.html',
            parent: angular.element(document.body),
            targetEvent: mouseEvent,
            locals:{
              eventData: data
            }
          })
        };

        function detailsController($scope, $mdDialog, eventData) {
          $scope.event = eventData;
          $scope.hide = function () {
            $mdDialog.hide();
          };
          $scope.showFriendFb = function(url){
            if(url.length > 0)window.open(url, '_blank');
          }
        }
      },
      //this is the HTML template for the directive
      template: "<md-button ng-click='showDetails($event)' class='md-primary md-raised'>Details</md-button>",
      //we set it so only element tags are relevant
      restrict: 'E',
      //linking the attribute "city" value to the scope.city variable (maybe others too)
      scope: {
        eventId: "="
      }
    }
  });
