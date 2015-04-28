var model = require('./model');
var fbEventParser = require('./services/fbEventParser.js');


fbEventParser.authenticateWithFB(function(){
    fbEventParser.fetchAllPagesForCity();
});




//getAllCitiesInDB
//city has no bounds
    //fetchCityBounds and Save
//create twoDimArrayForCity
//iterate over City Array calling FB API to fetch pages for each node
    //save each page into DB
//iterate over pages in DB to fetch all Events
    //save each event into DB

