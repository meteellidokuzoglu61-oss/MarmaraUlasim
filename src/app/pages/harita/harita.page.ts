import {
  AfterViewInit,
  Component,
  OnDestroy
} from '@angular/core';

import {
  IonHeader,
  IonToolbar,
  IonTitle,
  IonContent,
  IonButton,
  IonButtons,
  IonIcon
} from '@ionic/angular';

import { addIcons } from 'ionicons';

import {
  mapOutline,
  refreshOutline
} from 'ionicons/icons';

import * as L from 'leaflet';

import {
  MarmaraApiService,
  Sefer,
  SeferGuzergahi
} from '../../services/marmara-api.service';

@Component({
  selector: 'app-harita',
  templateUrl: './harita.page.html',
  styleUrls: ['./harita.page.scss'],
  standalone: true,
  imports: [
    IonHeader,
    IonToolbar,
    IonTitle,
    IonContent,
    IonButton,
    IonButtons,
    IonIcon
  ]
})
export class HaritaPage implements AfterViewInit, OnDestroy {

  private map?: L.Map;

  private guzergahCizgisi?: L.Polyline;

  seferler: Sefer[] = [];

  seciliSefer: Sefer | null = null;

  yukleniyor = false;

  hata = '';

  constructor(
    private readonly api: MarmaraApiService
  ) {
    addIcons({
      mapOutline,
      refreshOutline
    });
  }

  ngAfterViewInit(): void {
    setTimeout(() => {
      this.haritaOlustur();
      this.seferleriGetir();
    }, 100);
  }

  ngOnDestroy(): void {
    this.map?.remove();
    this.map = undefined;
  }

  private haritaOlustur(): void {
    const element = document.getElementById('harita');

    if (!element) {
      this.hata = 'Harita alanı oluşturulamadı.';
      return;
    }

    this.map = L.map(element, {
      center: [40.765, 29.940],
      zoom: 11,
      zoomControl: true
    });

    L.tileLayer(
      'https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png',
      {
        maxZoom: 19,
        attribution: '&copy; OpenStreetMap contributors'
      }
    ).addTo(this.map);

    setTimeout(() => {
      this.map?.invalidateSize();
    }, 300);
  }

  seferleriGetir(): void {
    this.yukleniyor = true;
    this.hata = '';

    this.api.getSeferler(1, 50).subscribe({
      next: (sonuc) => {
        this.seferler = sonuc;
        this.yukleniyor = false;

        if (this.seferler.length > 0) {
          this.seferSec(this.seferler[0]);
        }
      },

      error: (error) => {
        console.error('Seferler alınamadı:', error);
        this.hata = 'Seferler API üzerinden alınamadı.';
        this.yukleniyor = false;
      }
    });
  }

  seferSec(sefer: Sefer): void {
    this.seciliSefer = sefer;

    if (!sefer.seferKodu || !this.map) {
      return;
    }

    this.yukleniyor = true;
    this.hata = '';

    this.api
      .getSeferGuzergahi(sefer.seferKodu)
      .subscribe({
        next: (sonuc) => {
          this.guzergahiHaritadaGoster(sonuc);
          this.yukleniyor = false;

          setTimeout(() => {
            this.map?.invalidateSize();
          }, 100);
        },

        error: (error) => {
          console.error('Güzergâh alınamadı:', error);
          this.hata = 'Sefer güzergâhı alınamadı.';
          this.yukleniyor = false;
        }
      });
  }

  private guzergahiHaritadaGoster(
    guzergah: SeferGuzergahi
  ): void {
    if (!this.map) {
      return;
    }

    if (
      !guzergah.noktalar ||
      guzergah.noktalar.length === 0
    ) {
      this.hata =
        'Bu sefer için güzergâh noktası bulunamadı.';
      return;
    }

    if (this.guzergahCizgisi) {
      this.map.removeLayer(this.guzergahCizgisi);
    }

    const koordinatlar: L.LatLngExpression[] =
      guzergah.noktalar.map((nokta) => [
        nokta.enlem,
        nokta.boylam
      ]);

    this.guzergahCizgisi = L.polyline(
      koordinatlar,
      {
        weight: 5,
        opacity: 0.9
      }
    ).addTo(this.map);

    this.map.fitBounds(
      this.guzergahCizgisi.getBounds(),
      {
        padding: [25, 25]
      }
    );
  }

  yenidenYukle(): void {
    if (this.guzergahCizgisi && this.map) {
      this.map.removeLayer(this.guzergahCizgisi);
      this.guzergahCizgisi = undefined;
    }

    this.seciliSefer = null;
    this.seferleriGetir();
  }
}
