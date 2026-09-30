import { ComponentFixture, TestBed } from '@angular/core/testing';
import { DuraklarPage } from './duraklar.page';

describe('DuraklarPage', () => {
  let component: DuraklarPage;
  let fixture: ComponentFixture<DuraklarPage>;

  beforeEach(() => {
    fixture = TestBed.createComponent(DuraklarPage);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
