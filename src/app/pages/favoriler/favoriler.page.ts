import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { IonContent, IonHeader, IonTitle, IonToolbar } from '@ionic/angular';

@Component({
  selector: 'app-favoriler',
  templateUrl: './favoriler.page.html',
  styleUrls: ['./favoriler.page.scss'],
  imports: [IonContent, IonHeader, IonTitle, IonToolbar, CommonModule, FormsModule]
})
export class FavorilerPage implements OnInit {

  constructor() { }

  ngOnInit() {
  }

}
