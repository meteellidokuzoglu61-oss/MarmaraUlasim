import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  IonContent, IonHeader, IonTitle, IonToolbar, IonList, IonItem,
  IonLabel, IonButton, IonIcon, IonText
} from '@ionic/angular';
import { addIcons } from 'ionicons';
import { heart, trashOutline } from 'ionicons/icons';
import { Favori, FavoriService } from '../../services/favori.service';

@Component({
  selector: 'app-favoriler',
  templateUrl: './favoriler.page.html',
  styleUrls: ['./favoriler.page.scss'],
  standalone: true,
  imports: [
    CommonModule, IonContent, IonHeader, IonTitle, IonToolbar,
    IonList, IonItem, IonLabel, IonButton, IonIcon, IonText
  ]
})
export class FavorilerPage implements OnInit {
  favoriler: Favori[] = [];

  constructor(private readonly favoriService: FavoriService) {
    addIcons({ heart, trashOutline });
  }

  ngOnInit(): void {
    this.yukle();
  }

  ionViewWillEnter(): void {
    this.yukle();
  }

  yukle(): void {
    this.favoriler = this.favoriService.getFavoriler();
  }

  sil(favori: Favori): void {
    this.favoriService.kaldir(favori.tur, favori.kod);
    this.yukle();
  }
}
