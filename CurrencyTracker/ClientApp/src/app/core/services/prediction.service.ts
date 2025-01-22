import { Injectable, inject } from '@angular/core';
import { HttpService } from './http.service';
import { RouteService } from './route.service';
import { ApiUrlResources, ApiUrls } from '../../shared/constants.value';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class PredictionService {
  private httpService = inject(HttpService);
  private routeService = inject(RouteService);

  //TODO: change 'any' type
  getHammerPrediction(currency: string, interval: string): Observable<any> {
    return this.httpService.get(this.routeService.buildUrl(ApiUrls.PATTERN_ANALYZER), { currency: currency, interval: interval });
  }
}
