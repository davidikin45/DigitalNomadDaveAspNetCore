import { Injectable, ErrorHandler } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map, tap, filter, catchError, mergeMap } from "rxjs/operators";

import { Author } from './author.model';
import { BaseService } from '../../shared/base.service';
import { AuthorForCreation } from './author-for-creation.model';
import { Operation } from 'fast-json-patch';

@Injectable()
export class AuthorService extends BaseService {

  constructor(private http: HttpClient) {
    super();
  }

  getAll(): Observable<Author[]> {
    return this.http.get<Author[]>(`${this.apiUrl}/authors`);
  }

  get(id: string): Observable<Author> {
    return this.http.get<Author>(`${this.apiUrl}/authors/${id}`);
  }

  add(entityToAdd: AuthorForCreation): Observable<Author> {
    return this.http.post<Author>(`${this.apiUrl}/authors`, entityToAdd,
      { headers: { 'Content-Type': 'application/json' } });
  }

  partiallyUpdate(id: string, patchDocument: Operation[]): Observable<any> {
    return this.http.patch(`${this.apiUrl}/authors/${id}`, patchDocument,
      { headers: { 'Content-Type': 'application/json-patch+json' } });
  }
}
