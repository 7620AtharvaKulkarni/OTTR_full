import { TestBed } from '@angular/core/testing';

import { OttrService } from './ottr.service';

describe('HttpService', () => {
  let service: OttrService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(OttrService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
