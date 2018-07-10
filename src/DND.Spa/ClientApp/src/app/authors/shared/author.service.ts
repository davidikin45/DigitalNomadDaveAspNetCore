import { Injectable, ErrorHandler } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map, tap, filter, catchError, mergeMap } from "rxjs/operators";
import { BaseCrudApiService } from '../../shared/base-crud-api.service';
import { Operation } from 'fast-json-patch';

import { AuthorForCreation } from './author-for-creation.model';
import { AuthorForRead } from './author-for-read.model';
import { AuthorForUpdate } from './author-for-update.model';
import { AuthorForDeletion } from './author-for-deletion.model';

@Injectable()
export class AuthorService extends BaseCrudApiService<AuthorForCreation, AuthorForRead, AuthorForUpdate, AuthorForDeletion> {

  constructor(protected http: HttpClient) {
    super('authors',http);
  }

}
