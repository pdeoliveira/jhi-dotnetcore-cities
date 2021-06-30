import * as dayjs from "dayjs";
import { Continents } from "app/entities/enumerations/continents.model";

export interface ICity {
  id?: number;
  name?: string;
  countryCode?: string;
  district?: string;
  population?: number;
  isFavorite?: boolean | null;
  lastVisited?: dayjs.Dayjs | null;
  continent?: Continents | null;
}

export class City implements ICity {
  constructor(
    public id?: number,
    public name?: string,
    public countryCode?: string,
    public district?: string,
    public population?: number,
    public isFavorite?: boolean | null,
    public lastVisited?: dayjs.Dayjs | null,
    public continent?: Continents | null
  ) {
    this.isFavorite = this.isFavorite ?? false;
  }
}

export function getCityIdentifier(city: ICity): number | undefined {
  return city.id;
}
