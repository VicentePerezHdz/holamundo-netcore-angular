import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HolaMundoApiService } from '../../services/hola-mundo-api.service';

/**
 * Input de texto -> POST /api/saludo -> muestra el mensaje que construyó .NET.
 * signal(): estado reactivo de Angular 17; al cambiar, la vista se actualiza sola.
 */
@Component({
  selector: 'app-saludo',
  standalone: true,
  imports: [FormsModule],
  template: `
    <section class="tarjeta">
      <h2>1. Saludo (input texto)</h2>
      <label for="nombre">Tu nombre</label>
      <input id="nombre" [(ngModel)]="nombre" (keyup.enter)="saludar()" placeholder="Escribe tu nombre" />
      <button (click)="saludar()" [disabled]="cargando()">{{ cargando() ? 'Enviando…' : 'Saludar' }}</button>
      @if (mensaje()) { <p class="ok">{{ mensaje() }}</p> }
      @if (error()) { <p class="error" role="alert">{{ error() }}</p> }
    </section>
  `
})
export class SaludoComponent {
  private readonly api = inject(HolaMundoApiService);

  nombre = '';
  mensaje = signal('');
  error = signal('');
  cargando = signal(false);

  saludar(): void {
    this.cargando.set(true);
    this.error.set('');
    this.api.saludar(this.nombre).subscribe({
      next: r => { this.mensaje.set(r.mensaje); this.cargando.set(false); },
      error: (e: Error) => { this.error.set(e.message); this.mensaje.set(''); this.cargando.set(false); }
    });
  }
}
