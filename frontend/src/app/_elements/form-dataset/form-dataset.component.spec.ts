import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FormDatasetComponent } from './form-dataset.component';

describe('FormDatasetComponent', () => {
  let component: FormDatasetComponent;
  let fixture: ComponentFixture<FormDatasetComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ FormDatasetComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(FormDatasetComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
