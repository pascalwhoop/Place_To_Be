angular
  .module('placeToBe', ['ngMaterial', 'ngRoute', 'ngMap'])
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
      .primaryPalette('brown')
      .accentPalette('red');

  })
  .config(function ($routeProvider) {
    $routeProvider
      .when('/', {
        templateUrl: 'src/heatmap/heatmapView.html',
        controller: 'heatmapController'
      })
      .when('/login',{
        templateUrl: 'src/login/loginView.html',
        controller: 'loginController'
      })
      .otherwise({
        redirectTo: '/'
      });
  });
