'use strict';

/**
 * @ngdoc directive
 * @name frontendApp.directive:heatmapDirective
 * @description
 * # heatmapDirective
 */
angular.module('placeToBe')
.directive( 'goClick', function ( $location ) {
  return function ( scope, element, attrs ) {
    var path;

    attrs.$observe( 'goClick', function (val) {
      path = val;
    });

    element.bind( 'click', function () {
      scope.$apply( function () {
        $location.path( path );
      });
    });
  };
});
