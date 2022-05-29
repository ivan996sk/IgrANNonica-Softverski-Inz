import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MissingvaluesDialogComponent } from './missingvalues-dialog.component';

describe('MissingvaluesDialogComponent', () => {
  let component: MissingvaluesDialogComponent;
  let fixture: ComponentFixture<MissingvaluesDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ MissingvaluesDialogComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(MissingvaluesDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
