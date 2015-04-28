'use strict';

module.exports = function(app) {
	var users = require('../../app/controllers/users.server.controller');
	var pages = require('../../app/controllers/pages.server.controller');

	// Pages Routes
	app.route('/pages')
		.get(pages.list)
		.post(users.requiresLogin, pages.create);

	app.route('/pages/:pageId')
		.get(pages.read)
		.put(users.requiresLogin, pages.hasAuthorization, pages.update)
		.delete(users.requiresLogin, pages.hasAuthorization, pages.delete);

	// Finish by binding the Page middleware
	app.param('pageId', pages.pageByID);
};
