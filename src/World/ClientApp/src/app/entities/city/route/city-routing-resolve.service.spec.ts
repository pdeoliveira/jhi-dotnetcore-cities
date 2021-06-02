jest.mock("@angular/router");

import { TestBed } from "@angular/core/testing";
import { HttpResponse } from "@angular/common/http";
import { HttpClientTestingModule } from "@angular/common/http/testing";
import { ActivatedRouteSnapshot, Router } from "@angular/router";
import { of } from "rxjs";

import { ICity, City } from "../city.model";
import { CityService } from "../service/city.service";

import { CityRoutingResolveService } from "./city-routing-resolve.service";

describe("Service Tests", () => {
  describe("City routing resolve service", () => {
    let mockRouter: Router;
    let mockActivatedRouteSnapshot: ActivatedRouteSnapshot;
    let routingResolveService: CityRoutingResolveService;
    let service: CityService;
    let resultCity: ICity | undefined;

    beforeEach(() => {
      TestBed.configureTestingModule({
        imports: [HttpClientTestingModule],
        providers: [Router, ActivatedRouteSnapshot],
      });
      mockRouter = TestBed.inject(Router);
      mockActivatedRouteSnapshot = TestBed.inject(ActivatedRouteSnapshot);
      routingResolveService = TestBed.inject(CityRoutingResolveService);
      service = TestBed.inject(CityService);
      resultCity = undefined;
    });

    describe("resolve", () => {
      it("should return ICity returned by find", () => {
        // GIVEN
        service.find = jest.fn((id) => of(new HttpResponse({ body: { id } })));
        mockActivatedRouteSnapshot.params = { id: 123 };

        // WHEN
        routingResolveService
          .resolve(mockActivatedRouteSnapshot)
          .subscribe((result) => {
            resultCity = result;
          });

        // THEN
        expect(service.find).toBeCalledWith(123);
        expect(resultCity).toEqual({ id: 123 });
      });

      it("should return new ICity if id is not provided", () => {
        // GIVEN
        service.find = jest.fn();
        mockActivatedRouteSnapshot.params = {};

        // WHEN
        routingResolveService
          .resolve(mockActivatedRouteSnapshot)
          .subscribe((result) => {
            resultCity = result;
          });

        // THEN
        expect(service.find).not.toBeCalled();
        expect(resultCity).toEqual(new City());
      });

      it("should route to 404 page if data not found in server", () => {
        // GIVEN
        spyOn(service, "find").and.returnValue(
          of(new HttpResponse({ body: null }))
        );
        mockActivatedRouteSnapshot.params = { id: 123 };

        // WHEN
        routingResolveService
          .resolve(mockActivatedRouteSnapshot)
          .subscribe((result) => {
            resultCity = result;
          });

        // THEN
        expect(service.find).toBeCalledWith(123);
        expect(resultCity).toEqual(undefined);
        expect(mockRouter.navigate).toHaveBeenCalledWith(["404"]);
      });
    });
  });
});
