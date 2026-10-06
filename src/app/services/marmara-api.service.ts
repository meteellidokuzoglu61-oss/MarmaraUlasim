import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Sefer {
  id: number;
  seferKodu: string;
  hatKodu: string;
  servisKodu: string | null;
  varisYonu: string | null;
  shapeId: string | null;
  kaynak: string;
  aktif: boolean;
}

export interface GuzergahNoktasi {
  sira: number;
  enlem: number;
  boylam: number;
  mesafe: number | null;
}

export interface SeferGuzergahi {
  seferKodu: string;
  hatKodu: string;
  shapeId: string;
  noktaSayisi: number;
  noktalar: GuzergahNoktasi[];
}

export interface Durak {
  id: number;
  durakKodu: string;
  ad: string;
  kaynak: string;
  enlem: number;
  boylam: number;
  aktif: boolean;
  ilceId: number | null;
  ilce: string | null;
  mahalleId: number | null;
  mahalle: string | null;
  il: string | null;
  plakaKodu: number | null;
}

@Injectable({
  providedIn: 'root'
})
export class MarmaraApiService {

  private readonly http = inject(HttpClient);

  private readonly apiUrl = 'http://localhost:5197/api';

  getSeferler(
    sayfa: number = 1,
    sayfaBoyutu: number = 20
  ): Observable<Sefer[]> {

    return this.http.get<Sefer[]>(
      `${this.apiUrl}/Duraklar/seferler`,
      {
        params: {
          sayfa,
          sayfaBoyutu
        }
      }
    );
  }

  getSeferGuzergahi(
    seferKodu: string
  ): Observable<SeferGuzergahi> {

    return this.http.get<SeferGuzergahi>(
      `${this.apiUrl}/Duraklar/sefer/${encodeURIComponent(seferKodu)}/guzergah`
    );
  }

  getDuraklar(
    sayfa: number = 1,
    sayfaBoyutu: number = 100
  ): Observable<any> {

    return this.http.get<any>(
      `${this.apiUrl}/Duraklar`,
      {
        params: {
          sayfa,
          sayfaBoyutu
        }
      }
    );
  }
}

