'use strict';

/**
 * @ngdoc directive
 * @name frontendApp.directive:goClick
 * @description
 * # goClick
 */
angular.module('placeToBe')
  .directive('goClick', function ($location) {
    return function (scope, element, attrs) {
      var path;

      attrs.$observe('goClick', function (val) {
        path = val;
      });

      element.bind('click', function () {
        scope.$apply(function () {
          $location.path(path);
        });
      });
    };
  })


/**
 * @ngdoc directive
 * @name frontendApp.directive:equals
 * @description
 * # equals
 */
  .directive('equals', function () {
    return {
      restrict: 'A', // only activate on element attribute
      require: '?ngModel', // get a hold of NgModelController
      link: function (scope, elem, attrs, ngModel) {
        if (!ngModel) return; // do nothing if no ng-model

        // watch own value and re-validate on change
        scope.$watch(attrs.ngModel, function () {
          validate();
        });

        // observe the other value and re-validate on change
        attrs.$observe('equals', function (val) {
          validate();
        });

        var validate = function () {
          // values
          var val1 = ngModel.$viewValue;
          var val2 = attrs.equals;

          // set validity
          ngModel.$setValidity('equals', !val1 || !val2 || val1 === val2);
        };
      }
    }
  })


/**
 * @ngdoc directive
 * @name frontendApp.directive:equals
 * @description
 * A directive to show and hide a loading bar. it takes two attributes (show-event and hide-event) which must be the event names it should listen on on the rootScope.
 */
  .directive('waitBar', ['$rootScope', function ($rootScope) {
    return {
      restrict: 'E', // only activate on element attribute
      link: function (scope, elem, attrs) {

        scope.visible = false;
        scope.message = attrs.message;
        // watch own value and re-validate on change
        $rootScope.$on(attrs.showEvent, function () {
          scope.visible = true;
        });

        $rootScope.$on(attrs.hideEvent, function () {
          scope.visible = false;
        });

      },
      template: '<div ng-show="visible"><p>{{message}}</p></p><md-progress-linear md-mode="indeterminate" flex></md-progress-linear></div>'
    }
  }]);
