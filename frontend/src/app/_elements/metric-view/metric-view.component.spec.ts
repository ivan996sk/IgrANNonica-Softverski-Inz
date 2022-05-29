import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MetricViewComponent } from './metric-view.component';

describe('MetricViewComponent', () => {
  let component: MetricViewComponent;
  let fixture: ComponentFixture<MetricViewComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ MetricViewComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(MetricViewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
