'use strict';

/**
 * @ngdoc function
 * @name frontendApp.controller:MainCtrl
 * @description
 * # MainCtrl
 * Controller of the frontendApp
 */
angular.module('placeToBe')
  .controller('heatmapController', function ($rootScope, $scope, $location, $http, $resource, loginService, configService) {
    //forbid users to see this page if they aren't logged in
    if(!loginService.getLoginStateSync()) $location.path('/');
    $scope.loginService = loginService;

    $scope.mapStyles = configService.MAPS_STYLE_ARRAY;

    $scope.query = {
      place: {},
      startDate: new Date(),
      startHour: 18
    };
    $scope.eventData = [];

    var BASE_URL = configService.BASE_URL;
    //var BASE_URL = "http://192.168.125.136:18172/api";

    /**
     * fetchEvents calls the backend passed to the directive and then passes them to a callback
     * @param city
     * @param time
     */
    $scope.fetchEvents = function (query) {
      if(!query.place || !query.place.place_id || !query.startDate || !query.startHour) return;
      $rootScope.$emit('serverCallStart');

      $http.get(buildEventQueryUrl(query.place, query.startDate, query.startHour))
        .success(function (data, status, headers, config) {
          //place the data from the server into a variable and make the heatmap visible
          $scope.heatmapData = data;
          localStorage.setItem("ptb_lastQuery", JSON.stringify(query));
          $rootScope.$emit('serverCallEnd');
        });
    };

    $scope.mapClick = function(event){
      $http.get(buildEventClickQueryUrl(event.latLng))
        .success(function(data, status, headers, config){
          console.log("fetched events for location");
          $scope.eventData = data;
        })
    };

    var buildEventQueryUrl = function(city, startDate, hour){
      return BASE_URL + "/event/filter/" + city.place_id + "/" + startDate.getFullYear() + "/" + (startDate.getMonth()+1) + "/" + startDate.getDate() + "/" + hour
    };

    var buildEventClickQueryUrl = function(latLng){
      //TODO fix Radius CONST value
      var sd = $scope.query.startDate;
      return BASE_URL + "/event/filter/" + latLng.lat() + "/" + latLng.lng() + "/" + 3000 + "/" + sd.getFullYear() + "/" + (sd.getMonth()+1) + "/" + sd.getDate() + "/" + $scope.query.startHour
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
