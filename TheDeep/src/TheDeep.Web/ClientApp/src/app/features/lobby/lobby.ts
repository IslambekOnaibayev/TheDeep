import { ChangeDetectionStrategy, Component, computed, inject, OnInit, signal } from '@angular/core';
import { Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatDialog } from '@angular/material/dialog';
import { TranslocoModule } from '@jsverse/transloco';

import { GameHubService } from '../../core/game-hub.service';
import { SessionService } from '../../core/session.service';
import { GameSummary, CreateGameRequest } from '../../core/models';
import { CreateGameDialog } from './create-game-dialog';

@Component({
  selector: 'app-lobby',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatChipsModule,
    MatFormFieldModule,
    MatInputModule,
    TranslocoModule,
  ],
  templateUrl: './lobby.html',
  styleUrl: './lobby.scss',
})
export class Lobby implements OnInit {
  protected readonly hub = inject(GameHubService);
  private readonly session = inject(SessionService);
  private readonly router = inject(Router);
  private readonly dialog = inject(MatDialog);

  readonly query = signal('');

  /** Open games filtered by room title or host name (case-insensitive). */
  readonly filteredGames = computed(() => {
    const q = this.query().trim().toLowerCase();
    const games = this.hub.lobby();
    if (!q) return games;
    return games.filter(
      (g) => g.title.toLowerCase().includes(q) || g.hostName.toLowerCase().includes(q),
    );
  });

  onSearch(value: string) {
    this.query.set(value);
  }

  ngOnInit() {
    this.hub.clearGame();
    this.hub.ensureStarted().then(() => this.hub.refreshLobby());
  }

  isMine(game: GameSummary): boolean {
    return game.hostName === this.session.player()?.name;
  }

  async openCreate() {
    const ref = this.dialog.open(CreateGameDialog, { width: '520px', maxWidth: '94vw' });
    const request: CreateGameRequest | undefined = await ref.afterClosed().toPromise();
    if (!request) return;
    const gameId = await this.hub.createGame(request);
    this.router.navigate(['/game', gameId]);
  }

  async join(game: GameSummary) {
    if (this.isMine(game)) {
      this.router.navigate(['/game', game.gameId]);
      return;
    }
    await this.hub.joinGame(game.gameId);
    this.router.navigate(['/game', game.gameId]);
  }

  refresh() {
    this.hub.refreshLobby();
  }
}
