import { Component, OnInit, ErrorHandler } from '@angular/core';

import { Author } from './shared/author.model';
import { AuthorService } from './shared/author.service';

@Component({
  selector: 'app-authors',
  templateUrl: './authors.component.html',
  styleUrls: ['./authors.component.css']
})
export class AuthorsComponent implements OnInit {
  title: string = 'Author overview'
  authors: Author[] = [];

  constructor(private authorService: AuthorService) {    
        }

  ngOnInit() {
    this.authorService.getAuthors()
      .subscribe(authors => {
      this.authors = authors;
    });    
  }

}
