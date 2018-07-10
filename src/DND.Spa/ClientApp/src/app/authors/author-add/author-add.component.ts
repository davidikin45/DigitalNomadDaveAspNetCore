import { Component, OnInit, OnDestroy } from '@angular/core';
import { Band } from '../../shared/band.model';
import { FormGroup, FormBuilder, FormArray, Validators } from '@angular/forms';
import { Subscription } from 'rxjs';
import { MasterDataService } from '../../shared/master-data.service';
import { AuthorService } from '../shared/author.service';
import { Router } from '@angular/router';
import { Manager } from '../../shared/manager.model';
import { ValidationErrorHandler } from '../../shared/validation-error-handler';
import { OpenIdConnectService } from '../../shared/open-id-connect.service';

@Component({
  selector: 'app-author-add',
  templateUrl: './author-add.component.html',
  styleUrls: ['./author-add.component.css']
})
export class AuthorAddComponent implements OnInit {

  public authorForm: FormGroup;
  private isAdmin: boolean = 
    (this.openIdConnectService.user.profile.role === "Admin");

  constructor(private masterDataService: MasterDataService,
    private authorService: AuthorService,
    private formBuilder: FormBuilder,
    private router: Router,
    private openIdConnectService: OpenIdConnectService) { }

  ngOnInit() {

    // define the authorForm (with empty default values)
    this.authorForm = this.formBuilder.group({
      name: ['', [Validators.required, Validators.maxLength(200)]],
      urlSlug: ['', Validators.maxLength(2000)],
    }, {  });

    // get bands from master data service
    //this.masterDataService.getBands()
    //  .subscribe(bands => {
    //    this.bands = bands;
    //  });

    //if (this.isAdmin === true) {
    //  // get managers from master data service
    //  this.masterDataService.getManagers()
    //    .subscribe(managers => {
    //      this.managers = managers;
    //    });
    //}
  }
 
  addAuthor(): void {
    if (this.authorForm.dirty && this.authorForm.valid) {
        let author = automapper.map(
          'AuthorFormModel',
          'AuthorForCreation',
          this.authorForm.value);
        this.authorService.create(author)
          .subscribe(
            () => {
              this.router.navigateByUrl('/authors');
            },
            (validationResult) => { ValidationErrorHandler.handleValidationErrors(this.authorForm, validationResult); });
    }
  }
}
