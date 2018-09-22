import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map, tap, filter, catchError, mergeMap } from "rxjs/operators";
import { Operation } from 'fast-json-patch';
import { BaseService } from './base.service';

@Injectable()
export class BaseCrudApiService<TCreate, TRead, TUpdate, TDelete> extends BaseService {

  constructor(protected apiPath: string, protected http: HttpClient) {
    super();
  }

  getAll(): Observable<TRead[]> {
    return this.http.get<any>(`${this.apiUrl}${this.apiPath}`).pipe(
      map((response) => response.value)
    );
  }

  get(id: any): Observable<TRead> {
    return this.http.get<TRead>(`${this.apiUrl}${this.apiPath}/${id}`);
  }

  create(entityToCreate: TCreate): Observable<TRead> {
    return this.http.post<TRead>(`${this.apiUrl}${this.apiPath}`, entityToCreate,
      { headers: { 'Content-Type': 'application/json' } });
  }

  update(id: any, entityToUpdate: TUpdate): Observable<any> {
    return this.http.put(`${this.apiUrl}${this.apiPath}/${id}`, entityToUpdate,
      { headers: { 'Content-Type': 'application/json' } });
  }

  partiallyUpdate(id: any, patchDocument: Operation[]): Observable<any> {
    return this.http.patch(`${this.apiUrl}${this.apiPath}/${id}`, patchDocument,
      { headers: { 'Content-Type': 'application/json-patch+json' } });
  }

  delete(id: any, entityToDelete: TDelete): Observable<any> {
    return this.http.request('DELETE', `${this.apiUrl}${this.apiPath}/${id}`,
      {
        body: entityToDelete,
        headers: { 'Content-Type': 'application/json' }
      }
    );
  }
}
