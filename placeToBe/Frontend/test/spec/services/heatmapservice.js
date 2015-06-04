'use strict';

describe('Service: heatmapService', function () {

  // load the service's module
  beforeEach(module('frontendApp'));

  // instantiate service
  var heatmapService;
  beforeEach(inject(function (_heatmapService_) {
    heatmapService = _heatmapService_;
  }));

  it('should do something', function () {
    expect(!!heatmapService).toBe(true);
  });

});
