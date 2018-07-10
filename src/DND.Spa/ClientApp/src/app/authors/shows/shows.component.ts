import { Component, OnInit, Input } from '@angular/core';
import { ShowForRead } from './shared/show-for-read.model';
import { Subscription } from 'rxjs';
import { ActivatedRoute } from '@angular/router';
import { ShowService } from './shared/show.service';
import { OnDestroy } from '@angular/core/src/metadata/lifecycle_hooks';

@Component({
  selector: 'app-shows',
  templateUrl: './shows.component.html',
  styleUrls: ['./shows.component.css']
})

export class ShowsComponent implements OnInit {

  @Input() shows: ShowForRead[];

  ngOnInit() {
  }
}
