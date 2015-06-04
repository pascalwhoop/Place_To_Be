'use strict';

/**
 * @ngdoc directive
 * @name frontendApp.directive:heatmapDirective
 * @description
 * # heatmapDirective
 */
angular.module('frontendApp')
  .directive('heatmapDirective', function () {
    return {
      templateUrl: 'scripts/directives/heatmapdirective.html',
      restrict: 'E',
      scope: {
        city: "="
      }
    };
  });
