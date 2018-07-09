import { Component, OnInit, OnDestroy } from '@angular/core';
import { MasterDataService } from '../../shared/master-data.service';
import { AuthorService } from '../shared/author.service';
import { Author } from '../shared/author.model';
import { Band } from '../../shared/band.model';
import { Subscription } from 'rxjs';
import { ActivatedRoute, Router } from '@angular/router';
import { DatePipe } from '@angular/common'
import { FormBuilder, FormGroup, FormControl, Validators } from '@angular/forms';
import { AuthorForUpdate } from '../shared/author-for-update.model';
import { compare } from 'fast-json-patch';

@Component({
  selector: 'app-author-update',
  templateUrl: './author-update.component.html',
  styleUrls: ['./author-update.component.css']
})
export class AuthorUpdateComponent implements OnInit, OnDestroy {

  public authorForm: FormGroup;
  private author: Author;
  private id: string;
  private sub: Subscription;
  private originalAuthorForUpdate: AuthorForUpdate;

  constructor(private masterDataService: MasterDataService,
    private authorService: AuthorService,
    private route: ActivatedRoute,
    private router: Router,
    private formBuilder: FormBuilder) { }

  ngOnInit() {
    // define the authorForm (with empty default values)
    this.authorForm = this.formBuilder.group({
      title: ['', [Validators.required, Validators.maxLength(200)]],
      description: ['', [Validators.required, Validators.maxLength(2000)]]
    }, {  });

    // get route data (id)
    this.sub = this.route.params.subscribe(
      params => {
        this.id = params['id'];

        // load author
        this.authorService.get(this.id)
          .subscribe(author => {
            this.author = author;
            this.updateAuthorForm();

            this.originalAuthorForUpdate = automapper.map(
              'AuthorFormModel',
              'AuthorForUpdate',
              this.authorForm.value);

          });
      }
    );
  }

  ngOnDestroy(): void {
    this.sub.unsubscribe();
  }

  private updateAuthorForm(): void {
    let datePipe = new DatePipe(navigator.language);
    let dateFormat = 'yyyy-MM-dd';

    this.authorForm.patchValue({
      title: this.author.title,
      description: this.author.description,
    });
  }

  saveAuthor(): void {
    if (this.authorForm.dirty && this.authorForm.valid) {
      // TODO
      // [
      //   { op: "replace", path: "/description", value: "Updated description"}
      //   {op: "replace", path: "/title", value: "Updated title"}
      // ]

      let changedAuthorForUpdate = automapper.map(
        'AuthorFormModel',
        'AuthorForUpdate',
        this.authorForm.value);

      let patchDocument = compare(this.originalAuthorForUpdate, changedAuthorForUpdate);

      this.authorService.partiallyUpdate(this.id, patchDocument)
        .subscribe(
          () => {
            this.router.navigateByUrl('/authors');
          });
    }
  }
}
