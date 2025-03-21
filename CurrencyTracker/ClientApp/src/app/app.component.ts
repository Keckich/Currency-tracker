import { Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute, NavigationEnd, Router } from '@angular/router';
import { RouteService } from './core/services/route.service';
import { filter } from 'rxjs';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html'
})
export class AppComponent implements OnInit {
  title = 'app';

  private router = inject(Router);
  private activatedRoute = inject(ActivatedRoute);
  private routeService = inject(RouteService);

  ngOnInit(): void {
    this.router.events
      .pipe(filter(event => event instanceof NavigationEnd))
      .subscribe(() => {
        this.routeService.onRouteChanged(this.activatedRoute.snapshot);
      })
  }
}
