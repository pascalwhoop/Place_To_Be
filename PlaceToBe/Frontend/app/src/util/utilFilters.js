'use strict';

/**
 * @ngdoc filter
 * @name placeToBe.filter:range
 * @function
 * @description
 * # range
 * Filter in the placeToBe.
 */
angular.module('placeToBe')
  .filter('range', function () {
    return function(input, min, max) {
      min = parseInt(min); //Make string input int
      max = parseInt(max);
      for (var i=min; i<=max; i++)
        input.push(i);
      return input;
    };
  })
  .filter('cut', function () {
    return function (value, wordwise, max, tail) {
      if (!value) return '';

      max = parseInt(max, 10);
      if (!max) return value;
      if (value.length <= max) return value;

      value = value.substr(0, max);
      if (wordwise) {
        var lastspace = value.lastIndexOf(' ');
        if (lastspace != -1) {
          value = value.substr(0, lastspace);
        }
      }

      return value + (tail || ' …');
    };
  });
