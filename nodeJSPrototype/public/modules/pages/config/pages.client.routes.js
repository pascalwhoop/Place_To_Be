'use strict';

//Setting up route
angular.module('pages').config(['$stateProvider',
	function($stateProvider) {
		// Pages state routing
		$stateProvider.
		state('listPages', {
			url: '/pages',
			templateUrl: 'modules/pages/views/list-pages.client.view.html'
		}).
		state('createPage', {
			url: '/pages/create',
			templateUrl: 'modules/pages/views/create-page.client.view.html'
		}).
		state('viewPage', {
			url: '/pages/:pageId',
			templateUrl: 'modules/pages/views/view-page.client.view.html'
		}).
		state('editPage', {
			url: '/pages/:pageId/edit',
			templateUrl: 'modules/pages/views/edit-page.client.view.html'
		});
	}
]);