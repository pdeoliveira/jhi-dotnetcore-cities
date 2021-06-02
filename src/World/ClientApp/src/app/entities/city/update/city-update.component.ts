import { Component, OnInit } from "@angular/core";
import { HttpResponse } from "@angular/common/http";
import { FormBuilder, Validators } from "@angular/forms";
import { ActivatedRoute } from "@angular/router";
import { Observable } from "rxjs";
import { finalize } from "rxjs/operators";

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
  });

  constructor(
    protected cityService: CityService,
    protected activatedRoute: ActivatedRoute,
    protected fb: FormBuilder
  ) {}

  ngOnInit(): void {
    this.activatedRoute.data.subscribe(({ city }) => {
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
    };
  }
}
