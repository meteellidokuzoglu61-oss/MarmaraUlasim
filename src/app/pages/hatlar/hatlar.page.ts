import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import {
  IonContent, IonHeader, IonTitle, IonToolbar, IonList, IonItem,
  IonLabel, IonSpinner, IonText, IonSearchbar, IonButton, IonIcon
} from '@ionic/angular';
import { addIcons } from 'ionicons';
import { heartOutline, heart } from 'ionicons/icons';
import { HatService, Hat } from '../../services/hat.service';
import { FavoriService } from '../../services/favori.service';

@Component({
  selector: 'app-hatlar',
  templateUrl: './hatlar.page.html',
  styleUrls: ['./hatlar.page.scss'],
  standalone: true,
  imports: [
    IonContent, IonHeader, IonTitle, IonToolbar, IonList, IonItem,
    IonLabel, IonSpinner, IonText, IonSearchbar, IonButton, IonIcon,
    CommonModule, FormsModule
  ]
})
export class HatlarPage implements OnInit {
  hatlar: Hat[] = [];
  filtreliHatlar: Hat[] = [];
  arama = '';
  yukleniyor = true;
  hata = '';

  constructor(
    private readonly hatService: HatService,
    private readonly favoriService: FavoriService
  ) {
    addIcons({ heartOutline, heart });
  }

  ngOnInit(): void {
    this.hatlariGetir();
  }

  hatlariGetir(): void {
    this.yukleniyor = true;
    this.hata = '';

    this.hatService.getHatlar().subscribe({
      next: data => {
        this.hatlar = data;
        this.filtrele();
        this.yukleniyor = false;
      },
      error: error => {
        console.error('Hatlar alınamadı:', error);
        this.hata = 'Hatlar yüklenirken bir hata oluştu.';
        this.yukleniyor = false;
      }
    });
  }

  filtrele(): void {
    const metin = this.arama.toLocaleLowerCase('tr-TR').trim();
    this.filtreliHatlar = this.hatlar.filter(hat =>
      hat.hatKodu.toLocaleLowerCase('tr-TR').includes(metin) ||
      hat.ad.toLocaleLowerCase('tr-TR').includes(metin) ||
      (hat.aciklama ?? '').toLocaleLowerCase('tr-TR').includes(metin)
    );
  }

  favoriMi(hat: Hat): boolean {
    return this.favoriService.favoriMi('hat', hat.hatKodu);
  }

  favoriDegistir(hat: Hat): void {
    if (this.favoriMi(hat)) {
      this.favoriService.kaldir('hat', hat.hatKodu);
    } else {
      this.favoriService.ekle({
        tur: 'hat',
        kod: hat.hatKodu,
        ad: hat.ad
      });
    }
  }
}
