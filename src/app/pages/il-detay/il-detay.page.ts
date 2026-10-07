import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import {
  IonHeader, IonToolbar, IonTitle, IonContent, IonButtons,
  IonBackButton, IonList, IonItem, IonLabel, IonBadge,
  IonSpinner, IonText
} from '@ionic/angular';
import { IlDetay, IlService } from '../../services/il.service';

@Component({
  selector: 'app-il-detay',
  templateUrl: './il-detay.page.html',
  styleUrls: ['./il-detay.page.scss'],
  standalone: true,
  imports: [
    CommonModule, IonHeader, IonToolbar, IonTitle, IonContent,
    IonButtons, IonBackButton, IonList, IonItem, IonLabel,
    IonBadge, IonSpinner, IonText
  ]
})
export class IlDetayPage implements OnInit {
  il: IlDetay | null = null;
  yukleniyor = true;
  hata = '';

  constructor(
    private readonly route: ActivatedRoute,
    private readonly router: Router,
    private readonly ilService: IlService
  ) {}

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    if (!id) {
      this.hata = 'Geçersiz il.';
      this.yukleniyor = false;
      return;
    }

    this.ilService.getIl(id).subscribe({
      next: il => {
        this.il = il;
        this.yukleniyor = false;
      },
      error: error => {
        console.error('İl detayı alınamadı:', error);
        this.hata = 'İl bilgileri alınamadı.';
        this.yukleniyor = false;
      }
    });
  }

  duraklariAc(ilceId: number): void {
    this.router.navigate(['/duraklar'], { queryParams: { ilceId } });
  }
}
