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
import { DurakService, Durak } from '../../services/durak.service';

@Component({
  selector: 'app-duraklar',
  templateUrl: './duraklar.page.html',
  styleUrls: ['./duraklar.page.scss'],
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
export class DuraklarPage implements OnInit {
  duraklar: Durak[] = [];
  yukleniyor = true;
  hata = '';

  constructor(private durakService: DurakService) {}

  ngOnInit(): void {
    this.duraklariGetir();
  }

  duraklariGetir(): void {
    this.yukleniyor = true;
    this.hata = '';

    this.durakService.getDuraklar().subscribe({
      next: (data) => {
        this.duraklar = data;
        this.yukleniyor = false;
      },
      error: (error) => {
        console.error('Duraklar alınamadı:', error);
        this.hata = 'Duraklar yüklenirken bir hata oluştu.';
        this.yukleniyor = false;
      }
    });
  }
}