'use strict';

/**
 * Module dependencies.
 */
var mongoose = require('mongoose'),
	errorHandler = require('./errors.server.controller'),
	Page = mongoose.model('Page'),
	_ = require('lodash');





/**
 * Create a Page
 */
exports.create = function(req, res) {
	var page = new Page(req.body);
	page.user = req.user;

	page.save(function(err) {
		if (err) {
			return res.status(400).send({
				message: errorHandler.getErrorMessage(err)
			});
		} else {
			res.jsonp(page);
		}
	});
};

/**
 * Show the current Page
 */
exports.read = function(req, res) {
	res.jsonp(req.page);
};

/**
 * Update a Page
 */
exports.update = function(req, res) {
	var page = req.page ;

	page = _.extend(page , req.body);

	page.save(function(err) {
		if (err) {
			return res.status(400).send({
				message: errorHandler.getErrorMessage(err)
			});
		} else {
			res.jsonp(page);
		}
	});
};

/**
 * Delete an Page
 */
exports.delete = function(req, res) {
	var page = req.page ;

	page.remove(function(err) {
		if (err) {
			return res.status(400).send({
				message: errorHandler.getErrorMessage(err)
			});
		} else {
			res.jsonp(page);
		}
	});
};

/**
 * List of Pages
 */
exports.list = function(req, res) {
	Page.find().sort('-created').populate('user', 'displayName').exec(function(err, pages) {
		if (err) {
			return res.status(400).send({
				message: errorHandler.getErrorMessage(err)
			});
		} else {
			res.jsonp(pages);
		}
	});
};

/*

exports.list = function(req, res) {
    Page.find({},{name: 1, geo_point_mongo: 1, _id: 0}).exec(function(err, pages) {
        if (err) {
            return res.status(400).send({
                message: errorHandler.getErrorMessage(err)
            });
        } else {


            res.jsonp(pages.slice(1, 40));
        }
    });
};*/

/**
 * Page middleware
 */
exports.pageByID = function(req, res, next, id) { 
	Page.findById(id).populate('user', 'displayName').exec(function(err, page) {
		if (err) return next(err);
		if (! page) return next(new Error('Failed to load Page ' + id));
		req.page = page ;
		next();
	});
};

/**
 * Page authorization middleware
 */
exports.hasAuthorization = function(req, res, next) {
	if (req.page.user.id !== req.user.id) {
		return res.status(403).send('User is not authorized');
	}
	next();
};
