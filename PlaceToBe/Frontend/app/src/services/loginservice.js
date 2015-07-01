'use strict';

/**
 * @ngdoc service
 * @name placeToBe.loginService
 * @description
 * # gaussianService code taken from https://github.com/errcw/gaussian
 * Service in the frontendApp.
 */
angular.module('placeToBe')
  .factory('loginService', function () {

    var loggedIn = false;

    var setLoginState = function(state){
      loggedIn = state;
    };
    var getLoginState = function(){
      return loggedIn;
    };

    return {
      setLoginState: setLoginState,
      getLoginState: getLoginState
    }
  });
