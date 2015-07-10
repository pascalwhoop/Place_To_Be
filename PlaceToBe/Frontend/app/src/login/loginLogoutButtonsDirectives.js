'use strict';

/**
 * @ngdoc directive
 * @name placeToBe.directive:heatmapDirective
 * @description
 * # heatmapDirective
 */
angular.module('placeToBe')
  .directive('loginButton', function ($http, $mdDialog) {
    return {
      //here we place the logic of the directive (link function)
      link: function (scope, element, attr) {


        scope.showLoginDialog = function (event) {
          $mdDialog.show({
            controller: LoginDialogController,
            templateUrl: 'src/login/loginDirective.html',
            parent: angular.element(document.body),
            targetEvent: event
          })
        };


        function LoginDialogController($scope, $mdDialog, loginService, toastNotifyService, configService) {
          $scope.cancel = function () {
            $mdDialog.cancel();
          };


          $scope.dialog = "login"; //default. we can switch between "login" "register" and "forgotPassword"

          //we register a User. passing the values to our service
          $scope.registerUser = function (user) {
            $scope.serverWait = true;
            loginService.registerUser(user).then(function (successRes) {
              toastNotifyService.showNotifyToast(configService.STRINGS.registration_success);
              $mdDialog.hide();
            }, function (errResponse) {
              toastNotifyService.showNotifyToast(configService.STRINGS.registration_error);
            })
          };

          $scope.ptbLogin = function (user) {
            $scope.browser = true;
            loginService.loginUser(user).then(function (successRes) {
              $mdDialog.hide();
            }, function (errResponse) {

            })
          };

          $scope.fbLogin = function () {
            loginService.facebook.login().then(function (response) {
              loginService.checkFacebookLogin();
              if (response.status == 'connected') $mdDialog.hide();
            });
          };

          $scope.performPasswordReset = function (email) {
            loginService.resetPassword(email)
              .success(function(){
                toastNotifyService.showNotifyToast(configService.STRINGS.reset_email_sent);
              })
              .error(function(){
                toastNotifyService.showNotifyToast(configService.STRINGS.backend_error);
              })
          }




        }


      },

      //this is the HTML template for the directive
      template: "<md-button ng-click='showLoginDialog($event)' class='md-raised' ng-show='!loginService.getLoginStateSync()'>Login</md-button>",
      //we set it so only element tags are relevant
      restrict: 'E',
      //linking the attribute "city" value to the scope.city variable (maybe others too)
      scope: {
        loginService: "="
      }
    };
  })
  .directive('logoutButton', function ($http, $mdDialog) {
    return {
      //here we place the logic of the directive (link function)
      link: function (scope, element, attr) {

        scope.showLogoutDialog = function (event) {
          $mdDialog.show({
            controller: LogoutDialogController,
            templateUrl: 'src/login/logoutDirective.html',
            parent: angular.element(document.body),
            targetEvent: event
          })
        };


        function LogoutDialogController($scope, $mdDialog, loginService) {
          $scope.cancel = function () {
            $mdDialog.cancel();
          };

          $scope.logout = function(){
            loginService.logout();
            $mdDialog.hide();
            location.reload();
          }

        }


      },

      //this is the HTML template for the directive
      template: "<md-button ng-click='showLogoutDialog($event)' class='md-raised' ng-show='loginService.getLoginStateSync()'>Logout</md-button>",
      //we set it so only element tags are relevant
      restrict: 'E',
      //linking the attribute "city" value to the scope.city variable (maybe others too)
      scope: {
        loginService: "="
      }
    };
  });
