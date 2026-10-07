import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import {
  IonContent,
  IonHeader,
  IonTitle,
  IonToolbar,
  IonList,
  IonItem,
  IonLabel,
  IonSpinner,
  IonText
} from '@ionic/angular';
import { HatService, Hat } from '../../services/hat.service';

@Component({
  selector: 'app-hatlar',
  templateUrl: './hatlar.page.html',
  styleUrls: ['./hatlar.page.scss'],
  imports: [
    IonContent,
    IonHeader,
    IonTitle,
    IonToolbar,
    IonList,
    IonItem,
    IonLabel,
    IonSpinner,
    IonText,
    CommonModule,
    FormsModule
  ]
})
export class HatlarPage implements OnInit {
  hatlar: Hat[] = [];
  yukleniyor = true;
  hata = '';

  constructor(private hatService: HatService) {}

  ngOnInit(): void {
    this.hatlariGetir();
  }

  hatlariGetir(): void {
    this.yukleniyor = true;
    this.hata = '';

    this.hatService.getHatlar().subscribe({
      next: (data) => {
        this.hatlar = data;
        this.yukleniyor = false;
      },
      error: (error) => {
        console.error('Hatlar alınamadı:', error);
        this.hata = 'Hatlar yüklenirken bir hata oluştu.';
        this.yukleniyor = false;
      }
    });
  }
}