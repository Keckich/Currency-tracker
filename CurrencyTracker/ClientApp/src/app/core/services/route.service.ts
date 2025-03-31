import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, ParamMap } from '@angular/router';
import { BehaviorSubject } from 'rxjs';
import { RouteParams } from '../../shared/shared.model';
import { merge } from 'lodash';

@Injectable({
  providedIn: 'root'
})
export class RouteService {
  private paramsSubject = new BehaviorSubject<RouteParams | null>(null);
  public params$ = this.paramsSubject.asObservable();

  buildUrl(url: string, ...resources: (string | number)[]): string {
    return [url]
    .concat(resources.map(r => r.toString()))
    .join('/');
  }

  onRouteChanged(snapshot: ActivatedRouteSnapshot): void {
    const snapshots = this.getAllRouteSnapshots(snapshot);
    const paramsArray = snapshots.map(s => this.getParams(s.paramMap));
    const params = merge({}, ...paramsArray);

    this.paramsSubject.next(params);
  }

  private getAllRouteSnapshots(snapshot: ActivatedRouteSnapshot): ActivatedRouteSnapshot[] {
    const snapshots = [snapshot];

    let currentSnapshot = snapshot;
    while (currentSnapshot.firstChild) {
      currentSnapshot = currentSnapshot.firstChild;
      snapshots.push(currentSnapshot);
    }

    return snapshots;
  }

  private getParams(paramMap: ParamMap): RouteParams {
    const keys = Object.keys(new RouteParams());

    return keys.reduce((params, key) => {
      params[key as keyof RouteParams] = paramMap.get(key) || undefined;
      return params;
    }, {} as RouteParams);
  }
}
