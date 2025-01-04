import { HttpClient, HttpParams } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class HttpService {
  private baseUrl!: string;
  constructor(private http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.baseUrl = baseUrl;
  }

  public get<T>(url: string, params: any = {}): Observable<T> {
    return this.http.get<T>(this.baseUrl + url, { params: params });
  }

  public post<T>(url: string, data: any): Observable<T> {
    return this.http.post<T>(this.baseUrl + url, data);
  }
}
