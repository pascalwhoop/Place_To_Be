angular
  .module('placeToBe', ['ngMaterial', 'ngRoute', 'ngMap', 'ngResource', 'ngFacebook'])
  .config(function($mdThemingProvider, $mdIconProvider){

    $mdIconProvider
      .defaultIconSet("./assets/svg/avatars.svg", 128)
      .icon("menu"       , "./assets/svg/menu.svg"        , 24)
      .icon("share"      , "./assets/svg/share.svg"       , 24)
      .icon("google_plus", "./assets/svg/google_plus.svg" , 512)
      .icon("hangouts"   , "./assets/svg/hangouts.svg"    , 512)
      .icon("twitter"    , "./assets/svg/twitter.svg"     , 512)
      .icon("phone"      , "./assets/svg/phone.svg"       , 512);

    $mdThemingProvider.theme('default')
      .primaryPalette('cyan')
      .accentPalette('deep-orange');

  })
  .config(function ($routeProvider) {
    $routeProvider
      .when('/', {
        templateUrl: 'src/heatmap/heatmapView.html',
        controller: 'heatmapController'
      })
      .otherwise({
        redirectTo: '/'
      });
  })
  .config(function($facebookProvider){
    $facebookProvider.setAppId('857640940981214');
  })
  .run(function($rootScope){
    (function(d, s, id){
      var js, fjs = d.getElementsByTagName(s)[0];
      if (d.getElementById(id)) {return;}
      js = d.createElement(s); js.id = id;
      js.src = "//connect.facebook.net/en_US/sdk.js";
      fjs.parentNode.insertBefore(js, fjs);
    }(document, 'script', 'facebook-jssdk'));
  });

/*red
 pink
 purple
 deep-purple
 indigo
 blue
 light-blue
 cyan
 teal
 green
 light-green
 lime
 yellow
 amber
 orange
 deep-orange
 brown
 grey
 blue-grey*/
