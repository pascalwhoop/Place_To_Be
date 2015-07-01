'use strict';

/**
 * @ngdoc service
 * @name placeToBe.configService
 * @description
 * # gaussianService code taken from https://github.com/errcw/gaussian
 * Service in the frontendApp.
 */
angular.module('placeToBe')
  .factory('configService', function () {
    return{
      BASE_URL: "https://placetobe-koeln.azurewebsites.net/api",

    }

  });
