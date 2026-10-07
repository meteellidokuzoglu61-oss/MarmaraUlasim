import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Il {
  id: number;
  ad: string;
  plakaKodu: number;
  ilceSayisi: number;
  durakSayisi: number;
}

export interface IlDetay {
  id: number;
  ad: string;
  plakaKodu: number;
  ilceler: Ilce[];
}

export interface Ilce {
  id: number;
  apiId?: number;
  ad: string;
  mahalleSayisi: number;
  durakSayisi: number;
}

@Injectable({ providedIn: 'root' })
export class IlService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = 'http://localhost:5197/api';

  getIller(): Observable<Il[]> {
    return this.http.get<Il[]>(`${this.apiUrl}/Iller`);
  }

  getIl(id: number): Observable<IlDetay> {
    return this.http.get<IlDetay>(`${this.apiUrl}/Iller/${id}`);
  }

  getIlceler(ilId: number): Observable<Ilce[]> {
    return this.http.get<Ilce[]>(`${this.apiUrl}/Ilceler/il/${ilId}`);
  }
}
