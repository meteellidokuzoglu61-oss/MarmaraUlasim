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

  private map!: L.Map;

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
    this.haritaOlustur();
    this.seferleriGetir();
  }

  ngOnDestroy(): void {
    if (this.map) {
      this.map.remove();
    }
  }

  private haritaOlustur(): void {
    this.map = L.map('harita', {
      center: [40.765, 29.940],
      zoom: 11
    });

    L.tileLayer(
      'https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png',
      {
        maxZoom: 19,
        attribution: '&copy; OpenStreetMap'
      }
    ).addTo(this.map);
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
        console.error(
          'Seferler alınamadı:',
          error
        );

        this.hata =
          'Seferler API üzerinden alınamadı.';

        this.yukleniyor = false;
      }
    });
  }

  seferSec(sefer: Sefer): void {
    this.seciliSefer = sefer;

    if (!sefer.seferKodu) {
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
        },

        error: (error) => {
          console.error(
            'Güzergâh alınamadı:',
            error
          );

          this.hata =
            'Sefer güzergâhı alınamadı.';

          this.yukleniyor = false;
        }
      });
  }

  private guzergahiHaritadaGoster(
    guzergah: SeferGuzergahi
  ): void {

    if (
      !guzergah.noktalar ||
      guzergah.noktalar.length === 0
    ) {
      this.hata =
        'Bu sefer için güzergâh noktası bulunamadı.';

      return;
    }

    if (this.guzergahCizgisi) {
      this.map.removeLayer(
        this.guzergahCizgisi
      );
    }

    const koordinatlar: L.LatLngExpression[] =
      guzergah.noktalar.map(
        nokta => [
          nokta.enlem,
          nokta.boylam
        ]
      );

    this.guzergahCizgisi =
      L.polyline(
        koordinatlar,
        {
          weight: 5
        }
      ).addTo(this.map);

    this.map.fitBounds(
      this.guzergahCizgisi.getBounds(),
      {
        padding: [30, 30]
      }
    );
  }

  yenidenYukle(): void {
    this.seferleriGetir();
  }
}
