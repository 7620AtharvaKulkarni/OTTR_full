import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { OttrListComponent } from './ottr-list.component';

describe('OttrListComponent', () => {
  let component: OttrListComponent;
  let fixture: ComponentFixture<OttrListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ OttrListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(OttrListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
