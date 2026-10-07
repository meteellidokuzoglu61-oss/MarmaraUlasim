import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Durak {
  id: number;
  durakKodu: string;
  ad: string;
  kaynak: string;
  enlem: number;
  boylam: number;
  ilceId?: number | null;
  mahalleId?: number | null;
  ilce?: {
    id: number;
    ad: string;
  } | null;
  mahalle?: {
    id: number;
    ad: string;
  } | null;
  aktif: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class DurakService {
  private readonly apiUrl = 'http://localhost:5197/api/duraklar';

  constructor(private http: HttpClient) {}

  getDuraklar(): Observable<Durak[]> {
    return this.http.get<Durak[]>(this.apiUrl);
  }
}