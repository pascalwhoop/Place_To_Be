'use strict';

/**
 * @ngdoc filter
 * @name frontendApp.filter:range
 * @function
 * @description
 * # range
 * Filter in the frontendApp.
 */
angular.module('frontendApp')
  .filter('range', function () {
    return function(input, min, max) {
      min = parseInt(min); //Make string input int
      max = parseInt(max);
      for (var i=min; i<=max; i++)
        input.push(i);
      return input;
    };
  });
