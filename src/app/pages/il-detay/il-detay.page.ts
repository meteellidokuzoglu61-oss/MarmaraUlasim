import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { IonContent, IonHeader, IonTitle, IonToolbar } from '@ionic/angular';

@Component({
  selector: 'app-il-detay',
  templateUrl: './il-detay.page.html',
  styleUrls: ['./il-detay.page.scss'],
  imports: [IonContent, IonHeader, IonTitle, IonToolbar, CommonModule, FormsModule]
})
export class IlDetayPage implements OnInit {

  constructor() { }

  ngOnInit() {
  }

}
