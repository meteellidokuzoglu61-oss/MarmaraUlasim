import { ComponentFixture, TestBed } from '@angular/core/testing';
import { IlDetayPage } from './il-detay.page';

describe('IlDetayPage', () => {
  let component: IlDetayPage;
  let fixture: ComponentFixture<IlDetayPage>;

  beforeEach(() => {
    fixture = TestBed.createComponent(IlDetayPage);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
