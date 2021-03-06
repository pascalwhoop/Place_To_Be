'use strict';

/**
 * @ngdoc function
 * @name placeToBe.controller:MainCtrl
 * @description
 * # MainCtrl
 * Controller of the frontendApp
 */
angular.module('placeToBe')
  .controller('heatmapController', function ($mdDialog, $rootScope, $scope, $location, $http, $resource, toastNotifyService, eventService, loginService, configService) {
    $scope.loginService = loginService;

    $scope.mapStyles = configService.MAPS_STYLE_ARRAY;

    //default values
    $scope.query = {
      place: {},
      startDate: new Date(),
      startHour: 18
    };
    $scope.eventData = [];


    $scope.fetchEvents = function (query) {
      if (!query.place || !query.place.place_id || !query.startDate || !query.startHour) return;
      eventService.fetchEvents(query)
        .success(function (data, status, headers, config) {
          //place the data from the server into a variable and make the heatmap visible
          $scope.heatmapData = data;
        })
        .error(function (data, status) {
          toastNotifyService.showNotifyToast(configService.STRINGS.backend_error);
        })
    };

    $scope.mapClick = function (event) {
      $rootScope.$emit('serverCallStart');
      eventService.fetchEventDetailsForLocation(event, $scope.query)
        .success(function (data) {
          $scope.eventData = data;
        })
        .error(function () {
          toastNotifyService.showNotifyToast(configService.STRINGS.backend_error);
        })
    };


    $scope.showPickDate = function (ev) {
      $mdDialog.show({
        controller: datePickerController,
        templateUrl: 'src/util/dateDialog.html',
        parent: angular.element(document.body),
        targetEvent: ev,
      }).then(function (answer) {
        $scope.query.startDate = answer;
        $scope.fetchEvents($scope.query);
      }, function () {

      });
    };

    var datePickerController = function ($scope, $mdDialog) {
      $scope.hide = function () {
        $mdDialog.hide();
      };
      $scope.answer = function (answer) {
        $mdDialog.hide(answer);
      };
    };


    var City = $resource(configService.BASE_URL + '/city');
    var cities = City.query({}, function () {
      $scope.cities = cities;
    });

    var initialize = function () {
      var lastQuery = eventService.getLastQuery();
      if (lastQuery) {
        $scope.query = lastQuery;
        $scope.fetchEvents(lastQuery);
      }
    };
    initialize();
  })


/**
 * EVENT SERVICE
 */
  .factory('eventService', function ($http, $rootScope, configService) {

    var BASE_URL = configService.BASE_URL;

    var fetchEvents = function (query) {
      $rootScope.$emit('serverCallStart');
      return $http.get(buildEventQueryUrl(query.place, query.startDate, query.startHour), {cache: true})
        .success(function () {
          //place the data from the server into a variable and make the heatmap visible
          localStorage.setItem("ptb_lastQuery", JSON.stringify(query));
          $rootScope.$emit('serverCallEnd');
        });
    };

    var fetchEventDetailsForLocation = function (clickEvent, query) {
      $rootScope.$emit('serverCallStart');
      return $http.get(buildEventClickQueryUrl(clickEvent.latLng, query))
        .success(function () {
          $rootScope.$emit('serverCallEnd');
        })
        .error(function () {
          $rootScope.$emit('serverCallEnd');
        })
    };

    /**
     * gets an event from the backend based on its ID
     * @param eventId
     */
    var getEventById = function (eventId) {
      $rootScope.$emit('serverCallStart');
      return $http.get(BASE_URL + '/event/' + eventId, {cache: true}).success(function (data) {
        $rootScope.$emit('serverCallEnd');
      })
        .error(function () {
          $rootScope.$emit('serverCallEnd');
        })
    };

    /**
     * wraps any call to a backend that returns a promise with a servercall start & end event on the rootScope
     * @param serverCall
     * @param callback
     */
    var performServerCall = function (serverCall, callback) {
      $rootScope.$emit('serverCallStart');
      return serverCall().then(function () {
        $rootScope.$emit('serverCallEnd');
      }, function () {
        $rootScope.$emit('serverCallEnd');
      })

    };

    var buildEventQueryUrl = function (city, startDate, hour) {
      return BASE_URL + "/event/filter/" + city.place_id + "/" + startDate.getFullYear() + "/" + (startDate.getMonth() + 1) + "/" + startDate.getDate() + "/" + hour
    };

    var buildEventClickQueryUrl = function (latLng, query) {
      var sd = query.startDate;
      //TODO fix Radius CONST value
      return BASE_URL + "/event/filter/" + latLng.lat() + "/" + latLng.lng() + "/" + 3000 + "/" + sd.getFullYear() + "/" + (sd.getMonth() + 1) + "/" + sd.getDate() + "/" + query.startHour
    };

    var getLastQuery = function () {
      //set last query as current query
      var localStorageQuery = localStorage.getItem("ptb_lastQuery");
      if (localStorageQuery) {
        var query = JSON.parse(localStorageQuery);
        query.startDate = new Date(query.startDate);
        return query;
      }
    };

    return {
      fetchEvents: fetchEvents,
      fetchEventDetailsForLocation: fetchEventDetailsForLocation,
      getLastQuery: getLastQuery,
      getEventById: getEventById
    }
  });
