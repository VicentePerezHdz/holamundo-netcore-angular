import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Operacion } from '../../models/contratos';
import { HolaMundoApiService } from '../../services/hola-mundo-api.service';

/** Calculadora: la regla (incluida la división entre cero) vive en .NET, no en Angular. */
@Component({
  selector: 'app-calculadora',
  standalone: true,
  imports: [FormsModule],
  template: `
    <section class="tarjeta">
      <h2>2. Calculadora</h2>
      <div class="fila">
        <input type="number" [(ngModel)]="a" aria-label="Número A" />
        <select [(ngModel)]="operacion" aria-label="Operación">
          @for (op of operaciones; track op.valor) { <option [value]="op.valor">{{ op.simbolo }}</option> }
        </select>
        <input type="number" [(ngModel)]="b" aria-label="Número B" />
        <button (click)="calcular()">=</button>
      </div>
      @if (resultado()) { <p class="ok">{{ resultado() }}</p> }
      @if (error()) { <p class="error" role="alert">{{ error() }}</p> }
    </section>
  `
})
export class CalculadoraComponent {
  private readonly api = inject(HolaMundoApiService);

  readonly operaciones: { valor: Operacion; simbolo: string }[] = [
    { valor: 'suma', simbolo: '+' },
    { valor: 'resta', simbolo: '−' },
    { valor: 'multiplicacion', simbolo: '×' },
    { valor: 'division', simbolo: '÷' }
  ];
  a = 10;
  b = 5;
  operacion: Operacion = 'suma';
  resultado = signal('');
  error = signal('');

  calcular(): void {
    this.error.set('');
    this.api.calcular({ a: this.a, b: this.b, operacion: this.operacion }).subscribe({
      next: r => this.resultado.set(r.expresion),
      error: (e: Error) => { this.error.set(e.message); this.resultado.set(''); }
    });
  }
}
