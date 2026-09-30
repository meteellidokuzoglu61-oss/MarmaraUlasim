import { ComponentFixture, TestBed } from '@angular/core/testing';
import { FavorilerPage } from './favoriler.page';

describe('FavorilerPage', () => {
  let component: FavorilerPage;
  let fixture: ComponentFixture<FavorilerPage>;

  beforeEach(() => {
    fixture = TestBed.createComponent(FavorilerPage);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
