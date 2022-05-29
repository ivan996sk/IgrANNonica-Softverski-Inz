import { TestBed } from '@angular/core/testing';

import { PredictorsService } from './predictors.service';

describe('PredictorsService', () => {
  let service: PredictorsService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(PredictorsService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
