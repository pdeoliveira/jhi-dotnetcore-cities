import { Component, OnInit } from "@angular/core";
import { HttpResponse } from "@angular/common/http";
import { FormBuilder, Validators } from "@angular/forms";
import { ActivatedRoute } from "@angular/router";
import { Observable } from "rxjs";
import { finalize } from "rxjs/operators";

import * as dayjs from "dayjs";
import { DATE_TIME_FORMAT } from "app/config/input.constants";

import { ICity, City } from "../city.model";
import { CityService } from "../service/city.service";

@Component({
  selector: "jhi-city-update",
  templateUrl: "./city-update.component.html",
})
export class CityUpdateComponent implements OnInit {
  isSaving = false;

  editForm = this.fb.group({
    id: [],
    name: [null, [Validators.required]],
    countryCode: [null, [Validators.required]],
    district: [null, [Validators.required]],
    population: [null, [Validators.required]],
    isFavorite: [],
    lastVisited: [],
    continent: [],
  });

  constructor(
    protected cityService: CityService,
    protected activatedRoute: ActivatedRoute,
    protected fb: FormBuilder
  ) {}

  ngOnInit(): void {
    this.activatedRoute.data.subscribe(({ city }) => {
      if (city.id === undefined) {
        const today = dayjs().startOf("day");
        city.lastVisited = today;
      }

      this.updateForm(city);
    });
  }

  previousState(): void {
    window.history.back();
  }

  save(): void {
    this.isSaving = true;
    const city = this.createFromForm();
    if (city.id !== undefined) {
      this.subscribeToSaveResponse(this.cityService.update(city));
    } else {
      this.subscribeToSaveResponse(this.cityService.create(city));
    }
  }

  protected subscribeToSaveResponse(
    result: Observable<HttpResponse<ICity>>
  ): void {
    result.pipe(finalize(() => this.onSaveFinalize())).subscribe(
      () => this.onSaveSuccess(),
      () => this.onSaveError()
    );
  }

  protected onSaveSuccess(): void {
    this.previousState();
  }

  protected onSaveError(): void {
    // Api for inheritance.
  }

  protected onSaveFinalize(): void {
    this.isSaving = false;
  }

  protected updateForm(city: ICity): void {
    this.editForm.patchValue({
      id: city.id,
      name: city.name,
      countryCode: city.countryCode,
      district: city.district,
      population: city.population,
      isFavorite: city.isFavorite,
      lastVisited: city.lastVisited
        ? city.lastVisited.format(DATE_TIME_FORMAT)
        : null,
      continent: city.continent,
    });
  }

  protected createFromForm(): ICity {
    return {
      ...new City(),
      id: this.editForm.get(["id"])!.value,
      name: this.editForm.get(["name"])!.value,
      countryCode: this.editForm.get(["countryCode"])!.value,
      district: this.editForm.get(["district"])!.value,
      population: this.editForm.get(["population"])!.value,
      isFavorite: this.editForm.get(["isFavorite"])!.value,
      lastVisited: this.editForm.get(["lastVisited"])!.value
        ? dayjs(this.editForm.get(["lastVisited"])!.value, DATE_TIME_FORMAT)
        : undefined,
      continent: this.editForm.get(["continent"])!.value,
    };
  }
}
