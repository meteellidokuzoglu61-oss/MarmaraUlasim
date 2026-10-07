import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Durak {
  id: number;
  durakKodu: string;
  ad: string;
  kaynak: string;
  enlem: number;
  boylam: number;
  ilceId: number | null;
  ilce: string | null;
  mahalleId: number | null;
  mahalle: string | null;
  il: string | null;
  plakaKodu: number | null;
  aktif: boolean;
}

export interface DurakSonuc {
  toplam: number;
  sayfa: number;
  sayfaBoyutu: number;
  veriler: Durak[];
}

@Injectable({ providedIn: 'root' })
export class DurakService {
  private readonly apiUrl = 'http://localhost:5197/api/duraklar';

  constructor(private readonly http: HttpClient) {}

  getDuraklar(
    arama = '',
    ilceId?: number,
    sayfa = 1,
    sayfaBoyutu = 100
  ): Observable<DurakSonuc> {
    let params = new HttpParams()
      .set('sayfa', sayfa)
      .set('sayfaBoyutu', sayfaBoyutu);

    if (arama.trim()) params = params.set('arama', arama.trim());
    if (ilceId) params = params.set('ilceId', ilceId);

    return this.http.get<DurakSonuc>(this.apiUrl, { params });
  }
}
