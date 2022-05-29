import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UpdateExperimentDialogComponent } from './update-experiment-dialog.component';

describe('UpdateExperimentDialogComponent', () => {
  let component: UpdateExperimentDialogComponent;
  let fixture: ComponentFixture<UpdateExperimentDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ UpdateExperimentDialogComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(UpdateExperimentDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
