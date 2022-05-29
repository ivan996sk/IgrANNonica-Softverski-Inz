import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EncodingDialogComponent } from './encoding-dialog.component';

describe('EncodingDialogComponent', () => {
  let component: EncodingDialogComponent;
  let fixture: ComponentFixture<EncodingDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ EncodingDialogComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(EncodingDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
