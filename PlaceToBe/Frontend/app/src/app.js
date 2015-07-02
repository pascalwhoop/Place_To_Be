//main app entry point
angular.module('placeToBe', ['ngMaterial', 'ngRoute', 'ngMap', 'ngResource', 'ngFacebook', 'ngMessages'])

  //setting the theme of our app
  .config(function($mdThemingProvider){
    $mdThemingProvider.theme('default')
      .primaryPalette('cyan')
      .accentPalette('deep-orange');

  })
  //our apps routes
  .config(function ($routeProvider) {
    $routeProvider
      .when('/', {
        templateUrl: 'src/landingpage/landingpageView.html',
        controller: 'landingpageController'
      })
      .when('/heatmap', {
        templateUrl: 'src/heatmap/heatmapView.html',
        controller: 'heatmapController'
      })
      .when('/profile', {
        templateUrl: 'src/profile/profileView.html',
        controller: 'profileController'
      })
      .otherwise({
        redirectTo: '/'
      });
  })
  //configuring facebook plugin with AppID and permissions
  .config(function($facebookProvider){
    $facebookProvider.setAppId('857640940981214');
    $facebookProvider.setPermissions("email,user_likes,user_friends,user_events,user_actions.music,user_actions.news,user_actions.books,rsvp_event,");
  })
  //downloading facebook Javascript
  .run(function($rootScope){
    (function(d, s, id){
      var js, fjs = d.getElementsByTagName(s)[0];
      if (d.getElementById(id)) {return;}
      js = d.createElement(s); js.id = id;
      js.src = "//connect.facebook.net/en_US/sdk.js";
      fjs.parentNode.insertBefore(js, fjs);
    }(document, 'script', 'facebook-jssdk'));
  });

