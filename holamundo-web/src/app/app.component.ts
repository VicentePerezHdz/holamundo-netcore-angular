import { Component } from '@angular/core';
import { CalculadoraComponent } from './componentes/calculadora/calculadora.component';
import { ComboLenguajesComponent } from './componentes/combo-lenguajes/combo-lenguajes.component';
import { SaludoComponent } from './componentes/saludo/saludo.component';

/** Componente raíz: solo compone las tres tarjetas. */
@Component({
  selector: 'app-root',
  standalone: true,
  imports: [SaludoComponent, CalculadoraComponent, ComboLenguajesComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  title = 'Hola Mundo: Angular 17 + .NET 8';
}
