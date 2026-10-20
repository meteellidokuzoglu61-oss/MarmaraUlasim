import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import {
  IonHeader, IonToolbar, IonTitle, IonContent, IonButtons,
  IonBackButton, IonList, IonItem, IonLabel, IonSpinner, IonText
} from '@ionic/angular';
import { IlceDetay, IlService } from '../../services/il.service';

@Component({
  selector: 'app-ilce-detay',
  templateUrl: './ilce-detay.page.html',
  styleUrls: ['./ilce-detay.page.scss'],
  standalone: true,
  imports: [
    CommonModule, IonHeader, IonToolbar, IonTitle, IonContent,
    IonButtons, IonBackButton, IonList, IonItem, IonLabel,
    IonSpinner, IonText
  ]
})
export class IlceDetayPage implements OnInit {
  ilce: IlceDetay | null = null;
  yukleniyor = true;
  hata = '';

  constructor(
    private readonly route: ActivatedRoute,
    private readonly router: Router,
    private readonly ilService: IlService
  ) {}

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    if (!Number.isInteger(id) || id <= 0) {
      this.hata = 'Geçersiz ilçe.';
      this.yukleniyor = false;
      return;
    }

    this.ilService.getIlce(id).subscribe({
      next: ilce => {
        this.ilce = ilce;
        this.yukleniyor = false;
      },
      error: error => {
        console.error('İlçe bilgileri alınamadı:', error);
        this.hata = 'İlçe bilgileri alınamadı. API bağlantısını kontrol et.';
        this.yukleniyor = false;
      }
    });
  }

  duraklariAc(): void {
    if (!this.ilce) return;
    this.router.navigate(['/duraklar'], { queryParams: { ilceId: this.ilce.id } });
  }
}
