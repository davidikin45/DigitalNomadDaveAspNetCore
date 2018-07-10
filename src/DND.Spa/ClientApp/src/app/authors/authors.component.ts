import { Component, OnInit, ErrorHandler } from '@angular/core';

import { AuthorForRead } from './shared/author-for-read.model';
import { AuthorService } from './shared/author.service';

@Component({
  selector: 'app-authors',
  templateUrl: './authors.component.html',
  styleUrls: ['./authors.component.css']
})
export class AuthorsComponent implements OnInit {
  title: string = 'Author overview'
  authors: AuthorForRead[] = [];

  constructor(private authorService: AuthorService) {    
        }

  ngOnInit() {
    this.authorService.getAll()
      .subscribe(authors => {
      this.authors = authors;
    });    
  }

}
