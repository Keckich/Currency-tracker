import { Injectable, inject } from '@angular/core';
import { ActivatedRoute, ParamMap } from '@angular/router';
import { Observable } from 'rxjs';
import { RouteParams } from '../../shared/shared.model';

@Injectable({
  providedIn: 'root'
})
export class RouteService {
  private activatedRoute = inject(ActivatedRoute);

  getParams(paramMap: ParamMap): RouteParams {
    const keys = Object.keys(new RouteParams());

    return keys.reduce((params, key) => {
      params[key as keyof RouteParams] = paramMap.get(key) || undefined;
      return params;
    }, {} as RouteParams);
  }

  buildUrl(url: string, ...resources: (string | number)[]): string {
    return [url]
    .concat(resources.map(r => r.toString()))
    .join('/');
  }
}
