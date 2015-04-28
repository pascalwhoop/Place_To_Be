var express = require('express');
var app = express();
var places = require('routes/placesRoute.js');

// respond with "hello world" when a GET request is made to the homepage
app.get('/', function(req, res) {
    res.send('hello world');
});