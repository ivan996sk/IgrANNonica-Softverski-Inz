import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FormPredictorComponent } from './form-predictor.component';

describe('FormPredictorComponent', () => {
  let component: FormPredictorComponent;
  let fixture: ComponentFixture<FormPredictorComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ FormPredictorComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(FormPredictorComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
