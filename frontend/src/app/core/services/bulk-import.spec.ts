import { TestBed } from '@angular/core/testing';

import { BulkImport } from './bulk-import';

describe('BulkImport', () => {
  let service: BulkImport;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(BulkImport);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
