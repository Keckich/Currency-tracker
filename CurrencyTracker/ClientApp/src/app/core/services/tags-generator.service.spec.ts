import { TestBed } from '@angular/core/testing';

import { TagsGeneratorService } from './tags-generator.service';

describe('TagsGeneratorService', () => {
  let service: TagsGeneratorService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(TagsGeneratorService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
