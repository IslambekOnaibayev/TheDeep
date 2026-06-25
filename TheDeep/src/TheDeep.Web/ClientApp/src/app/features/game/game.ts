import { ChangeDetectionStrategy, Component, computed, inject, input, OnDestroy, OnInit, signal } from '@angular/core';
import { Router } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { TranslocoModule } from '@jsverse/transloco';

import { GameHubService } from '../../core/game-hub.service';
import { SessionService } from '../../core/session.service';
import { Coordinate, ShipPlacementInput } from '../../core/models';
import { BoardGrid } from '../../shared/board-grid/board-grid';
import { Placement } from './placement/placement';

@Component({
  selector: 'app-game',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    TranslocoModule,
    BoardGrid,
    Placement,
  ],
  templateUrl: './game.html',
  styleUrl: './game.scss',
})
export class Game implements OnInit, OnDestroy {
  readonly id = input.required<string>();

  protected readonly hub = inject(GameHubService);
  private readonly session = inject(SessionService);
  private readonly router = inject(Router);

  readonly state = this.hub.gameState;
  readonly lastShot = signal<Coordinate | null>(null);
  readonly loading = signal(true);

  readonly opponentName = computed(() => this.state()?.opponentName ?? '');
  readonly yourName = computed(() => this.session.player()?.name ?? this.state()?.hostName ?? '');
  readonly youWon = computed(() => {
    const s = this.state();
    return !!s && s.status === 'Finished' && s.winnerId === s.yourPlayerId;
  });

  async ngOnInit() {
    try {
      await this.hub.ensureStarted();
      await this.hub.requestState(this.id());
    } catch {
      this.router.navigate(['/lobby']);
    } finally {
      this.loading.set(false);
    }
  }

  ngOnDestroy() {
    // Leaving the game screen by ANY means (button, result card, or typing a
    // different URL) counts as leaving the room. If the game is still live, tell
    // the server so the opponent is notified (forfeit win, or cancel during setup).
    const s = this.state();
    if (s && s.status !== 'Finished') {
      this.hub.leaveGame(s.gameId).catch(() => undefined);
    }
    this.hub.clearGame();
    this.session.refreshStats();
  }

  onConfirmPlacement(placements: ShipPlacementInput[]) {
    this.hub.placeShips(this.id(), placements);
  }

  fire(coord: Coordinate) {
    this.lastShot.set(coord);
    this.hub.fireShot(this.id(), coord.x, coord.y);
  }

  leave() {
    this.router.navigate(['/lobby']);
  }

  backToLobby() {
    this.router.navigate(['/lobby']);
  }
}
