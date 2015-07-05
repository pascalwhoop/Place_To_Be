'use strict';

/**
 * @ngdoc directive
 * @name frontendApp.directive:heatmapDirective
 * @description
 * # heatmapDirective
 */
angular.module('placeToBe')
    .directive('eventList', function () {
        return {
            //here we place the logic of the directive (link function)
            link: function (scope, element, attr) {

            },

            //this is the HTML template for the directive
            templateUrl: 'src/eventlist/EventListDirectiveTemplate.html',
            //we set it so only element tags are relevant
            restrict: 'E',
            //linking the attribute "city" value to the scope.city variable (maybe others too)
            scope: {
                eventData: "="
            }
        };
    });
