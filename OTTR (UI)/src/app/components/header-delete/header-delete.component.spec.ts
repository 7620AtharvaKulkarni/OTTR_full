import { ComponentFixture, TestBed } from '@angular/core/testing';

import { HeaderDeleteComponent } from './header-delete.component';

describe('HeaderDeleteComponent', () => {
  let component: HeaderDeleteComponent;
  let fixture: ComponentFixture<HeaderDeleteComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ HeaderDeleteComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(HeaderDeleteComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
