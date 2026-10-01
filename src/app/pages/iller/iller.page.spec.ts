import { ComponentFixture, TestBed } from '@angular/core/testing';
import { IllerPage } from './iller.page';

describe('IllerPage', () => {
  let component: IllerPage;
  let fixture: ComponentFixture<IllerPage>;

  beforeEach(() => {
    fixture = TestBed.createComponent(IllerPage);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
