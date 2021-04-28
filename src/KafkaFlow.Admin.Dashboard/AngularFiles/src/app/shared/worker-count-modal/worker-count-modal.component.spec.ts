import { ComponentFixture, TestBed } from '@angular/core/testing';

import { WorkerCountModalComponent } from './worker-count-modal.component';

describe('WorkerCountModalComponent', () => {
  let component: WorkerCountModalComponent;
  let fixture: ComponentFixture<WorkerCountModalComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ WorkerCountModalComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(WorkerCountModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
