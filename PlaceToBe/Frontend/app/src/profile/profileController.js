'use strict';

/**
 * @ngdoc function
 * @name frontendApp.controller:landingpageController
 * @description
 * # MainCtrl
 * Controller of the frontendApp
 */
angular.module('placeToBe')
  .controller('profileController', function ($scope, loginService) {
    $scope.loginService = loginService;

    $scope.menu = {
      isOpen : false,
    }


  });

