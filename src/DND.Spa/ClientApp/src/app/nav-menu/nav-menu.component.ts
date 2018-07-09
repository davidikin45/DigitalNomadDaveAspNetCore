import { Component } from '@angular/core';
import { OpenIdConnectService } from '../shared/open-id-connect.service';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css']
})
export class NavMenuComponent {
  isExpanded = false;

  collapse() {
    this.isExpanded = false;
  }

  toggle() {
    this.isExpanded = !this.isExpanded;
  }

  constructor(private openIdConnectService: OpenIdConnectService) {
  }
}


