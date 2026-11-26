import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { OpcionCatalogo } from '../../models/contratos';
import { HolaMundoApiService } from '../../services/hola-mundo-api.service';

/**
 * Combobox: las opciones vienen de GET /api/catalogos/lenguajes al iniciar (ngOnInit).
 * computed(): valor derivado; se recalcula solo cuando cambia la selección o la lista.
 */
@Component({
  selector: 'app-combo-lenguajes',
  standalone: true,
  imports: [FormsModule],
  template: `
    <section class="tarjeta">
      <h2>3. Combobox con mensaje</h2>
      <label for="lenguaje">Elige un lenguaje</label>
      <select id="lenguaje" [ngModel]="seleccion()" (ngModelChange)="seleccion.set($event)">
        <option value="">-- Selecciona --</option>
        @for (o of opciones(); track o.clave) { <option [value]="o.clave">{{ o.texto }}</option> }
      </select>
      @if (mensaje()) { <p class="ok">{{ mensaje() }}</p> }
      @if (error()) { <p class="error" role="alert">{{ error() }}</p> }
    </section>
  `
})
export class ComboLenguajesComponent implements OnInit {
  private readonly api = inject(HolaMundoApiService);

  opciones = signal<OpcionCatalogo[]>([]);
  seleccion = signal('');
  error = signal('');
  mensaje = computed(() => this.opciones().find(o => o.clave === this.seleccion())?.mensaje ?? '');

  ngOnInit(): void {
    this.api.lenguajes().subscribe({
      next: lista => this.opciones.set(lista),
      error: (e: Error) => this.error.set(e.message)
    });
  }
}
