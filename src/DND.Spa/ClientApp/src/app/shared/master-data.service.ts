import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { map, tap, filter, catchError, mergeMap } from "rxjs/operators";
import { Band } from './band.model';
import { Manager } from './manager.model';
import { BaseService } from './base.service';


// Master data that's used across the application
@Injectable()
export class MasterDataService extends BaseService {

  private bands: Band[];
  private managers: Manager[];

  constructor(private http: HttpClient) {
    super();
  }

  getBands(): Observable<Band[]> {
    if (this.bands) {
      return of(this.bands);
    }
    else {
      return this.http.get<Band[]>(`${this.apiUrl}/bands`)
        .pipe(
          tap((bandsFromResponse) => {
        this.bands = bandsFromResponse;
      })
        );
    }
  }

  getManagers(): Observable<Manager[]> {
    if (this.managers) {
      return of(this.managers);
    }
    else {
      return this.http.get<Manager[]>(`${this.apiUrl}/managers`).pipe(
        tap((managersFromResponse) => {
          this.managers = managersFromResponse;
        })
      )       
    }
  }
}
