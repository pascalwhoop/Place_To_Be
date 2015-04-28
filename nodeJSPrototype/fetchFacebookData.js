var model = require('./model');
var fbEventParser = require('./services/fbEventParser.js');


fbEventParser.authenticateWithFB(function(){
    fbEventParser.fetchAllPagesForCity();
});
