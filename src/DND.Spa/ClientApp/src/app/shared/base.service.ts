import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map, tap, filter, catchError, mergeMap } from "rxjs/operators";
import { environment } from '../../environments/environment';
import { Operation } from 'fast-json-patch';

@Injectable()
export class BaseService {
  apiUrl = environment.apiUrl;

  constructor() { }

}
