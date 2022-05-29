import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PointLinechartComponent } from './point-linechart.component';

describe('PointLinechartComponent', () => {
  let component: PointLinechartComponent;
  let fixture: ComponentFixture<PointLinechartComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PointLinechartComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PointLinechartComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
