'use strict';

/**
 * @ngdoc directive
 * @name frontendApp.directive:heatmapDirective
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


        function LoginDialogController($scope, $mdDialog, loginService) {
          $scope.cancel = function () {
            $mdDialog.cancel();
          };


          $scope.registerDialog = false;
          $scope.toggleRegister = function () {
            $scope.registerDialog = !$scope.registerDialog;
          };

          //we register a User. passing the values to our service
          $scope.registerUser = function (user) {
            $scope.serverWait = true;
            loginService.registerUser(user).then(function (successRes) {

            }, function (errResponse) {

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

          $scope.ptbLogout = function(){
            //TODO logout ptb
          }

          $scope.fbLogout = function(){
            //TODO logout fb
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
