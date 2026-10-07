import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import {
  IonContent, IonHeader, IonTitle, IonToolbar, IonList,
  IonItem, IonLabel, IonSpinner, IonText, IonSearchbar,
  IonButton, IonIcon
} from '@ionic/angular';
import { addIcons } from 'ionicons';
import { heartOutline, heart } from 'ionicons/icons';
import { DurakService, Durak } from '../../services/durak.service';
import { FavoriService } from '../../services/favori.service';

@Component({
  selector: 'app-duraklar',
  templateUrl: './duraklar.page.html',
  styleUrls: ['./duraklar.page.scss'],
  standalone: true,
  imports: [
    IonContent, IonHeader, IonTitle, IonToolbar, IonList, IonItem,
    IonLabel, IonSpinner, IonText, IonSearchbar, IonButton, IonIcon,
    CommonModule, FormsModule
  ]
})
export class DuraklarPage implements OnInit {
  duraklar: Durak[] = [];
  arama = '';
  ilceId?: number;
  toplam = 0;
  yukleniyor = true;
  hata = '';

  constructor(
    private readonly durakService: DurakService,
    private readonly favoriService: FavoriService,
    private readonly route: ActivatedRoute
  ) {
    addIcons({ heartOutline, heart });
  }

  ngOnInit(): void {
    this.ilceId = Number(this.route.snapshot.queryParamMap.get('ilceId')) || undefined;
    this.duraklariGetir();
  }

  duraklariGetir(): void {
    this.yukleniyor = true;
    this.hata = '';

    this.durakService.getDuraklar(this.arama, this.ilceId).subscribe({
      next: sonuc => {
        this.duraklar = sonuc.veriler;
        this.toplam = sonuc.toplam;
        this.yukleniyor = false;
      },
      error: error => {
        console.error('Duraklar alınamadı:', error);
        this.hata = 'Duraklar yüklenirken bir hata oluştu.';
        this.yukleniyor = false;
      }
    });
  }

  favoriMi(durak: Durak): boolean {
    return this.favoriService.favoriMi('durak', durak.durakKodu);
  }

  favoriDegistir(durak: Durak): void {
    if (this.favoriMi(durak)) {
      this.favoriService.kaldir('durak', durak.durakKodu);
    } else {
      this.favoriService.ekle({
        tur: 'durak',
        kod: durak.durakKodu,
        ad: durak.ad
      });
    }
  }
}
