import { ChangeDetectionStrategy, Component, computed, input, output } from '@angular/core';
import { CellShot, Coordinate, Ship } from '../../core/models';

type CellKind = 'water' | 'ship' | 'miss' | 'hit' | 'sunk';
type ShipPos = 'h-bow' | 'h-mid' | 'h-stern' | 'v-bow' | 'v-mid' | 'v-stern' | 'single';

interface CellVm {
  x: number;
  y: number;
  kind: CellKind;
  clickable: boolean;
  justFired: boolean;
  shipPos: ShipPos | null;
}

const LETTERS = 'ABCDEFGHIJKLMNOPQRSTUVWXYZ';

@Component({
  selector: 'app-board-grid',
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './board-grid.html',
  styleUrl: './board-grid.scss',
})
export class BoardGrid {
  readonly width = input.required<number>();
  readonly height = input.required<number>();
  readonly ships = input<Ship[]>([]);
  readonly shots = input<CellShot[]>([]);
  readonly showShips = input<boolean>(true);
  readonly interactive = input<boolean>(false);
  readonly lastShot = input<Coordinate | null>(null);

  readonly cellClick = output<Coordinate>();

  readonly columns = computed(() => Array.from({ length: this.width() }, (_, i) => LETTERS[i] ?? `${i + 1}`));
  readonly rowLabels = computed(() => Array.from({ length: this.height() }, (_, i) => i + 1));

  readonly rows = computed<CellVm[][]>(() => {
    const shipSet = new Set<string>();
    const shipPosMap = new Map<string, ShipPos>();
    for (const ship of this.ships()) {
      for (const c of ship.cells) shipSet.add(key(c.x, c.y));
      assignShipPositions(ship.cells, shipPosMap);
    }

    const shotMap = new Map<string, CellShot>();
    for (const s of this.shots()) shotMap.set(key(s.x, s.y), s);

    const last = this.lastShot();
    const showShips = this.showShips();
    const interactive = this.interactive();
    const grid: CellVm[][] = [];

    for (let y = 0; y < this.height(); y++) {
      const row: CellVm[] = [];
      for (let x = 0; x < this.width(); x++) {
        const k = key(x, y);
        const shot = shotMap.get(k);
        let kind: CellKind = 'water';
        if (shot) kind = shot.result === 'Miss' ? 'miss' : shot.result === 'Sunk' ? 'sunk' : 'hit';
        else if (showShips && shipSet.has(k)) kind = 'ship';

        row.push({
          x,
          y,
          kind,
          clickable: interactive && !shot,
          justFired: !!last && last.x === x && last.y === y,
          shipPos: kind === 'ship' ? shipPosMap.get(k) ?? null : null,
        });
      }
      grid.push(row);
    }
    return grid;
  });

  onCell(cell: CellVm) {
    if (cell.clickable) this.cellClick.emit({ x: cell.x, y: cell.y });
  }
}

function key(x: number, y: number): string {
  return `${x},${y}`;
}

/** Tags each cell of a ship as bow/mid/stern of a horizontal or vertical hull. */
function assignShipPositions(cells: Coordinate[], out: Map<string, ShipPos>): void {
  if (cells.length === 0) return;
  if (cells.length === 1) {
    out.set(key(cells[0].x, cells[0].y), 'single');
    return;
  }
  const horizontal = cells[0].y === cells[1].y;
  const sorted = [...cells].sort((a, b) => (horizontal ? a.x - b.x : a.y - b.y));
  const dir = horizontal ? 'h' : 'v';
  sorted.forEach((c, i) => {
    const end = i === 0 ? 'bow' : i === sorted.length - 1 ? 'stern' : 'mid';
    out.set(key(c.x, c.y), `${dir}-${end}` as ShipPos);
  });
}
