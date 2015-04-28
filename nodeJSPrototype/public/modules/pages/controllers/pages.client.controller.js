'use strict';

// Pages controller
angular.module('pages').controller('PagesController', ['$scope', '$stateParams', '$location', 'Authentication', 'Pages',
	function($scope, $stateParams, $location, Authentication, Pages) {
		$scope.authentication = Authentication;

		// Create new Page
		$scope.create = function() {
			// Create new Page object
			var page = new Pages ({
				name: this.name
			});

			// Redirect after save
			page.$save(function(response) {
				$location.path('pages/' + response._id);

				// Clear form fields
				$scope.name = '';
			}, function(errorResponse) {
				$scope.error = errorResponse.data.message;
			});
		};

		// Remove existing Page
		$scope.remove = function(page) {
			if ( page ) { 
				page.$remove();

				for (var i in $scope.pages) {
					if ($scope.pages [i] === page) {
						$scope.pages.splice(i, 1);
					}
				}
			} else {
				$scope.page.$remove(function() {
					$location.path('pages');
				});
			}
		};

		// Update existing Page
		$scope.update = function() {
			var page = $scope.page;

			page.$update(function() {
				$location.path('pages/' + page._id);
			}, function(errorResponse) {
				$scope.error = errorResponse.data.message;
			});
		};

		// Find a list of Pages
		$scope.find = function() {
			$scope.pages = Pages.query();
		};

		// Find existing Page
		$scope.findOne = function() {
			$scope.page = Pages.get({ 
				pageId: $stateParams.pageId
			});
		};
	}
]);