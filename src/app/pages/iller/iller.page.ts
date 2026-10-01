import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';

import {
  IonHeader,
  IonToolbar,
  IonTitle,
  IonContent,
  IonIcon,
  IonSearchbar,
  IonBadge,
  IonCard,
  IonCardContent
} from '@ionic/angular';

interface City {
  id: number;
  name: string;
  districtCount: number;
}

@Component({
  selector: 'app-iller',
  templateUrl: './iller.page.html',
  styleUrls: ['./iller.page.scss'],
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,

    IonHeader,
    IonToolbar,
    IonTitle,
    IonContent,
    IonIcon,
    IonSearchbar,
    IonBadge,
    IonCard,
    IonCardContent
  ]
})
export class IllerPage {

  searchText = '';

  cities: City[] = [];

  filteredCities: City[] = [];

  constructor(private router: Router) {}

  searchCities() {
    const search = this.searchText
      .toLocaleLowerCase('tr-TR')
      .trim();

    this.filteredCities = this.cities.filter(city =>
      city.name
        .toLocaleLowerCase('tr-TR')
        .includes(search)
    );
  }

  openCity(id: number) {
    this.router.navigate(['/il-detay', id]);
  }
}