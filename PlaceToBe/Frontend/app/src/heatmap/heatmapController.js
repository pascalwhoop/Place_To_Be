'use strict';

/**
 * @ngdoc function
 * @name frontendApp.controller:MainCtrl
 * @description
 * # MainCtrl
 * Controller of the frontendApp
 */
angular.module('placeToBe')
  .controller('heatmapController', function ($scope) {


    //TODO MOCK DATA FOR PROTOTYPE 1
    $scope.filter = {
      city: {
        name: "cologne",
        location: null
      },
      time: 9000
    };
    $scope.eventSourceUrl = "https://placetobe-koeln.azurewebsites.net/api/event";


  });
