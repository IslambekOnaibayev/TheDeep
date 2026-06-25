import { ChangeDetectionStrategy, Component, computed, input, OnInit, output, signal } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { TranslocoModule } from '@jsverse/transloco';

import { Coordinate, Orientation, ShipPlacementInput, ShipSpec } from '../../../core/models';

interface ShipItem {
  id: number;
  length: number;
  orientation: Orientation;
  bow: Coordinate | null;
}

type ShipPos = 'h-bow' | 'h-mid' | 'h-stern' | 'v-bow' | 'v-mid' | 'v-stern' | 'single';

interface CellVm {
  x: number;
  y: number;
  occupied: boolean;
  shipPos: ShipPos | null;
  preview: 'none' | 'valid' | 'invalid';
}

@Component({
  selector: 'app-placement',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [MatButtonModule, MatIconModule, TranslocoModule],
  templateUrl: './placement.html',
  styleUrl: './placement.scss',
})
export class Placement implements OnInit {
  readonly width = input.required<number>();
  readonly height = input.required<number>();
  readonly fleet = input.required<ShipSpec[]>();
  readonly shipsMayTouch = input<boolean>(false);

  readonly confirm = output<ShipPlacementInput[]>();

  readonly ships = signal<ShipItem[]>([]);
  readonly selectedId = signal<number | null>(null);
  readonly hover = signal<Coordinate | null>(null);

  ngOnInit() {

    this.reset();
  }

  readonly unplaced = computed(() => this.ships().filter((s) => s.bow === null));
  readonly allPlaced = computed(() => this.ships().length > 0 && this.ships().every((s) => s.bow !== null));
  readonly remaining = computed(() => this.unplaced().length);

  private readonly occupied = computed(() => {
    const map = new Map<string, number>();
    for (const ship of this.ships()) {
      if (ship.bow === null) continue;
      for (const c of cellsOf(ship)) map.set(key(c), ship.id);
    }
    return map;
  });

  private readonly shipPositions = computed(() => {
    const map = new Map<string, ShipPos>();
    for (const ship of this.ships()) {
      if (ship.bow === null) continue;
      const cells = cellsOf(ship);
      if (cells.length === 1) {
        map.set(key(cells[0]), 'single');
        continue;
      }
      const dir = ship.orientation === 'Horizontal' ? 'h' : 'v';
      cells.forEach((c, i) => {
        const end = i === 0 ? 'bow' : i === cells.length - 1 ? 'stern' : 'mid';
        map.set(key(c), `${dir}-${end}` as ShipPos);
      });
    }
    return map;
  });

  readonly grid = computed<CellVm[][]>(() => {
    const occ = this.occupied();
    const shipPos = this.shipPositions();
    const previewCells = this.previewCells();
    const previewValid = this.previewValid();
    const rows: CellVm[][] = [];
    for (let y = 0; y < this.height(); y++) {
      const row: CellVm[] = [];
      for (let x = 0; x < this.width(); x++) {
        const k = key({ x, y });
        const inPreview = previewCells.has(k);
        row.push({
          x,
          y,
          occupied: occ.has(k),
          shipPos: shipPos.get(k) ?? null,
          preview: inPreview ? (previewValid ? 'valid' : 'invalid') : 'none',
        });
      }
      rows.push(row);
    }
    return rows;
  });

  private readonly selectedShip = computed(() => this.ships().find((s) => s.id === this.selectedId()) ?? null);

  private readonly previewCells = computed(() => {
    const ship = this.selectedShip();
    const at = this.hover();
    const set = new Set<string>();
    if (!ship || !at) return set;
    for (const c of cellsOf({ ...ship, bow: at })) set.add(key(c));
    return set;
  });

  private readonly previewValid = computed(() => {
    const ship = this.selectedShip();
    const at = this.hover();
    if (!ship || !at) return false;
    return this.canPlace(ship.id, at, ship.orientation);
  });

  select(id: number) {
    this.selectedId.set(this.selectedId() === id ? null : id);
  }

  rotateSelected() {
    const id = this.selectedId();
    if (id === null) return;
    this.ships.update((ships) =>
      ships.map((s) => (s.id === id ? { ...s, orientation: flip(s.orientation) } : s)),
    );

    const ship = this.ships().find((s) => s.id === id)!;
    if (ship.bow && !this.canPlace(id, ship.bow, ship.orientation)) {
      this.setBow(id, null);
    }
  }

  onCellEnter(x: number, y: number) {
    this.hover.set({ x, y });
  }

  onCellLeaveGrid() {
    this.hover.set(null);
  }

  onCellClick(x: number, y: number) {

    const occupantId = this.occupied().get(key({ x, y }));
    if (occupantId !== undefined) {
      this.liftToTray(occupantId);
      return;
    }

    const ship = this.selectedShip();
    if (ship && this.canPlace(ship.id, { x, y }, ship.orientation)) {
      this.setBow(ship.id, { x, y });
      this.selectNextUnplaced();
    }
  }

  liftToTray(id: number) {
    this.setBow(id, null);
    this.selectedId.set(id);
  }

  random() {
    const layout = this.generateRandom();
    if (layout) {
      this.ships.set(layout);
      this.selectNextUnplaced();
    }
  }

  reset() {
    const items: ShipItem[] = [];
    let id = 0;
    const expanded = this.fleet()
      .flatMap((s) => Array.from({ length: s.count }, () => s.length))
      .sort((a, b) => b - a);
    for (const length of expanded) items.push({ id: id++, length, orientation: 'Horizontal', bow: null });
    this.ships.set(items);
    this.selectedId.set(items[0]?.id ?? null);
    this.hover.set(null);
  }

  ready() {
    if (!this.allPlaced()) return;
    const placements: ShipPlacementInput[] = this.ships().map((s) => ({
      x: s.bow!.x,
      y: s.bow!.y,
      orientation: s.orientation,
      length: s.length,
    }));
    this.confirm.emit(placements);
  }

  private setBow(id: number, bow: Coordinate | null) {
    this.ships.update((ships) => ships.map((s) => (s.id === id ? { ...s, bow } : s)));
  }

  private selectNextUnplaced() {
    const next = this.ships().find((s) => s.bow === null);
    this.selectedId.set(next ? next.id : null);
  }

  private canPlace(id: number, bow: Coordinate, orientation: Orientation): boolean {
    const length = this.ships().find((s) => s.id === id)!.length;
    const cells = cellsOf({ length, orientation, bow });
    const w = this.width();
    const h = this.height();

    if (cells.some((c) => c.x < 0 || c.y < 0 || c.x >= w || c.y >= h)) return false;

    const others = new Set<string>();
    for (const ship of this.ships()) {
      if (ship.id === id || ship.bow === null) continue;
      for (const c of cellsOf(ship)) others.add(key(c));
    }

    for (const c of cells) {
      if (others.has(key(c))) return false;
      if (!this.shipsMayTouch()) {
        for (const n of neighbors8(c)) if (others.has(key(n))) return false;
      }
    }
    return true;
  }

  private generateRandom(): ShipItem[] | null {
    const lengths = this.fleet()
      .flatMap((s) => Array.from({ length: s.count }, () => s.length))
      .sort((a, b) => b - a);
    const w = this.width();
    const h = this.height();

    for (let attempt = 0; attempt < 200; attempt++) {
      const placed: ShipItem[] = [];
      const occ = new Set<string>();
      let ok = true;

      for (let i = 0; i < lengths.length; i++) {
        const length = lengths[i];
        let positioned = false;
        for (let tries = 0; tries < 200 && !positioned; tries++) {
          const orientation: Orientation = Math.random() < 0.5 ? 'Horizontal' : 'Vertical';
          const bow = {
            x: Math.floor(Math.random() * w),
            y: Math.floor(Math.random() * h),
          };
          const cells = cellsOf({ length, orientation, bow });
          if (cells.some((c) => c.x < 0 || c.y < 0 || c.x >= w || c.y >= h)) continue;
          if (cells.some((c) => occ.has(key(c)))) continue;
          if (!this.shipsMayTouch() && cells.some((c) => neighbors8(c).some((n) => occ.has(key(n))))) continue;

          for (const c of cells) occ.add(key(c));
          placed.push({ id: i, length, orientation, bow });
          positioned = true;
        }
        if (!positioned) {
          ok = false;
          break;
        }
      }
      if (ok) return placed;
    }
    return null;
  }
}

function cellsOf(ship: { length: number; orientation: Orientation; bow: Coordinate | null }): Coordinate[] {
  if (!ship.bow) return [];
  const cells: Coordinate[] = [];
  for (let i = 0; i < ship.length; i++) {
    cells.push(
      ship.orientation === 'Horizontal'
        ? { x: ship.bow.x + i, y: ship.bow.y }
        : { x: ship.bow.x, y: ship.bow.y + i },
    );
  }
  return cells;
}

function neighbors8(c: Coordinate): Coordinate[] {
  const out: Coordinate[] = [];
  for (let dy = -1; dy <= 1; dy++)
    for (let dx = -1; dx <= 1; dx++) if (dx || dy) out.push({ x: c.x + dx, y: c.y + dy });
  return out;
}

function flip(o: Orientation): Orientation {
  return o === 'Horizontal' ? 'Vertical' : 'Horizontal';
}

function key(c: Coordinate): string {
  return `${c.x},${c.y}`;
}
