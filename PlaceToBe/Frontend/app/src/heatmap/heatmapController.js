'use strict';

/**
 * @ngdoc function
 * @name frontendApp.controller:MainCtrl
 * @description
 * # MainCtrl
 * Controller of the frontendApp
 */
angular.module('placeToBe')
  .controller('heatmapController', function ($scope, $http, $resource, loginService) {


    $scope.query = {
      place: {},
      startDate: new Date(),
      startHour: 18
    };
    //$scope.eventSourceUrl = "https://placetobe-koeln.azurewebsites.net/api/event";
    var BASE_URL = "http://192.168.125.136:18172/api";

    /**
     * fetchEvents calls the backend passed to the directive and then passes them to a callback
     * @param city
     * @param time
     */
    $scope.fetchEvents = function (query) {

      $http.get(buildEventQueryUrl(query.place, query.startDate, query.startHour))
        .success(function (data, status, headers, config) {

          //place the data from the server into a variable and make the heatmap visible
          $scope.heatmapData = data;


        });
    };

    var buildEventQueryUrl = function(city, startDate, hour){
      return BASE_URL + "/event/filter/" + city.place_id + "/" + startDate.getFullYear() + "/" + (startDate.getMonth()+1) + "/" + startDate.getDate() + "/" + hour
    };

    var City = $resource(BASE_URL + '/city');
    var cities  = City.query({}, function() {
      $scope.cities = cities;
    });




  });
