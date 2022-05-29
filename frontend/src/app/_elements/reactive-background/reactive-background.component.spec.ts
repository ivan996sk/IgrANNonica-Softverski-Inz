import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ReactiveBackgroundComponent } from './reactive-background.component';

describe('ReactiveBackgroundComponent', () => {
  let component: ReactiveBackgroundComponent;
  let fixture: ComponentFixture<ReactiveBackgroundComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ReactiveBackgroundComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ReactiveBackgroundComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
