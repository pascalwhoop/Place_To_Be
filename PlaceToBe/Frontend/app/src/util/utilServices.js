'use strict';

/**
 * @ngdoc service
 * @name placeToBe.service:toastNotifyService
 * @description A Service that exposes functions to show toasts to the user. this way we can show notifications to the user from services or in response to backend responses.
 */
angular.module('placeToBe').factory('toastNotifyService', function ($rootScope, $mdToast) {


  /**
   *
   * @param message the notification message
   * @param position the position of the toast. defaults to bottom left. can be a combination of top,left,right,bottom e.g. "top left"
   * @param delay the delay how long the toast will be shown. Defaults to 3sec
   */
  var showNotifyToast = function (message, position, delay) {
    if (!position) position = "bottom left";
    if (!message) return;
    if (!delay) delay = 3000;

    $mdToast.show(
      $mdToast.simple()
        .content(message)
        .position(position)
        .hideDelay(delay)
    )
  };

  /**
   * displays a toast with an action to perform. if the user clicks the button the callback action is called otherwise, the toast disapperas after a while
   * @param action the function to perform as a callback if the user clicks the button
   * @param actionButtonText the text for the button
   * @param message the notification message
   * @param position the position of the toast. defaults to bottom left. can be a combination of top,left,right,bottom e.g. "top left"
   * @param delay the delay how long the toast will be shown. Defaults to 5sec
   */
  var showActionToast = function (action, actionButtonText, message, position, delay) {
    if (!message || !action || !actionButtonText) return;
    if (!position) position = "bottom left";
    if (!delay) delay = 5000;


    var toast = $mdToast.simple()
      .content(message)
      .position(position)
      .hideDelay(delay)
      .action(actionButtonText);
    $mdToast.show(toast).then(action);

  };

  return {
    showNotifyToast: showNotifyToast,
    showActionToast: showActionToast
  };
});
