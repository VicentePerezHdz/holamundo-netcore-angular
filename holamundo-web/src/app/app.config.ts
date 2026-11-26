import { ApplicationConfig } from '@angular/core';
import { provideHttpClient } from '@angular/common/http';

// Equivalente al Program.cs de .NET: aquí se registran los "servicios" globales de Angular.
// provideHttpClient() habilita HttpClient para inyectarlo en HolaMundoApiService.
export const appConfig: ApplicationConfig = {
  providers: [provideHttpClient()]
};
