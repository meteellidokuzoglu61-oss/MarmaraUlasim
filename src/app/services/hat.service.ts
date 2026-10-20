import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface Hat {
  id: number;
  hatKodu: string;
  ad: string;
  tip?: string | null;
  aciklama?: string | null;
  kaynak: string;
  aktif: boolean;
}

@Injectable({ providedIn: 'root' })
export class HatService {
  private readonly apiUrl = `${environment.apiUrl}/Hatlar`;

  constructor(private readonly http: HttpClient) {}

  getHatlar(): Observable<Hat[]> {
    return this.http.get<Hat[]>(this.apiUrl);
  }
}
