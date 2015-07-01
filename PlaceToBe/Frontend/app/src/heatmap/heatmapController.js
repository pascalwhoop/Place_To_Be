'use strict';

/**
 * @ngdoc function
 * @name frontendApp.controller:MainCtrl
 * @description
 * # MainCtrl
 * Controller of the frontendApp
 */
angular.module('placeToBe')
  .controller('heatmapController', function ($scope, $http, $resource, loginService, configService) {

    $scope.loginService = loginService;

    $scope.loggedIn = loginService.getLoginState();

    $scope.query = {
      place: {},
      startDate: new Date(),
      startHour: 18
    };

    var BASE_URL = configService.BASE_URL;
    //var BASE_URL = "http://192.168.125.136:18172/api";

    /**
     * fetchEvents calls the backend passed to the directive and then passes them to a callback
     * @param city
     * @param time
     */
    $scope.fetchEvents = function (query) {
      if(!query.place || !query.place.place_id) return;

      $http.get(buildEventQueryUrl(query.place, query.startDate, query.startHour))
        .success(function (data, status, headers, config) {
          //place the data from the server into a variable and make the heatmap visible
          $scope.heatmapData = data;
          localStorage.setItem("ptb_lastQuery", JSON.stringify(query));
        });
    };

    var buildEventQueryUrl = function(city, startDate, hour){
      return BASE_URL + "/event/filter/" + city.place_id + "/" + startDate.getFullYear() + "/" + (startDate.getMonth()+1) + "/" + startDate.getDate() + "/" + hour
    };

    var City = $resource(BASE_URL + '/city');
    var cities  = City.query({}, function() {
      $scope.cities = cities;
    });

    //set last query as current query
    var localStorageQuery = localStorage.getItem("ptb_lastQuery");
    if(localStorageQuery) {
      $scope.query = JSON.parse(localStorageQuery);
      $scope.query.startDate = new Date($scope.query.startDate);
      $scope.fetchEvents($scope.query);
    }
  });
