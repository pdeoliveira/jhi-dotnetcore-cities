import { Component, OnInit } from "@angular/core";
import { HttpResponse } from "@angular/common/http";
import { NgbModal } from "@ng-bootstrap/ng-bootstrap";

import { ICity } from "../city.model";
import { CityService } from "../service/city.service";
import { CityDeleteDialogComponent } from "../delete/city-delete-dialog.component";

import { ITEMS_PER_PAGE } from "app/config/pagination.constants";
import { LazyLoadEvent } from "primeng/api";

@Component({
  selector: "jhi-city",
  templateUrl: "./city.component.html",
})
export class CityComponent implements OnInit {
  cities?: ICity[];
  isLoading = false;
  page = 0;
  itemsPerPage = ITEMS_PER_PAGE;
  totalRecords!: number;
  event: LazyLoadEvent = {"first":0,"rows":20,"sortOrder":1,"filters":{},"globalFilter":null};

  constructor(
    protected cityService: CityService,
    protected modalService: NgbModal
  ) {}

  loadAll(): void {
    this.isLoading = true;

    this.cityService.query({
        page: this.page,
        loadEvent: JSON.stringify(this.event)
      }).subscribe(
      (res: HttpResponse<ICity[]>) => {
        this.isLoading = false;
        this.totalRecords = Number(res.headers.get("X-Total-Count"));
        this.cities = res.body ?? [];
      },
      () => {
        this.isLoading = false;
      }
    );
  }

  ngOnInit(): void {
    this.loadAll();
  }

  trackId(index: number, item: ICity): number {
    return item.id!;
  }

  delete(city: ICity): void {
    const modalRef = this.modalService.open(CityDeleteDialogComponent, {
      size: "lg",
      backdrop: "static",
    });
    modalRef.componentInstance.city = city;
    // unsubscribe not needed because closed completes on modal close
    modalRef.closed.subscribe((reason) => {
      if (reason === "deleted") {
        this.loadAll();
      }
    });
  }

  loadCities(event: LazyLoadEvent): void {
    this.event = event;
    this.page = event.first! / this.itemsPerPage;
    this.loadAll();
  }

  refresh(): void {
    this.event = {"first":0,"rows":20,"sortOrder":1,"filters":{},"globalFilter":null};
    this.page = 0;
    this.loadAll();
  }
}
