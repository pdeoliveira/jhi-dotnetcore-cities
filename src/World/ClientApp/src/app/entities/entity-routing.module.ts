import { NgModule } from "@angular/core";
import { RouterModule } from "@angular/router";

@NgModule({
  imports: [
    RouterModule.forChild([
      {
        path: "city",
        data: { pageTitle: "worldApp.city.home.title" },
        loadChildren: () =>
          import("./city/city.module").then((m) => m.CityModule),
      },
      {
        path: "city",
        data: { pageTitle: "worldApp.city.home.title" },
        loadChildren: () =>
          import("./city/city.module").then((m) => m.CityModule),
      },
      {
        path: "city",
        data: { pageTitle: "worldApp.city.home.title" },
        loadChildren: () =>
          import("./city/city.module").then((m) => m.CityModule),
      },
      {
        path: "city",
        data: { pageTitle: "worldApp.city.home.title" },
        loadChildren: () =>
          import("./city/city.module").then((m) => m.CityModule),
      },
      {
        path: "city",
        data: { pageTitle: "worldApp.city.home.title" },
        loadChildren: () =>
          import("./city/city.module").then((m) => m.CityModule),
      },
      /* jhipster-needle-add-entity-route - JHipster will add entity modules routes here */
    ]),
  ],
})
export class EntityRoutingModule {}
