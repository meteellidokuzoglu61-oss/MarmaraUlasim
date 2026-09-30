import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { IonContent, IonHeader, IonTitle, IonToolbar } from '@ionic/angular';

@Component({
  selector: 'app-duraklar',
  templateUrl: './duraklar.page.html',
  styleUrls: ['./duraklar.page.scss'],
  imports: [IonContent, IonHeader, IonTitle, IonToolbar, CommonModule, FormsModule]
})
export class DuraklarPage implements OnInit {

  constructor() { }

  ngOnInit() {
  }

}
