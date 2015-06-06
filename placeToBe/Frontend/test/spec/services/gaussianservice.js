'use strict';

describe('Service: gaussianService', function () {

  // load the service's module
  beforeEach(module('frontendApp'));

  // instantiate service
  var gaussianService;
  beforeEach(inject(function (_gaussianService_) {
    gaussianService = _gaussianService_;
  }));

  it('should do something', function () {
    expect(!!gaussianService).toBe(true);
  });

});
