'use strict';
angular.module('placeToBe')
  .controller('loginController', function ($scope) {

      // @autor Stephan Blumenthal

      // This is called with the results from FB.getLoginStatus().
      function statusChangeCallback(response) {
          console.log('statusChangeCallback');
          console.log(response);

          // The response object is returned with a status field that lets the
          // app know the current login status of the person.

          if (response.status === 'connected') {

              //connected status means -> logged into placeToBe and Facebook.
              // the response also contains:
              //-> an access token for the person using placeToBe.
              //-> expiresIn (indicates the UNIX time when the token expires and needs to be renewed).
              //-> signedRequest (a signed parameter that contains information about the person using the app).
              //-> userID (the ID of the person using placeToBe).


              //ToDo redirect to placeToBe URL

          } else if (response.status === 'not_authorized') {
              // The person is logged into Facebook, but not placeToBe.
              document.getElementById('status').innerHTML = 'Please log ' +
                'into this app.';
          } else { //status === 'unknown'
              // The person is not logged into Facebook, so we're not sure if
              // they are logged into this app or not.
              document.getElementById('status').innerHTML = 'Please log ' +
                'into Facebook.';
          }
      }

      // This function is called when someone finishes with the Login
      // Button.  See the onlogin handler attached to it in the sample
      // code below.
      function checkLoginState() {
          FB.getLoginStatus(function (response) {
              statusChangeCallback(response);
          });
      }

      window.fbAsyncInit = function () {
          FB.init({
              appId: '857640940981214',
              cookie: true,  // enable cookies to allow the server to access
              // the session
              xfbml: true,  // parse social plugins on this page
              version: 'v2.2' // use version 2.2
          });

          // Now that we've initialized the JavaScript SDK, we call
          // FB.getLoginStatus().  This function gets the state of the
          // person visiting this page and can return one of three states to
          // the callback you provide.  They can be:
          //
          // 1. Logged into placeToBe ('connected')
          // 2. Logged into Facebook, but not placeToBe ('not_authorized')
          // 3. Not logged into Facebook and can't tell if they are logged into
          //    placeToBe or not.
          //
          // These three cases are handled in the callback function.

          FB.getLoginStatus(function (response) {
              statusChangeCallback(response);
          });
      };

  });
