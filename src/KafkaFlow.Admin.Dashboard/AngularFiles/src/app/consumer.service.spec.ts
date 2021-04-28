import { TestBed } from '@angular/core/testing';

import { ConsumerService } from './consumer.service';

describe('ConsumersService', () => {
  let service: ConsumerService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ConsumerService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
