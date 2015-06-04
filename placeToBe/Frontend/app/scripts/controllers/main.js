'use strict';

/**
 * @ngdoc function
 * @name frontendApp.controller:MainCtrl
 * @description
 * # MainCtrl
 * Controller of the frontendApp
 */
angular.module('frontendApp')
  .controller('MainCtrl', function ($scope, heatmapService) {
    var geocoder = new google.maps.Geocoder();

    $scope.city = {
      name: "cologne"

    };

    var mapObj = null;

    //once our map is initialized, we position it.
    $scope.$on('mapInitialized', function(event, map) {
      mapObj = map;
      geocoder.geocode({
        address: $scope.city.name
      }, function(results,status){
        if (status == google.maps.GeocoderStatus.OK) {
          map.setCenter(results[0].geometry.location);
          map.fitBounds(results[0].geometry.bounds);

        }
      })
    });

    $scope.drawEvents = function(){
      heatmapService.drawEvents("foo", "bar",mapObj, function(events){
        console.log(events);
      });
    }

  });
