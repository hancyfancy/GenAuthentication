import { Injectable } from "@angular/core";

@Injectable({ providedIn: 'root' })
export class Settings {
  public readonly domain: string = 'http://localhost:5138';
}
