'use strict';

/**
 * @ngdoc service
 * @name placeToBe.configService
 * @description
 * # gaussianService code taken from https://github.com/errcw/gaussian
 * Service in the placeToBe.
 */
angular.module('placeToBe')
  .factory('configService', function () {
    return{
      BASE_URL: "https://placetobe-koeln.azurewebsites.net/api",
      STRINGS: {
        backend_error: "An error occured. Try again later",
        reset_email_sent: "Reset Email sent!",
        registration_success: 'Registration successful. Check your email!',
        registration_error: 'We had a problem with the registration. Try a different email or come back later!'
      },
      //BASE_URL: "https://localhost:18172/api",
      MAPS_STYLE_ARRAY: [
        {
          "featureType": "administrative.locality",
          "elementType": "all",
          "stylers": [
            {
              "hue": "#2c2e33"
            },
            {
              "saturation": 7
            },
            {
              "lightness": 19
            },
            {
              "visibility": "on"
            }
          ]
        },
        {
          "featureType": "landscape",
          "elementType": "all",
          "stylers": [
            {
              "hue": "#ffffff"
            },
            {
              "saturation": -100
            },
            {
              "lightness": 100
            },
            {
              "visibility": "simplified"
            }
          ]
        },
        {
          "featureType": "poi",
          "elementType": "all",
          "stylers": [
            {
              "hue": "#ffffff"
            },
            {
              "saturation": -100
            },
            {
              "lightness": 100
            },
            {
              "visibility": "off"
            }
          ]
        },
        {
          "featureType": "road",
          "elementType": "geometry",
          "stylers": [
            {
              "hue": "#bbc0c4"
            },
            {
              "saturation": -93
            },
            {
              "lightness": 31
            },
            {
              "visibility": "simplified"
            }
          ]
        },
        {
          "featureType": "road",
          "elementType": "labels",
          "stylers": [
            {
              "hue": "#bbc0c4"
            },
            {
              "saturation": -93
            },
            {
              "lightness": 31
            },
            {
              "visibility": "on"
            }
          ]
        },
        {
          "featureType": "road.arterial",
          "elementType": "labels",
          "stylers": [
            {
              "hue": "#bbc0c4"
            },
            {
              "saturation": -93
            },
            {
              "lightness": -2
            },
            {
              "visibility": "simplified"
            }
          ]
        },
        {
          "featureType": "road.local",
          "elementType": "geometry",
          "stylers": [
            {
              "hue": "#e9ebed"
            },
            {
              "saturation": -90
            },
            {
              "lightness": -8
            },
            {
              "visibility": "simplified"
            }
          ]
        },
        {
          "featureType": "transit",
          "elementType": "all",
          "stylers": [
            {
              "hue": "#e9ebed"
            },
            {
              "saturation": 10
            },
            {
              "lightness": 69
            },
            {
              "visibility": "on"
            }
          ]
        },
        {
          "featureType": "water",
          "elementType": "all",
          "stylers": [
            {
              "hue": "#e9ebed"
            },
            {
              "saturation": -78
            },
            {
              "lightness": 67
            },
            {
              "visibility": "simplified"
            }
          ]
        }
      ]

    }

  });
