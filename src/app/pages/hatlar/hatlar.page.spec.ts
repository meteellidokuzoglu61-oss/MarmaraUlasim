import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HatlarPage } from './hatlar.page';

describe('HatlarPage', () => {
  let component: HatlarPage;
  let fixture: ComponentFixture<HatlarPage>;

  beforeEach(() => {
    fixture = TestBed.createComponent(HatlarPage);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
