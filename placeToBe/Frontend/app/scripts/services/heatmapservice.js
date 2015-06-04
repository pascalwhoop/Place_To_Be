'use strict';

/**
 * @ngdoc service
 * @name frontendApp.heatmapService
 * @description
 * # heatmapService
 * Service in the frontendApp.
 */
angular.module('frontendApp')
  .service('heatmapService', function ($http) {

    var backendUrl = "https://placetobe-koeln.azurewebsites.net/api/event"


    this.drawEvents= function(city, time, map, callback){
      $http.get(backendUrl + "/filter/cologne/now")
        .success(function(data, status, headers, config){
          callback(data);

          var heatmapData = [];
          data.forEach(function(element){
            var latLng = new google.maps.LatLng(element.locationCoordinates.coordinates[0], element.locationCoordinates.coordinates[1]);

            heatmapData.push({
              location: latLng,
              weight: element.attending_count
            });

          });


          heatmapData = new google.maps.MVCArray(heatmapData);

          var heatmap = new google.maps.visualization.HeatmapLayer({
            data: heatmapData,
            map: map
          })

          heatmap.setMap(map);
        }); //for now the cologne/now is not important, we just want to show a map
    }
  });
