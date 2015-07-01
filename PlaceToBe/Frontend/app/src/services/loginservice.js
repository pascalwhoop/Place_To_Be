'use strict';

/**
 * @ngdoc service
 * @name placeToBe.loginService
 * @description
 * # gaussianService code taken from https://github.com/errcw/gaussian
 * Service in the frontendApp.
 */
angular.module('placeToBe')
  .factory('loginService', function ($facebook, $http, configService) {

    var loggedIn = false;     // connected, unknown
    var name = "";

    $facebook.getLoginStatus().then(
      function(response){
      loggedIn = response.status;
    },
    function(error){
      loggedIn = error;
    });

    var setLoginState = function(state){
      loggedIn = state;
    };
    var getLoginState = function(){
      return loggedIn;
    };

    var registerUser = function(user){
      var url = configService.BASE_URL + '/user?userEmail=' + user.email + '&userPassword=' + user.password;
        return $http.post(url, {});
    };

    var loginUser = function(user){
      var url = configService.BASE_URL + '/user?userEmail=' + user.email + '&userPassword=' + user.password;
      return $http.put(url, {});
    };

    /*$facebook.login().then(fbLoginSuccess)*/

    /*var fbLoginSuccess = function(response){
     $facebook.api("/me").then(function(response){
     $scope.welcomeMsg = "Welcome " + response.name;
     })
     }*/

    return {
      setLoginState: setLoginState,
      getLoginState: getLoginState,
      registerUser: registerUser,
      loginUser: loginUser,
      facebook: $facebook

    }
  });
