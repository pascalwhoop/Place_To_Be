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
        scope.func = false;

        scope.showLoginDialog = function(event){
          $mdDialog.show({
            controller: LoginDialogController,
            templateUrl: 'src/login/loginDirective.html',
            parent: angular.element(document.body),
            targetEvent: event
          })
            .then(function(answer) {
              $scope.alert = 'You said the information was "' + answer + '".';
            }, function() {
              $scope.alert = 'You cancelled the dialog.';
            });
        };

        function LoginDialogController($scope, $mdDialog, $facebook) {
          $scope.hide = function() {
            $mdDialog.hide();
          };
          $scope.cancel = function() {
            $mdDialog.cancel();
          };
          $scope.answer = function(answer) {
            $mdDialog.hide(answer);
          };

          $scope.fbLogin = function(){
            $facebook.getLoginStatus().then(function(response){
              console.log(response);
            })
          };


        }


      },

      //this is the HTML template for the directive
      template: "<md-button ng-click='showLoginDialog($event)' class='md-raised'>{{loggedIn ? 'Logout' : 'Login'}}</md-button>",
      //we set it so only element tags are relevant
      restrict: 'E',
      //linking the attribute "city" value to the scope.city variable (maybe others too)
      scope: {
        city: "=",
        time: "=",
        eventSource: "="
      }
    };
  });
