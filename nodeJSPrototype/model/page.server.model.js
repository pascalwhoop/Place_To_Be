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
var PageSchema = new Schema({

    id: {type: String, index: {unique: true}},
    about: String,
    attire: String,
    category: String,
    category_list: [
        {
            id: String,
            name: String
        }
    ],
    checkins: Number,
    cover: {
        cover_id: String,
        offset_x:Number,
        offset_y:Number,
        source: String,
        id: String
    },
    is_community_page: Boolean,
    likes: Number,
    link: String,
    location: {
        city: String,
        country: String,
        latitude: Number,
        longitude: Number,
        street: String,
        zip: String
    },
    name: String,
    were_here_count: Number,
    has_had_events: Boolean,
    geo_point_mongo: {
        type     : { type: String, default: "Point" },
        coordinates: [
            {type: "Number", index: '2dsphere'}
        ]
    }

});

mongoose.model('Page', PageSchema);