'use strict';

/**
 * Module dependencies.
 */
var mongoose = require('mongoose'),
    Schema = mongoose.Schema;

/**
 * City Schema
 */
var CitySchema = new Schema({

    address_components: [
        {
            long_name: String,
            short_name: String,
            types: [
                String
            ]
        }
    ],
    formatted_address: { type: String, index: {unique: true}},
    geometry: {
        bounds: {
            northeast: {
                lat: Number,
                lng: Number
            },
            southwest: {
                lat: Number,
                lng: Number
            }
        },
        location: {
            lat: Number,
            lng: Number
        }
    },
    place_id: String,
    types: [String]

});

mongoose.model('City', CitySchema);