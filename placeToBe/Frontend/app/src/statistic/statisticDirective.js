'use strict';

angular.module('starterApp')
    .directive('statisticTable', function ($mdDialog) {
        return {
            //here we place the logic of the directive (link function)
            link: function (scope, element, attr) {
                scope.showStatDialog = function () {
                    $mdDialog.show({
                        controller: statController,
                        templateUrl: 'statisticDirectiveTemplate.html',
                        locals: {
                            eventData: scope.eventData
                        }
                    })
                };
                function statController($scope, $mdDialog, eventData, ngTableParams, $filter) {
                    $scope.eventData = eventData;


                    $scope.tableParams = new ngTableParams({
                        page: 1,            // show first page
                        count: 3,
                        sorting: {
                            name: 'asc'     // initial sorting
                        }
                    }, {
                        total: eventData.length, // length of data
                        getData: function ($defer, params) {
                            var orderedData = params.sorting() ?
                                $filter('orderBy')(eventData, params.orderBy()) :
                                eventData;
                            $defer.resolve(orderedData.slice((params.page() - 1) * params.count(), params.page() * params.count()));
                        }
                    })

                    $scope.cancel = function () {
                        $mdDialog.cancel();
                    }

                }

            },

            template: "<md-button ng-click='showStatDialog()' class='md-raised'>Stat</md-button>",
            restrict: 'E',
            //linking the attribute "city" value to the scope.city variable (maybe others too)
            scope: {
                eventData: "=",

            }
        };
    })
    .directive("statDetails", function ($mdDialog) {
        return {
            link: function (scope, element, attr) {
                scope.showStatDetailsDialog = function () {
                    $mdDialog.show({
                        controller: statDetailsController,
                        templateUrl: 'statDetailsTemplate.html',
                        parent: angular.element(document.body),
                    })
                };
                function statDetailsController($scope) {
                    $scope.cancel = function () {
                        $mdDialog.cancel();
                    }
                }
            },
            template: "<md-button class='md-raised' ng-click='showStatDetailsDialog()'>Details</md-button>",
            restrict: 'E'
        }
    });
