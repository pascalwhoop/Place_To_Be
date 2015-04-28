/**
 * iterate over all pages and add an attribute to each of them in DB then save again
 * */

Page.find({}, function(err, results){

    var asyncQueries = [];
    results.forEach(function(item){
        asyncQueries.push(function(callback){
            item.geo_point_mongo = {
                type: "Point",
                coordinates: [
                    item.location.latitude, item.location.longitude
                ]
            };
            item.save(function(err, response){
                callback();
            })
        })
    });
    async.series(asyncQueries, function(){
        console.log("all Done");
    })

});
