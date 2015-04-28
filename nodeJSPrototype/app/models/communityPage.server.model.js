'use strict';

/**
 * Module dependencies.
 */
var mongoose = require('mongoose'),
    Schema = mongoose.Schema;

/**
 * Page Schema
 * we fill this through a geolocation based query to FB API. But we cannot use all results since many are just is_community_page=true
 */
var CommunityPageSchema = new Schema({
    id: {type: String, index: {unique: true}},
    is_community_page: Boolean
});

mongoose.model('CommunityPage', CommunityPageSchema);