import { Component } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { IonHeader, IonToolbar, IonTitle, IonContent } from '@ionic/angular';

@Component({
  selector: 'app-home',
  templateUrl: 'home.page.html',
  styleUrls: ['home.page.scss'],
  standalone: true,
  imports: [IonHeader, IonToolbar, IonTitle, IonContent, RouterLink, FormsModule],
})
export class HomePage {
  varisDurağı = '';

  constructor(private readonly router: Router) {}

  durakAra(): void {
    const arama = this.varisDurağı.trim();
    if (!arama) {
      this.router.navigate(['/duraklar']);
      return;
    }

    this.router.navigate(['/duraklar'], { queryParams: { arama } });
  }
}
