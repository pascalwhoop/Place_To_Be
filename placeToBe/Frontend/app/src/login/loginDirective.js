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
          $scope.hide = function () {
            $mdDialog.hide();
          };
          $scope.cancel = function () {
            $mdDialog.cancel();
          };
          $scope.answer = function (answer) {
            $mdDialog.hide(answer);
          };

          $scope.registerDialog = false;
          $scope.toggleRegister = function () {
            $scope.registerDialog = !$scope.registerDialog;
          };

          //we register a User. passing the values to our service
          $scope.registerUser = function (user) {
            $scope.serverWait = true;
            loginService.registerUser(user).then(function(successRes){
              $scope.serverWait = false;
            }, function(errResponse){

            })
          };

          $scope.loginUser = function(user){
            $scope.browser = true;
            loginService.loginUser(user).then(function(successRes){
              $scope.serverWait = false;
            }, function(errResponse){

            })
          };

          $scope.fbLogin = function () {
            loginService.facebook.login().then(function (response) {
              loginService.facebook.getLoginStatus(function (response) {
                console.log(response);
              });
            });
            loginService.facebook.getLoginStatus();
            loginService.facebook.getAuthResponse();
            /*
             $mdDialog.hide();

             })*/
          };


        }


      },

      //this is the HTML template for the directive
      template: "<md-button ng-click='showLoginDialog($event)' class='md-raised'>{{(loginService.getLoginState() == 'connected') ? 'Logout' : 'Login'}}</md-button>",
      //we set it so only element tags are relevant
      restrict: 'E',
      //linking the attribute "city" value to the scope.city variable (maybe others too)
      scope: {
        loginService: "="
      }
    };
  });
