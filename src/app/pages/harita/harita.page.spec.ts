import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HaritaPage } from './harita.page';

describe('HaritaPage', () => {
  let component: HaritaPage;
  let fixture: ComponentFixture<HaritaPage>;

  beforeEach(() => {
    fixture = TestBed.createComponent(HaritaPage);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
