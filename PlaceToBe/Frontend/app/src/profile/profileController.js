'use strict';

/**
 * @ngdoc function
 * @name placeToBe.controller:landingpageController
 * @description
 * # MainCtrl
 * Controller of the frontendApp
 */
angular.module('placeToBe')
  .controller('profileController', function ($scope, loginService, $facebook, toastNotifyService) {
    $scope.loginService = loginService;

    $scope.loginType = loginService.getLoginType();

    if(loginService.getLoginType() == 'facebook'){
      $facebook.api('/me').then(function(response){
        $scope.fbMe = response;
      });
      $facebook.api('/me/picture?type=large&redirect=false').then(function(response){
        $scope.fbProfilePicUrl = response.data.url;
      })
    }else{
      $scope.ptbMe = loginService.getUserInfo();
    }

    $scope.performPasswordChange = function(changePassword){
      loginService.changePassword(changePassword.old, changePassword.newPassword1)
        .success(function(data, status, headers, config){
          if(status == 200){
            toastNotifyService.showNotifyToast("Password changed");
            $scope.changePassword = {}; //clear form
            $scope.changePsw = false;
          }
        })
        .error(function(response){
          toastNotifyService.showNotifyToast("Couldn't change Password. An error occured");
        })
    };

    $scope.disconnectFb = function(){
      var endpoint = $scope.fbMe.id + '/permissions';
      FB.api(endpoint, 'DELETE', {}, function(response){
        console.log(response); //TODO TOAST
        location.reload(); //we reset the website. the user will be back to the beginning
      })
    }


  });

