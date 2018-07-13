import { Component, Input } from "@angular/core";

@Component({
  selector: 'collapsible-card',
  template: `
  <div (click)="toggleContent()" class="card">
    <h4 class="card-header">
      <ng-content select="[card-title]"></ng-content>
    </h4>
    <ng-content *ngIf="visible" select="[card-body]"></ng-content>
  </div>
  `
})
export class CollapsibleCardComponent {
  visible: boolean = true;

  toggleContent() {
    this.visible = !this.visible;
  }
}
