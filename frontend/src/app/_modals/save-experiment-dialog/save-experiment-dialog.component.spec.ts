import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SaveExperimentDialogComponent } from './save-experiment-dialog.component';

describe('SaveExperimentDialogComponent', () => {
  let component: SaveExperimentDialogComponent;
  let fixture: ComponentFixture<SaveExperimentDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ SaveExperimentDialogComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(SaveExperimentDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
