'use strict';

/**
 * @ngdoc service
 * @name placeToBe.loginService
 * @description
 * # gaussianService code taken from https://github.com/errcw/gaussian
 * Service in the frontendApp.
 */
angular.module('placeToBe')
  .factory('loginService', function ($facebook, $http, configService, $rootScope) {

    //initially we're not connected to anything. but this changes after bootstrapping, since we try and connect with fb and our own servers
    var loggedIn = {
      fb: 'not_authorized',    // connected, unknown, not_authorized, pending
      ptb: 'not_authorized'    // connected, not_authorized, pending
    };

    var userInfo = {
      fb: {},
      ptp: {}
    };



    var checkFacebookLogin = function(){
      //checking for fb login state
      loggedIn.fb = "pending";                                //setting status to pending. we're checking right now
      $rootScope.$emit('serverCallStart');                    //creating an event to inform e.g. UI Elements about the process
      $facebook.getLoginStatus().then(
        function (response) {
          handleFbLoginResponse(response);
          $rootScope.$emit('serverCallEnd');                  //sending another event. we are done with the server call.
          //TODO send Token to Backend
        },

        function (error) {
          loggedIn.fb = error;
          $rootScope.$emit('serverCallEnd');
        });
    };

    /**
     * takes the Fb login success response and performs a few tasks. E.g. we set the loggedIn & userInfo fields and emit a login success event
     * @param response
     */
    var handleFbLoginResponse = function(response){
      loggedIn.fb = response.status;
      userInfo.fb.authResponse = response.authResponse;
      if(response.status == 'connected') $rootScope.$emit('loginSuccessful');
    };

    //register the above function to be called once the DOM is loaded
    angular.element(document).ready(checkFacebookLogin);

    var fetchFbUserDetails = function(){
      $facebook.api('/me').then(function(response){
        console.log(response);
      })
    };


    var getLoginType = function(){
      if (loggedIn.fb == 'connected') return 'facebook';
      else if(loggedIn.ptp == 'connected') return 'placeToBe';

    };

    //async login state
    var getLoginState = function (callback) {
      if (loggedIn.fb == 'connected' || loggedIn.ptb == 'connected') callback(true);
      //none of the two are connected. so we check if it's pending. if so, a call to a server is in progress and we wait for the call to finish
      else if(loggedIn.fb == 'pending' || loggedIn.ptb == 'pending') {
        var unregister = $rootScope.$on('serverCallEnd', function(){
          //call has ended. if it's now connected we set it to true otherwise we give up and set it to false
          if (loggedIn.fb == 'connected' || loggedIn.ptb == 'connected') callback(true);
          else callback(false);
          unregister();
        });
      }
      //it was neither connected nor pending so we're not connected to anything
      else callback(false);
    };

    var getLoginStateSync = function(){
      if(loggedIn.fb == 'connected' || loggedIn.ptb == 'connected') return true;
      return false;
    }


    /**
     * we register a user with the backend here.
     * @param user
     * @returns {*}
     */
    var registerUser = function (user) {
      $rootScope.$emit('serverCallStart');

      //create "clean" user variable
      var u = {
        email: user.email,
        password: user.password
      };
      var url = configService.BASE_URL + '/user';

      return $http.post(url, u).then(function(response){
        $rootScope.$emit('serverCallEnd');
      }, function(err){
        $rootScope.$emit('serverCallEnd');
      });
    };

    var loginUser = function (user) {
      $rootScope.$emit('serverCallStart');
      var url = configService.BASE_URL + '/user?userEmail=' + user.email + '&userPassword=' + user.password;
      return $http.put(url, {}).then(function(response){
        $rootScope.$emit('serverCallEnd');
      }, function(err){
        $rootScope.$emit('serverCallEnd');
      });
    };

    //check for logged in state during start


    /*$facebook.login().then(fbLoginSuccess)*/

    /*var fbLoginSuccess = function(response){
     $facebook.api("/me").then(function(response){
     $scope.welcomeMsg = "Welcome " + response.name;
     })
     }*/

    return {
      checkFacebookLogin: checkFacebookLogin,
      getLoginType:getLoginType,
      fetchFbUserDetails:fetchFbUserDetails,
      getLoginState: getLoginState,
      getLoginStateSync: getLoginStateSync,
      registerUser: registerUser,
      loginUser: loginUser,
      facebook: $facebook

    }
  });
