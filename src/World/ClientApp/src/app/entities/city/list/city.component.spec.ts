import { ComponentFixture, TestBed } from "@angular/core/testing";
import { HttpHeaders, HttpResponse } from "@angular/common/http";
import { HttpClientTestingModule } from "@angular/common/http/testing";
import { of } from "rxjs";

import { CityService } from "../service/city.service";

import { CityComponent } from "./city.component";

describe("Component Tests", () => {
  describe("City Management Component", () => {
    let comp: CityComponent;
    let fixture: ComponentFixture<CityComponent>;
    let service: CityService;

    beforeEach(() => {
      TestBed.configureTestingModule({
        imports: [HttpClientTestingModule],
        declarations: [CityComponent],
      })
        .overrideTemplate(CityComponent, "")
        .compileComponents();

      fixture = TestBed.createComponent(CityComponent);
      comp = fixture.componentInstance;
      service = TestBed.inject(CityService);

      const headers = new HttpHeaders().append("link", "link;link");
      spyOn(service, "query").and.returnValue(
        of(
          new HttpResponse({
            body: [{ id: 123 }],
            headers,
          })
        )
      );
    });

    it("Should call load all on init", () => {
      // WHEN
      comp.ngOnInit();

      // THEN
      expect(service.query).toHaveBeenCalled();
      expect(comp.cities?.[0]).toEqual(jasmine.objectContaining({ id: 123 }));
    });
  });
});
