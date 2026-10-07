import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Hat {
  id: number;
  hatKodu: string;
  ad: string;
  tip?: string | null;
  aciklama?: string | null;
  kaynak: string;
  aktif: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class HatService {
  private readonly apiUrl = 'http://localhost:5197/api/hatlar';

  constructor(private http: HttpClient) {}

  getHatlar(): Observable<Hat[]> {
    return this.http.get<Hat[]>(this.apiUrl);
  }
}