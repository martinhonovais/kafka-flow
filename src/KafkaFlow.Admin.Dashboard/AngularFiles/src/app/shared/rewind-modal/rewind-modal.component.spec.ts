import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RewindModalComponent } from './rewind-modal.component';

describe('RewindModalComponent', () => {
  let component: RewindModalComponent;
  let fixture: ComponentFixture<RewindModalComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ RewindModalComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(RewindModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
