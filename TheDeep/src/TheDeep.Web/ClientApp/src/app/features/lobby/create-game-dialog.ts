import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatIconModule } from '@angular/material/icon';
import { TranslocoModule } from '@jsverse/transloco';

import { CreateGameRequest, ShipSpec } from '../../core/models';

type Preset = 'classic' | 'small' | 'custom';

const PRESETS: Record<Exclude<Preset, 'custom'>, { w: number; h: number; fleet: ShipSpec[] }> = {
  classic: { w: 10, h: 10, fleet: [{ length: 4, count: 1 }, { length: 3, count: 2 }, { length: 2, count: 3 }, { length: 1, count: 4 }] },
  small: { w: 8, h: 8, fleet: [{ length: 3, count: 1 }, { length: 2, count: 2 }, { length: 1, count: 3 }] },
};

@Component({
  selector: 'app-create-game-dialog',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatButtonToggleModule,
    MatSlideToggleModule,
    MatIconModule,
    TranslocoModule,
  ],
  templateUrl: './create-game-dialog.html',
  styleUrl: './create-game-dialog.scss',
})
export class CreateGameDialog {
  private readonly ref = inject(MatDialogRef<CreateGameDialog>);

  readonly title = signal('');
  readonly preset = signal<Preset>('classic');
  readonly width = signal(10);
  readonly height = signal(10);
  readonly fleet = signal<ShipSpec[]>(structuredClone(PRESETS.classic.fleet));
  readonly shipsMayTouch = signal(false);
  readonly extraTurnOnHit = signal(true);

  readonly totalCells = computed(() => this.fleet().reduce((sum, s) => sum + s.length * s.count, 0));
  readonly totalShips = computed(() => this.fleet().reduce((sum, s) => sum + s.count, 0));

  readonly valid = computed(() => {
    const fleet = this.fleet();
    if (fleet.length === 0) return false;
    if (fleet.some((s) => s.length < 1 || s.count < 1)) return false;
    const maxLen = Math.max(...fleet.map((s) => s.length));
    if (maxLen > Math.max(this.width(), this.height())) return false;
    return this.totalCells() <= Math.floor((this.width() * this.height()) / 2);
  });

  usePreset(preset: Preset) {
    this.preset.set(preset);
    if (preset === 'custom') return;
    const p = PRESETS[preset];
    this.width.set(p.w);
    this.height.set(p.h);
    this.fleet.set(structuredClone(p.fleet));
  }

  setDimension(which: 'width' | 'height', value: number) {
    const v = Math.max(7, Math.min(12, Math.round(value || 0)));
    (which === 'width' ? this.width : this.height).set(v);
    this.preset.set('custom');
  }

  updateShip(index: number, field: 'length' | 'count', value: number) {
    const v = Math.max(1, Math.round(value || 0));
    this.fleet.update((fleet) => fleet.map((s, i) => (i === index ? { ...s, [field]: v } : s)));
    this.preset.set('custom');
  }

  addShip() {
    this.fleet.update((fleet) => [...fleet, { length: 1, count: 1 }]);
    this.preset.set('custom');
  }

  removeShip(index: number) {
    this.fleet.update((fleet) => fleet.filter((_, i) => i !== index));
    this.preset.set('custom');
  }

  cancel() {
    this.ref.close();
  }

  create() {
    if (!this.valid()) return;
    const request: CreateGameRequest = {
      width: this.width(),
      height: this.height(),
      fleet: this.fleet(),
      shipsMayTouch: this.shipsMayTouch(),
      extraTurnOnHit: this.extraTurnOnHit(),
      title: this.title().trim() || null,
    };
    this.ref.close(request);
  }
}
