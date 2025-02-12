import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { IPaginatedModel } from '../shared/models/paginated.model';

@Injectable({
  providedIn: 'root'
})
export class HttpService {
  constructor(private http: HttpClient) { }

  getPaginated(endpoint: string, filter: IPaginatedModel): Observable<any> {
    return this.http.get(`${environment.apiUrl}/${endpoint}?perPage=${filter.perPage}&currentPage=${filter.currentPage}`);
  }

  getById(endpoint: string, id: string): Observable<any> {
    return this.http.get(`${environment.apiUrl}/${endpoint}/${id}`);
  }
}
