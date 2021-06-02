export interface ICity {
  id?: number;
  name?: string;
  countryCode?: string;
  district?: string;
  population?: number;
}

export class City implements ICity {
  constructor(
    public id?: number,
    public name?: string,
    public countryCode?: string,
    public district?: string,
    public population?: number
  ) {}
}

export function getCityIdentifier(city: ICity): number | undefined {
  return city.id;
}
