'use strict';

/**
 * @ngdoc directive
 * @name frontendApp.directive:heatmapDirective
 * @description
 * # heatmapDirective
 */
angular.module('frontendApp')
  .directive('placetobeHeatmap', function ($http) {
    return {
      //here we place the logic of the directive (link function)
      link: function (scope, element, attr) {
        var geocoder = new google.maps.Geocoder();
        var mapObj = null;
        var TILE_SIZE = 256;

        scope.$on('mapInitialized', function (event, map) {
          mapObj = map;
          geocoder.geocode({
            address: scope.city.name
          }, function (results, status) {
            if (status == google.maps.GeocoderStatus.OK) {
              map.setCenter(results[0].geometry.location);
              map.fitBounds(results[0].geometry.bounds);
            }
          });

          fetchEvents(scope.city.name, scope.time, handleEvents);

        });

        /**
         * fetchEvents calls the backend passed to the directive and then passes them to a callback
         * @param city
         * @param time
         */
        var fetchEvents = function (city, time, callback) {
          $http.get(scope.eventSource + "/filter/" + city + "/" + time)
            .success(function (data, status, headers, config) {

              callback(data);
            }); //for now the cologne/now is not important, we just want to show a map
        };

        var handleEvents = function (events) {
          var heatmapData = [];
          events.forEach(function (element) {

              addPointsForEachGuest(element, heatmapData);
            }

            /*var latLng = new google.maps.LatLng(element.locationCoordinates.coordinates[0], element.locationCoordinates.coordinates[1]);

             heatmapData.push({
             location: latLng,
             weight: element.attending_count
             });*/
          );

          heatmapData = new google.maps.MVCArray(heatmapData);
          var heatmap = new google.maps.visualization.HeatmapLayer({
            data: heatmapData,
            map: mapObj,
            radius: getNewRadius()
          });
          heatmap.setMap(mapObj);

          google.maps.event.addListener(mapObj, 'zoom_changed', function () {
            heatmap.setOptions({radius:getNewRadius()});
          });
        };

        var addPointsForEachGuest = function (event, heatmapData) {
          var maxEventRadius = 0.00001 * Math.pow(event.attending_count, 0.66666);
          for (var i = 0; i < Math.ceil(event.attending_count/100); i++) {
            var heatPointDistance = maxEventRadius * Math.random();
            var angle = Math.random() * Math.PI * 2;
            var a = event.locationCoordinates.coordinates[0] + Math.cos(angle) * heatPointDistance;
            var b = event.locationCoordinates.coordinates[1] + Math.sin(angle) * heatPointDistance;
            var latLng = new google.maps.LatLng(a, b);


            heatmapData.push(latLng);
          }
        };

        //Mercator --BEGIN--   code from Google Maps Examples

        function bound(value, opt_min, opt_max) {
          if (opt_min !== null) value = Math.max(value, opt_min);
          if (opt_max !== null) value = Math.min(value, opt_max);
          return value;
        }

        function degreesToRadians(deg) {
          return deg * (Math.PI / 180);
        }

        function radiansToDegrees(rad) {
          return rad / (Math.PI / 180);
        }

        function MercatorProjection() {
          this.pixelOrigin_ = new google.maps.Point(TILE_SIZE / 2,
            TILE_SIZE / 2);
          this.pixelsPerLonDegree_ = TILE_SIZE / 360;
          this.pixelsPerLonRadian_ = TILE_SIZE / (2 * Math.PI);
        }

        MercatorProjection.prototype.fromLatLngToPoint = function (latLng,
                                                                   opt_point) {
          var me = this;
          var point = opt_point || new google.maps.Point(0, 0);
          var origin = me.pixelOrigin_;

          point.x = origin.x + latLng.lng() * me.pixelsPerLonDegree_;

          // NOTE(appleton): Truncating to 0.9999 effectively limits latitude to
          // 89.189.  This is about a third of a tile past the edge of the world
          // tile.
          var siny = bound(Math.sin(degreesToRadians(latLng.lat())), -0.9999,
            0.9999);
          point.y = origin.y + 0.5 * Math.log((1 + siny) / (1 - siny)) * -me.pixelsPerLonRadian_;
          return point;
        };

        MercatorProjection.prototype.fromPointToLatLng = function (point) {
          var me = this;
          var origin = me.pixelOrigin_;
          var lng = (point.x - origin.x) / me.pixelsPerLonDegree_;
          var latRadians = (point.y - origin.y) / -me.pixelsPerLonRadian_;
          var lat = radiansToDegrees(2 * Math.atan(Math.exp(latRadians)) - Math.PI / 2);
          return new google.maps.LatLng(lat, lng);
        };

        //Mercator --END--


        // function from   http://jsbin.com/rorecuce/1/edit?html,output

        function getNewRadius() { //TODO calculate event radius based on people participating

          var desiredRadiusPerPointInMeters = 50;
          var numTiles = 1 << mapObj.getZoom();
          var center = mapObj.getCenter();
          var moved = google.maps.geometry.spherical.computeOffset(center, 10000, 90);
          /*1000 meters to the right*/
          var projection = new MercatorProjection();
          var initCoord = projection.fromLatLngToPoint(center);
          var endCoord = projection.fromLatLngToPoint(moved);
          var initPoint = new google.maps.Point(
            initCoord.x * numTiles,
            initCoord.y * numTiles);
          var endPoint = new google.maps.Point(
            endCoord.x * numTiles,
            endCoord.y * numTiles);
          var pixelsPerMeter = (Math.abs(initPoint.x - endPoint.x)) / 10000.0;
          var totalPixelSize = Math.floor(desiredRadiusPerPointInMeters * pixelsPerMeter);
          console.log(totalPixelSize);
          return totalPixelSize;

        }

      },

      /** TODO we need to do this for every event currently in our heatmap array
       * google.maps.event.addListener(map, 'zoom_changed', function () {
              heatmap.setOptions({radius:getNewRadius()});
          });
       */


      //this is the HTML template for the directive
      templateUrl: 'scripts/directives/heatmapdirective.html',
      //we set it so only element tags are relevant
      restrict: 'E',
      //linking the attribute "city" value to the scope.city variable (maybe others too)
      scope: {
        city: "=",
        time: "=",
        eventSource: "="
      }
    };
  })
;
