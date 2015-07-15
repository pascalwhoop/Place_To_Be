angular.module('placeToBe')
  .directive('detailsButton', function($mdDialog){
    return {
      //here we place the logic of the directive (link function)
      link: function (scope, element, attr) {


        scope.showDetails = function (event) {
          $mdDialog.show({
            controller: detailsController,
            templateUrl: 'src/details/detailView.html',
            parent: angular.element(document.body),
            targetEvent: event,
            locals:{
              eventData: scope.eventData
            }
          })
        };

        function detailsController($scope, $mdDialog, eventData) {
          $scope.event = eventData;
          $scope.hide = function () {
            $mdDialog.hide();
          };

          $scope.showFriendFb = function(url){

            if(url.length > 0)window.open(url, '_blank');

          }

        }
      },

      //this is the HTML template for the directive
      template: "<md-button ng-click='showDetails($event)' class='md-primary md-raised'>Details</md-button>",
      //we set it so only element tags are relevant
      restrict: 'E',
      //linking the attribute "city" value to the scope.city variable (maybe others too)
      scope: {
        eventData: "="
      }
    };
  });
