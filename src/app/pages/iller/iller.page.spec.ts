import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { of } from 'rxjs';
import { IllerPage } from './iller.page';
import { IlService } from '../../services/il.service';

describe('IllerPage', () => {
  let component: IllerPage;
  let fixture: ComponentFixture<IllerPage>;

  const sampleIller = [
    { id: 1, ad: 'Balıkesir', plakaKodu: 10, ilceSayisi: 20, durakSayisi: 12 },
    { id: 2, ad: 'Bursa', plakaKodu: 16, ilceSayisi: 17, durakSayisi: 30 },
  ];

  const ilServiceMock = {
    getIller: () => of(sampleIller),
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [IllerPage],
      providers: [
        provideRouter([]),
        { provide: IlService, useValue: ilServiceMock },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(IllerPage);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create and load provinces', () => {
    expect(component).toBeTruthy();
    expect(component.yukleniyor).toBe(false);
    expect(component.cities.length).toBe(2);
  });

  it('should filter provinces using Turkish casing', () => {
    component.searchText = 'BURSA';
    component.searchCities();
    expect(component.filteredCities.map(city => city.ad)).toEqual(['Bursa']);
  });
});
