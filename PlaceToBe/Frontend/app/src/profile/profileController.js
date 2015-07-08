'use strict';

/**
 * @ngdoc function
 * @name frontendApp.controller:landingpageController
 * @description
 * # MainCtrl
 * Controller of the frontendApp
 */
angular.module('placeToBe')
  .controller('profileController', function ($scope, loginService, $facebook) {
    $scope.loginService = loginService;

    $scope.loginType = loginService.getLoginType();

    if(loginService.getLoginType() == 'facebook'){
      $facebook.api('/me').then(function(response){
        $scope.fbMe = response;
      });
      $facebook.api('/me/picture?type=large&redirect=false').then(function(response){
        $scope.fbProfilePicUrl = response.data.url;
      })
    }

    $scope.disconnectFb = function(){
      var endpoint = $scope.fbMe.id + '/permissions';
      FB.api(endpoint, 'DELETE', {}, function(response){
        console.log(response); //TODO TOAST
        location.reload(); //we reset the website. the user will be back to the beginning
      })
    }


  });

