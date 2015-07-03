SIZE_PER_PERSON = 0.00003; //in latitude/longitude 0.00001 ~ == 1m
PERSONS_PER_POINT = 40;

var getHeatmapDataArrayForEvents = function (events) {
  var heatmapData = [];
  events.forEach(function (element) {
      addPointsForEachGuest(element, heatmapData);
    }
  );
  return heatmapData;
};

//a method to create a heatmap around a coordinate
var addPointsForEachGuest = function (event, heatmapData) {
  //create a random number generator. we always use the same seed to always get the same visual representation of an event
  var rng = new Math.seedrandom("seed");
  //if event has no coordinates ignore this event
  if (!event.geoLocationCoordinates || !event.geoLocationCoordinates.coordinates) return;
  //calculate the maximum radius that should be filled with heatpoint dots
  var maxEventRadius = SIZE_PER_PERSON * Math.pow(event.attending_count, 0.66666);
  //we create points for every X people (PERSONS_PER_POINT)
  for (var i = 0; i < Math.ceil(event.attending_count / PERSONS_PER_POINT); i++) {
    //how far away from the event center?
    var heatPointDistance = maxEventRadius * rng.quick();
    //which direction from the event center?
    var angle = rng.quick() * Math.PI * 2;
    var a = event.geoLocationCoordinates.coordinates[0] + Math.cos(angle) * heatPointDistance;
    var b = event.geoLocationCoordinates.coordinates[1] + Math.sin(angle) * heatPointDistance;
    var latLng = {lat: a, lng: b}
    heatmapData.push(latLng);
  }
  return heatmapData;
};


importScripts("../../bower_components/seedrandom/seedrandom.min.js");


// we set up all our helper functions. we can now register an event listener
self.addEventListener('message', function(e){
  //we call our calculations and return the value as a message
  self.postMessage(getHeatmapDataArrayForEvents(e.data));
  self.close();
}, false);
