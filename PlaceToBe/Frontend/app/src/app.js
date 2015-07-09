/**
 * This is our main entry point to our app. we declare our dependencies, ...
 */
angular.module('placeToBe', ['ngMaterial', 'ngRoute', 'ngMap', 'ngResource', 'ngFacebook', 'ngMessages', 'ngTable'])

/**
 *  our style config,
 */
  .config(function ($mdThemingProvider) {
    $mdThemingProvider.theme('default')
      .primaryPalette('cyan')
      .accentPalette('deep-orange');

  })

/**
 * our navigation routes
 */
  .config(function ($routeProvider) {
    $routeProvider

      .when('/', {
        templateUrl: 'src/landingpage/landingpageView.html',
        controller: 'landingpageController',
        access: {
          requiresLogin: false
        }
      })
      .when('/heatmap', {
        templateUrl: 'src/heatmap/heatmapView.html',
        controller: 'heatmapController',
        access: {
          requiresLogin: true
        }
      })
      .when('/statistic', {
        templateUrl: 'src/statistic/statisticView.html',
        controller: 'statisticController',
        access: {
          requiresLogin: true
        }
      })
      .when('/profile', {
        templateUrl: 'src/profile/profileView.html',
        controller: 'profileController',
        access: {
          requiresLogin: true
        }
      })
      .otherwise({
        redirectTo: '/'
      });
  })

/**
 * our facebook module
 */
  .config(function ($facebookProvider) {
    $facebookProvider.setAppId('857640940981214');
    $facebookProvider.setPermissions("email,user_likes,user_friends,user_events,user_actions.music,user_actions.news,user_actions.books,rsvp_event,");
  })


  .run(function (loginService, $rootScope, $location) {
    /**
     * and download the fb javascript library once angular is ready
     */
    (function (d, s, id) {
      var js, fjs = d.getElementsByTagName(s)[0];
      if (d.getElementById(id)) {
        return;
      }
      js = d.createElement(s);
      js.id = id;
      js.src = "//connect.facebook.net/en_US/sdk.js";
      fjs.parentNode.insertBefore(js, fjs);
    }(document, 'script', 'facebook-jssdk'));


    /**
     * making sure our users can only see our landing page if they're not logged in
     * This could be much more advanced but for now this is sufficient
     * http://jonsamwell.com/url-route-authorization-and-security-in-angular/
     */
    $rootScope.$on('$routeChangeStart', function(event, next){
      var authorised;
      if(next.access !== undefined){
        if(!loginService.getLoginStateSync()){
          $location.path('/').replace();
        }
      }
    })

  });

