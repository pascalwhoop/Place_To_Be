/**
 * Module requirements
 */

var FB = require('fb');
var GM = require('googlemaps');
var async = require('async');
// needed for facebook paging logic
var Client = require('node-rest-client').Client;
client = new Client();

//DB access
var mongoose = require('mongoose'),
    City = mongoose.model('City'),
    Page = mongoose.model('Page'),
    CommunityPage = mongoose.model('CommunityPage');


/**
 * Module Static Configuration Data
 */

var FB_APP_ID = "857640940981214";
var FB_APP_SECRET = "469300d9c3ed9fe6ff4144d025bc1148";

/**
 * get the access token for the AppServer
 */
var authenticateWithFB = function (cb) {
    FB.api('oauth/access_token', {
        client_id: FB_APP_ID,
        client_secret: FB_APP_SECRET,
        grant_type: 'client_credentials'
    }, function (res) {
        if (!res || res.error) {
            console.log(!res ? 'error occurred' : res.error);
            return;
        }
        FB.setAccessToken(res.access_token);
        console.log("got access token: " + res.access_token);
        cb();

    });
};

var fetchAllPagesForCity = function () {
    iterateOverCitiesInDB(findPagesForCity);
};

/**
 * makes sure the city in the DB has data that describes the boundries in a rectangular form. If none are found, we
 * call the Google Maps API and get our bounds.Then save to DB
 * @param city
 */
var checkForBounds = function (city, callback) {
    if (!city.geometry || !city.geometry.bounds || boundsEmpty(city.geometry.bounds)) {
        GM.geocode(city.formatted_address, function (err, result) {
            city.remove();
            city = new City(result.results[0]);
            city.save();
            callback(city);
        });
    } else {
        callback(city);
    }

};

//utilityfunction for shuffling arrays
var shuffle = function (o){
    for(var j, x, i = o.length; i; j = Math.floor(Math.random() * i), x = o[--i], o[i] = o[j], o[j] = x);
    return o;
};


/**
 * queriing FB API in a grid like fashion to find all pages within a city. this is very intense on the API which is why
 * we shouldn't do this often TODO right now we query 50x50 grid (2500 * (query*paging/query)) queries. So if we have
 * to do paging 3x per query its 7500 calls to the API.. yeah might be obvious what we intend
 * @param city
 */
var findPagesForCity = function (city) {

    var coordinates = getCoordinatesArray(city);

    //we shuffle our array to not always start at the same place when searching for new places
    coordinates = shuffle(coordinates);


    var asyncPlacesQueries = [];
    var accessToken = FB.getAccessToken();

    //this will be complicated! its a highly parallel iteration over thousands of coordinates per city which will
    // result in FB calls which will return hundreds of places ...
    coordinates.forEach(function (coordinate) {
        asyncPlacesQueries.push(function (callback) {
            FB.api('/search', 'get', {
                access_token: accessToken,
                q: "",
                type: "place",
                center: coordinate.lat + "," + coordinate.lng,
                distance: 2000,
                limit: 5000
            }, function (res) {
                handlePlacesResponse(res);
                callback();
            });
        })
    });
    //at this point we haven't actually run any queries, we just added them all to an array that we'll run through now
    async.series(asyncPlacesQueries, function(){
        console.log("all done, all pages fetched for this city!");
    });

    /*////////
     for(var iter in coordinates){
     if(coordinates.hasOwnProperty(iter)){
     var coordinate = coordinates[iter];
     FB.api('/search', 'get', {access_token: FB.getAccessToken(), q: "", type: "place", center: coordinates[0].lat + "," + coordinates[0].lng, distance: 2000}, function (res) {
     console.log(res);
     });
     }

     }
     */
};
/**
 * this function handles the response from the facebook API query of form
 * /search?q=<query>&type=place&center=<lat,lng>&distance<distance>. We want to make sure we get all the places and
 * facebook uses paging so we got to go ahead and follow through the paging process until there is no more paging
 * @param response
 */
function handlePlacesResponse(response) {
    var placeIDArray = [];

    //function used to get the data from the facebook paging logic
    var handlePagingNext = function(url, callback){
        client.get(url, {}, function(data){
            var response = JSON.parse(data.toString());
            response.data.forEach(function(item){
                placeIDArray.push(item.id);
            });
            if(response.paging.next){
                handlePagingNext(response.paging.next, callback);
            }else{
                callback();
            }
        });
    };

    if(response.data && response.data.constructor === Array){
        response.data.forEach(function(item){
            placeIDArray.push(item.id);
        });
    }
    if(response.paging && response.paging.next){
        handlePagingNext(response.paging.next, function(){
            //this callback gets called once paging is complete
            //at the end we pass all ID's found for a coordinate to this handler which will get the details for each place and save if need be
            handlePlacesIDsArray(placeIDArray);
        });
    }
}

/**
 * handles an array of placeIDs and gets the full information for each of them from the FB API
 * @param arr
 */
function handlePlacesIDsArray(arr) {
    var accessToken = FB.getAccessToken();
    var asyncIndividualPlacesQueries = [];
    arr.forEach(function (id) {
        //push all functions into array which we'll work off soon
        asyncIndividualPlacesQueries.push(function (callback) {
            //check if page already exists in DB
            Page.find({id: id}, function(err, results){
                //if no page exists
                if(results.length == 0){
                    //also check if is community page (we don't want to requery the FB API then)
                    CommunityPage.find({id: id}, function(err, results){
                        //no page & no community page in our DB, lets get details
                        if(results.length == 0){

                            //get full page data from FB API
                            console.log("new place");
                            FB.api('/' + id, 'get', {access_token: accessToken}, function (place) {
                                handlePlace(place, callback);
                            })
                        }else{
                            console.log("duplicate community page");
                        }
                    })

                }else{
                    console.log("duplicate page in " + results[0].location.city + ", " + results[0].location.zip);
                }
            });


        })
    });

    async.parallel(asyncIndividualPlacesQueries, function(){

    })
}
/**
 * handles a single FB place and saves it in the DB
 * @param place
 * @param callback
 */
function handlePlace(place, callback) {
    if (!place.is_community_page && place.hasOwnProperty("is_community_page")) {         //we only save non-community-pages since only they will actually create events
        //place._id = place.id;               //_id is needed for mongoDB
        var page = new Page(place);         //a place that is not community owned is == to a page in the facebook world
        page.save(function (err, page) {
            if (err) {
                if(err.code != 11000){
                    console.log(err); //todo error handling
                }
            }
            callback();
        });
    }
    else if(place.is_community_page){
        new CommunityPage({id: place.id, is_community_page: place.is_community_page}).save(function(err, page){
            if(err) {
                // 11000 == duplicate key. Since we are running our code in parallel, it might be the previous check didnt return anything yet but now another thread has already fetched and stored a place in the DB
                if(err.code != 11000){
                    console.log(err); //todo error handling
                }

            }
            callback();
        });
    }else{
        console.log("object wasn't a place nor a community place")
    }
}

/**
 * returns a 50x50 array with coordinates of the form {lat: Number, lng: Number}
 * @param city
 */
var getCoordinatesArray = function (city) {
    var HOPS = 50;
    var cityCoordArray = [];

    var latHopDist = getHopDistance(city, "lat", HOPS);
    var lngHopDist = getHopDistance(city, "lng", HOPS);

    var southwest = city.geometry.bounds.southwest;
    //var northeast = city.bounds.northeast;

    for (var i = 0; i < HOPS; i++) {
        for (var j = 0; j < HOPS; j++) {
            cityCoordArray.push({
                lat: southwest.lat + latHopDist * i,
                lng: southwest.lng + lngHopDist * j
            });
        }
    }
    return cityCoordArray;
};

/**
 * calculates the hop distance (in lat/lng) for our grid
 * @param city
 * @param angle
 * @param hops
 * @returns {number}
 */
var getHopDistance = function (city, angle, hops) {
    return Math.abs((city.geometry.bounds.southwest[angle] - city.geometry.bounds.northeast[angle]) / hops);
};


/**
 * checking for bounds in bounds object. if any value is missing, returns false
 * @param bounds
 * @returns {boolean}
 */
var boundsEmpty = function (bounds) {
    return (!bounds.southwest.lat || !bounds.southwest.lng || !bounds.northeast.lat || !bounds.northeast.lng);
};

/**
 * iterates over Cities in DB and  passes each city to parameter function
 * @param action to perform on each city
 */
var iterateOverCitiesInDB = function (action) {
    City.find({}, function (err, cities) {
            for (var city in cities) {
                if (cities.hasOwnProperty(city)) {
                    checkForBounds(cities[city], action);
                }
            }
        }
    )
};


module.exports = {
    fetchAllPagesForCity: fetchAllPagesForCity,
    authenticateWithFB: authenticateWithFB
};




