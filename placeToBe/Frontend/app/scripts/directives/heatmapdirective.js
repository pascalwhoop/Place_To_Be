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
        var heatmapLayer = null;


        /*VARIABLES THAT INFLUENCE THE RENDERING OF THE HEATMAP*/
        scope.SIZE_PER_PERSON = 0.00001; //in latitude/longitude 0.00001 ~ == 1m
        scope.PERSONS_PER_POINT = 40;
        scope.TILE_SIZE = 256;
        scope.METER_RADIUS_PER_POINT= 20;
        var EVENTS = [    {
          "name": "BEATS x BASS x COLOGNE Summer Opening mit VIRTUAL RIOT, KLANGFARBE ANTIFEIND u.a.",
          "attending_count": 771,
          "geoLocationCoordinates": {
            "type": "Point",
            "coordinates": [
              50.954742334517,
              6.9383665945917
            ]
          }
        }, {
          "name": "PIM & CEMS Human Kicker Cup + BBQ Party 2015",
          "attending_count": 172,
          "geoLocationCoordinates": {
            "type": "Point",
            "coordinates": [
              50.927673503974,
              6.9290317620486
            ]
          }
        },   {
          "name": "Freundschaftsspiel Deutschland : USA",
          "attending_count": 15000,
          "geoLocationCoordinates": {
            "type": "Point",
            "coordinates": [
              50.934202141613,
              6.8751399570981
            ]
          }
        }, {
          "name": "DER DIENSTAG // EINTRITT FREI & KEIN MVZ // (FSK 18)",
          "attending_count": 284,
          "geoLocationCoordinates": {
            "type": "Point",
            "coordinates": [
              50.9416001407,
              6.9402307300079
            ]
          }
        },   {
          "name": "4. Kettcar Grand Prix d'Odonien",
          "attending_count": 413,
          "geoLocationCoordinates": {
            "type": "Point",
            "coordinates": [
              50.954742334517,
              6.9383665945917
            ]
          }
        }];



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
           //MOCK data
          //callback(MOCK_EVENTS);
          $http.get(scope.eventSource + "/filter/" + city + "/" + time)
            .success(function (data, status, headers, config) {

              //callback(data.slice(0,20));
              EVENTS = data;
              callback(data);
            }); //for now the cologne/now is not important, we just want to show a map
        };

        scope.refreshHeatmap = function(){
            heatmapLayer.setMap(null);
            setHeatmapDataArrayAsLayer(getHeatmapDataArrayForEvents(EVENTS));
        };

        var handleEvents = function (events) {
          setHeatmapDataArrayAsLayer(getHeatmapDataArrayForEvents(events));

          google.maps.event.addListener(mapObj, 'zoom_changed', function () {
            heatmapLayer.setOptions({radius: getNewRadius()});
          });
        };

        var setHeatmapDataArrayAsLayer = function(heatmapData){
          heatmapData = new google.maps.MVCArray(heatmapData);
          heatmapLayer = new google.maps.visualization.HeatmapLayer({
            data: heatmapData,
            map: mapObj,
            radius: getNewRadius()
          });
          heatmapLayer.setMap(mapObj);
        };

        var getHeatmapDataArrayForEvents= function(events){
          var heatmapData = [];
          events.forEach(function (element) {
              addPointsForEachGuest(element, heatmapData);
            }
          );
          return heatmapData;
        };

        var addPointsForEachGuest = function (event, heatmapData) {
          if(!event.geoLocationCoordinates || !event.geoLocationCoordinates.coordinates) return;
          var maxEventRadius = scope.SIZE_PER_PERSON * Math.pow(event.attending_count, 0.66666);
          for (var i = 0; i < Math.ceil(event.attending_count / scope.PERSONS_PER_POINT); i++) {
            var heatPointDistance = maxEventRadius * Math.random();
            var angle = Math.random() * Math.PI * 2;
            var a = event.geoLocationCoordinates.coordinates[0] + Math.cos(angle) * heatPointDistance;
            var b = event.geoLocationCoordinates.coordinates[1] + Math.sin(angle) * heatPointDistance;
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
          this.pixelOrigin_ = new google.maps.Point(scope.TILE_SIZE / 2,
            scope.TILE_SIZE / 2);
          this.pixelsPerLonDegree_ = scope.TILE_SIZE / 360;
          this.pixelsPerLonRadian_ = scope.TILE_SIZE / (2 * Math.PI);
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
          return Math.floor(scope.METER_RADIUS_PER_POINT * pixelsPerMeter);

        }

      },

      /** TODO we need to do this for every event currently in our heatmap array
       * google.maps.event.addListener(map, 'zoom_changed', function () {
              heatmap.setOptions({radius:getNewRadius()});
          });
       */


      //this is the HTML template for the directive
      template: '<h2>Eventmap for <input type="text" ng-model="city.name" ng-disabled="true"/>' +
      '</h2>' +
      '<p>SIZE_PER_PERSON <input type="number" ng-model="SIZE_PER_PERSON"/></p>' +
      '<p>PERSONS_PER_POINT <input type="number" ng-model="PERSONS_PER_POINT"/></p>' +
      '<p>TILE_SIZE <input type="number" ng-model="TILE_SIZE"/></p>' +
      '<p>METER_RADIUS_PER_POINT <input type="number" ng-model="METER_RADIUS_PER_POINT"/></p>' +
      '<button ng-click="refreshHeatmap()" ng-disabled="!heatmapLayer">RefreshHeatmap</button>' +
      '<map center="city.center" style="height: 700px;"></map>',
      //we set it so only element tags are relevant
      restrict: 'E',
      //linking the attribute "city" value to the scope.city variable (maybe others too)
      scope: {
        city: "=",
        time: "=",
        eventSource: "="
      }
    };
  });
