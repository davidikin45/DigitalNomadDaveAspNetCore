import { Injectable } from '@angular/core';
import { BaseService } from '../../../shared/base.service';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { ShowForRead } from './show-for-read.model';
import { Observable } from 'rxjs';
import { ShowForCreation } from './show-for-creation.model';

@Injectable()
export class ShowService extends BaseService {

  constructor(private http: HttpClient) {
    super();
  }

  getShows(authorId: number): Observable<ShowForRead[]> {
    return this.http.get<ShowForRead[]>(`${this.apiUrl}/authors/${authorId}/shows`);
  }

  addShowCollection(authorId: number, showsToAdd: ShowForCreation[]): Observable<ShowForRead[]> {
    return this.http.post<ShowForRead[]>(`${this.apiUrl}/authors/${authorId}/showcollections`, showsToAdd,
      { headers: { 'Content-Type': 'application/vnd.marvin.showcollectionforcreation+json' } });
  }
}
