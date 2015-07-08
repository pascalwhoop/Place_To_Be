'use strict';

/**
 * @ngdoc service
 * @name placeToBe.loginService
 * @description
 * # gaussianService code taken from https://github.com/errcw/gaussian
 * Service in the frontendApp.
 */
angular.module('placeToBe')
  .factory('loginService', function ($facebook, $http, configService, $rootScope, toastNotifyService) {
    var uNotify = toastNotifyService;

    var loggedIn = {};
    var userInfo = {};
    //initially we're not connected to anything. but this changes after bootstrapping, since we try and connect with fb and our own servers
    var initialize = function () {
      loggedIn = {
        fb: 'not_authorized',    // connected, unknown, not_authorized, pending
        ptb: 'not_authorized'    // connected, not_authorized, pending
      };

      userInfo = {
        fb: {},
        ptp: {}
      };
    };
    initialize();


    var getLoginType = function () {
      if (loggedIn.fb == 'connected') return 'facebook';
      else if (loggedIn.ptp == 'connected') return 'placeToBe';

    };

    /**
     * a function that clears all data that could be considered a "login" so disconnects from fb if connected and clears user data of ptb
     */
    var logout = function () {
      initialize();
      $facebook.logout()
        .then(function (response) {
          console.log(response);
        }, function (response) {
          console.log(response);
        })

    };

    //async login state
    var getLoginState = function (callback) {
      if (loggedIn.fb == 'connected' || loggedIn.ptb == 'connected') callback(true);
      //none of the two are connected. so we check if it's pending. if so, a call to a server is in progress and we wait for the call to finish
      else if (loggedIn.fb == 'pending' || loggedIn.ptb == 'pending') {
        var unregister = $rootScope.$on('serverCallEnd', function () {
          //call has ended. if it's now connected we set it to true otherwise we give up and set it to false
          if (loggedIn.fb == 'connected' || loggedIn.ptb == 'connected') callback(true);
          else callback(false);
          unregister();
        });
      }
      //it was neither connected nor pending so we're not connected to anything
      else callback(false);
    };

    var getLoginStateSync = function () {
      if (loggedIn.fb == 'connected' || loggedIn.ptb == 'connected') return true;
      return false;
    };

    /**
     * set the auth header for $http requests for this session. saves code
     * @param id the email of the user or the fbId
     * @param pass the password of the user or the fbToken (short one)
     */

    var setAuthHeader = function (id, pass) {
      $http.defaults.headers.common['Authorization'] = 'Basic ' + btoa(id + ':' + pass);
    };

    //================================== FB LOGIN/LOGOUT STUFF  ====================================

    /**
     * we check wether we're currently logged into facebook. if yes, we can proceed with our app, if not, we let the user login first
     */
    var checkFacebookLogin = function () {
      //checking for fb login state
      loggedIn.fb = "pending";                                //setting status to pending. we're checking right now
      $rootScope.$emit('serverCallStart');                    //creating an event to inform e.g. UI Elements about the process
      $facebook.getLoginStatus().then(
        function (response) {
          handleFbLoginResponse(response);
          $rootScope.$emit('serverCallEnd');                  //sending another event. we are done with the server call.
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
    var handleFbLoginResponse = function (response) {
      loggedIn.fb = response.status;
      userInfo.fb.authResponse = response.authResponse;
      if (response.status == 'connected') {
        $rootScope.$emit('loginSuccessful');
        uNotify.showNotifyToast('Login successful', null, 1500); //we show a little toast to the user notifying him of a successful login
        //this causes all following requests to be sent with a token in the authorization header
        setAuthHeader(response.authResponse.userID, response.authResponse.accessToken);
      }else{
        if(response.status != 'unknown')uNotify.showNotifyToast('Login unsuccessful. Your Facebook status to our app is ' + response.status);
      }

    };

    //register the above function to be called once the DOM is loaded
    angular.element(document).ready(checkFacebookLogin);

    var fetchFbUserDetails = function () {
      $facebook.api('/me').then(function (response) {
        console.log(response);
      })
    };


    //================================== PTB LOGIN STUFF  ====================================

    /**
     * we register a user with the backend here.
     * @param user
     * @returns {*}a promise of the http post response
     */
    var registerUser = function (user) {
      $rootScope.$emit('serverCallStart');

      //create "clean" user variable
      var u = {
        email: user.email,
        password: user.password
      };
      var url = configService.BASE_URL + '/user';

      return $http.post(url, u).then(function (response) {
        $rootScope.$emit('serverCallEnd');
      }, function (err) {
        $rootScope.$emit('serverCallEnd');
      });
    };

    /**
     * login user with ptb credentials
     * @param user a user object containing an email and a password
     * @returns {*}
     */
    var loginUser = function (user) {
      $rootScope.$emit('serverCallStart');

      var req = {
        method: 'GET',
        url: configService.BASE_URL + '/user/authorize',
        headers: {
          'Authorization': 'Basic ' + btoa(user.email + ":" + user.password)
        }
      };
      return $http(req)
        .success(function (data, status) {
          $rootScope.$emit('serverCallEnd');
          if (status == 200) {
            uNotify.showNotifyToast('Login successful');
            $rootScope.$emit('loginSuccessful');
            //set our state to connected
            loggedIn.ptb = 'connected';
            // add the email:pass to the userInfo. might need it
            userInfo.ptb = user;
            //set auth header for future calls
            setAuthHeader(user.email, user.password);
          }
        })
        .error(function (data, status) {
          $rootScope.$emit('serverCallEnd');
          $rootScope.$emit('loginError');
        });
    };


    return {
      checkFacebookLogin: checkFacebookLogin,
      getLoginType: getLoginType,
      fetchFbUserDetails: fetchFbUserDetails,
      getLoginState: getLoginState,
      getLoginStateSync: getLoginStateSync,
      registerUser: registerUser,
      loginUser: loginUser,
      logout: logout,
      facebook: $facebook

    }
  });
