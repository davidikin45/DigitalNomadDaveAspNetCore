import { Component, OnInit } from '@angular/core';
import { Author } from '../shared/author.model';
import { AuthorService } from '../shared/author.service';
import { ActivatedRoute } from '@angular/router';
import { OnDestroy } from '@angular/core/src/metadata/lifecycle_hooks';
import { MasterDataService } from '../../shared/master-data.service';
import { Subscription } from 'rxjs';
import { OpenIdConnectService } from '../../shared/open-id-connect.service';

@Component({
  selector: 'app-author-detail',
  templateUrl: './author-detail.component.html',
  styleUrls: ['./author-detail.component.css']
})
export class AuthorDetailComponent implements OnInit, OnDestroy {

  private author: any;
  private id: string;
  private sub: Subscription;
  private isAdmin: boolean = (this.openIdConnectService.user.profile.role === "Admin");

  constructor(private masterDataService: MasterDataService,
    private authorService: AuthorService,
    private route: ActivatedRoute,
    private openIdConnectService: OpenIdConnectService) {
  }

  ngOnInit() {
    // get route data (id)
    this.sub = this.route.params.subscribe(
      params => {
        this.id = params['id'];

        if (this.isAdmin === true) {
          // get author
          this.authorService.get(this.id)
            .subscribe(author => {
              this.author = author;
            });
        }
        else {
          // get tour 
          this.authorService.get(this.id)
            .subscribe(author => {
              this.author = author;
            });
        }    
      }
    );
  }

  ngOnDestroy(): void {
    this.sub.unsubscribe();
  }
}
