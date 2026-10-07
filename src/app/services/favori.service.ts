import { Injectable } from '@angular/core';

export interface Favori {
  tur: 'hat' | 'durak';
  kod: string;
  ad: string;
}

@Injectable({ providedIn: 'root' })
export class FavoriService {
  private readonly key = 'marmara-ulasim-favoriler';

  getFavoriler(): Favori[] {
    try {
      return JSON.parse(localStorage.getItem(this.key) ?? '[]');
    } catch {
      return [];
    }
  }

  ekle(favori: Favori): void {
    const favoriler = this.getFavoriler();
    if (!favoriler.some(x => x.tur === favori.tur && x.kod === favori.kod)) {
      favoriler.push(favori);
      localStorage.setItem(this.key, JSON.stringify(favoriler));
    }
  }

  kaldir(tur: 'hat' | 'durak', kod: string): void {
    const favoriler = this.getFavoriler()
      .filter(x => !(x.tur === tur && x.kod === kod));
    localStorage.setItem(this.key, JSON.stringify(favoriler));
  }

  favoriMi(tur: 'hat' | 'durak', kod: string): boolean {
    return this.getFavoriler().some(x => x.tur === tur && x.kod === kod);
  }
}
