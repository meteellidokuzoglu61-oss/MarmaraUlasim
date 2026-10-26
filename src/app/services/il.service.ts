import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, timeout } from 'rxjs';
import { environment } from '../../environments/environment';

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

export interface IlceDetay {
  id: number;
  ad: string;
  ilId: number;
  il: string | null;
  mahalleler: Array<{ id: number; ad: string }>;
}

@Injectable({ providedIn: 'root' })
export class IlService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = environment.apiUrl;

  getIller(): Observable<Il[]> {
    // Yanıt gelmezse sayfanın sonsuza kadar yükleniyor durumunda kalmasını önle.
    return this.http.get<Il[]>(`${this.apiUrl}/Iller`).pipe(
      timeout({ first: 15000 })
    );
  }

  getIl(id: number): Observable<IlDetay> {
    return this.http.get<IlDetay>(`${this.apiUrl}/Iller/${id}`).pipe(timeout({ first: 15000 }));
  }

  getIlceler(ilId: number): Observable<Ilce[]> {
    return this.http.get<Ilce[]>(`${this.apiUrl}/Ilceler/il/${ilId}`);
  }

  getIlce(id: number): Observable<IlceDetay> {
    return this.http.get<IlceDetay>(`${this.apiUrl}/Ilceler/${id}`).pipe(timeout({ first: 15000 }));
  }
}
