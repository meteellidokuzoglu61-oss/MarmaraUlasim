import { Component, OnInit } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import {
  IonHeader, IonToolbar, IonTitle, IonContent, IonIcon,
  IonSearchbar, IonBadge, IonCard, IonCardContent,
  IonSpinner, IonText
} from '@ionic/angular';
import { addIcons } from 'ionicons';
import { mapOutline, businessOutline, chevronForwardOutline, searchOutline } from 'ionicons/icons';
import { Il, IlService } from '../../services/il.service';

@Component({
  selector: 'app-iller',
  templateUrl: './iller.page.html',
  styleUrls: ['./iller.page.scss'],
  standalone: true,
  imports: [
    CommonModule, FormsModule, IonHeader, IonToolbar, IonTitle,
    IonContent, IonIcon, IonSearchbar, IonBadge, IonCard,
    IonCardContent, IonSpinner, IonText
  ]
})
export class IllerPage implements OnInit {
  searchText = '';
  cities: Il[] = [];
  filteredCities: Il[] = [];
  yukleniyor = true;
  hata = '';

  constructor(
    private readonly router: Router,
    private readonly ilService: IlService
  ) {
    addIcons({
      mapOutline,
      businessOutline,
      chevronForwardOutline,
      searchOutline
    });
  }

  ngOnInit(): void {
    this.illeriGetir();
  }

  illeriGetir(): void {
    this.yukleniyor = true;
    this.hata = '';

    this.ilService.getIller().subscribe({
      next: iller => {
        this.cities = iller;
        this.filteredCities = iller;
        this.yukleniyor = false;
      },
      error: (error: unknown) => {
        console.error('İller alınamadı:', error);
        if (error instanceof HttpErrorResponse && error.status === 0) {
          this.hata = 'API sunucusuna ulaşılamadı. API çalışıyor mu ve adresi doğru mu kontrol edin.';
        } else if (error instanceof HttpErrorResponse && error.status > 0) {
          this.hata = `İller alınamadı (HTTP ${error.status}). API günlüklerini kontrol edin.`;
        } else {
          this.hata = 'İstek zaman aşımına uğradı. API yanıt vermiyor olabilir.';
        }
        this.yukleniyor = false;
      }
    });
  }

  searchCities(): void {
    const search = this.searchText.toLocaleLowerCase('tr-TR').trim();
    this.filteredCities = this.cities.filter(city =>
      city.ad.toLocaleLowerCase('tr-TR').includes(search)
    );
  }

  openCity(id: number): void {
    this.router.navigate(['/il-detay', id]);
  }
}
