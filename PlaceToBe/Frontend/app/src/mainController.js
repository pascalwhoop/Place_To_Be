'use strict';

/**
 * @ngdoc function
 * @name frontendApp.controller:mainController
 * @description
 * # MainCtrl
 * Controller of the frontendApp
 */
angular.module('placeToBe')
  .controller('mainController', function ($scope, $rootScope, $location) {
    $scope.loggedIn = false;

    $rootScope.$on('loginSuccessful', function(){
      $scope.loggedIn = true;
      $location.path("heatmap")
    })

  });

