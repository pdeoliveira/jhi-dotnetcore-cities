import { Injectable } from "@angular/core";
import { HttpClient, HttpResponse } from "@angular/common/http";
import { Observable } from "rxjs";
import { map } from "rxjs/operators";
import * as dayjs from "dayjs";

import { isPresent } from "app/core/util/operators";
import { ApplicationConfigService } from "app/core/config/application-config.service";
import { createRequestOption } from "app/core/request/request-util";
import { ICity, getCityIdentifier } from "../city.model";

export type EntityResponseType = HttpResponse<ICity>;
export type EntityArrayResponseType = HttpResponse<ICity[]>;

@Injectable({ providedIn: "root" })
export class CityService {
  public resourceUrl = this.applicationConfigService.getEndpointFor(
    "api/cities"
  );

  constructor(
    protected http: HttpClient,
    private applicationConfigService: ApplicationConfigService
  ) {}

  create(city: ICity): Observable<EntityResponseType> {
    const copy = this.convertDateFromClient(city);
    return this.http
      .post<ICity>(this.resourceUrl, copy, { observe: "response" })
      .pipe(map((res: EntityResponseType) => this.convertDateFromServer(res)));
  }

  update(city: ICity): Observable<EntityResponseType> {
    const copy = this.convertDateFromClient(city);
    return this.http
      .put<ICity>(
        `${this.resourceUrl}/${getCityIdentifier(city) as number}`,
        copy,
        { observe: "response" }
      )
      .pipe(map((res: EntityResponseType) => this.convertDateFromServer(res)));
  }

  partialUpdate(city: ICity): Observable<EntityResponseType> {
    const copy = this.convertDateFromClient(city);
    return this.http
      .patch<ICity>(
        `${this.resourceUrl}/${getCityIdentifier(city) as number}`,
        copy,
        { observe: "response" }
      )
      .pipe(map((res: EntityResponseType) => this.convertDateFromServer(res)));
  }

  find(id: number): Observable<EntityResponseType> {
    return this.http
      .get<ICity>(`${this.resourceUrl}/${id}`, { observe: "response" })
      .pipe(map((res: EntityResponseType) => this.convertDateFromServer(res)));
  }

  query(req?: any): Observable<EntityArrayResponseType> {
    const options = createRequestOption(req);
    return this.http
      .get<ICity[]>(this.resourceUrl, { params: options, observe: "response" })
      .pipe(
        map((res: EntityArrayResponseType) =>
          this.convertDateArrayFromServer(res)
        )
      );
  }

  delete(id: number): Observable<HttpResponse<{}>> {
    return this.http.delete(`${this.resourceUrl}/${id}`, {
      observe: "response",
    });
  }

  addCityToCollectionIfMissing(
    cityCollection: ICity[],
    ...citiesToCheck: (ICity | null | undefined)[]
  ): ICity[] {
    const cities: ICity[] = citiesToCheck.filter(isPresent);
    if (cities.length > 0) {
      const cityCollectionIdentifiers = cityCollection.map(
        (cityItem) => getCityIdentifier(cityItem)!
      );
      const citiesToAdd = cities.filter((cityItem) => {
        const cityIdentifier = getCityIdentifier(cityItem);
        if (
          cityIdentifier == null ||
          cityCollectionIdentifiers.includes(cityIdentifier)
        ) {
          return false;
        }
        cityCollectionIdentifiers.push(cityIdentifier);
        return true;
      });
      return [...citiesToAdd, ...cityCollection];
    }
    return cityCollection;
  }

  protected convertDateFromClient(city: ICity): ICity {
    return Object.assign({}, city, {
      lastVisited: city.lastVisited?.isValid()
        ? city.lastVisited.toJSON()
        : undefined,
    });
  }

  protected convertDateFromServer(res: EntityResponseType): EntityResponseType {
    if (res.body) {
      res.body.lastVisited = res.body.lastVisited
        ? dayjs(res.body.lastVisited)
        : undefined;
    }
    return res;
  }

  protected convertDateArrayFromServer(
    res: EntityArrayResponseType
  ): EntityArrayResponseType {
    if (res.body) {
      res.body.forEach((city: ICity) => {
        city.lastVisited = city.lastVisited
          ? dayjs(city.lastVisited)
          : undefined;
      });
    }
    return res;
  }
}
