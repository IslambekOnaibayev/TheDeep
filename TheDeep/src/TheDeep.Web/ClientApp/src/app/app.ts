import { ChangeDetectionStrategy, Component, computed, effect, inject, untracked } from '@angular/core';
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { MatSnackBar } from '@angular/material/snack-bar';
import { TranslocoModule, TranslocoService } from '@jsverse/transloco';

import { SessionService } from './core/session.service';
import { GameHubService } from './core/game-hub.service';
import { LanguageService } from './core/language.service';

@Component({
  selector: 'app-root',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [
    RouterOutlet,
    RouterLink,
    RouterLinkActive,
    MatToolbarModule,
    MatButtonModule,
    MatIconModule,
    MatMenuModule,
    TranslocoModule,
  ],
  templateUrl: './app.html',
  styleUrl: './app.scss',
})
export class App {
  protected readonly session = inject(SessionService);
  protected readonly hub = inject(GameHubService);
  protected readonly lang = inject(LanguageService);
  private readonly router = inject(Router);
  private readonly snackBar = inject(MatSnackBar);
  private readonly transloco = inject(TranslocoService);

  /** True while the player is inside a game room — nav is hidden so they can't lose it. */
  protected readonly inGame = computed(() => this.hub.gameState() !== null);

  /** The brand logo returns to the active room when in a game, otherwise to the lobby. */
  protected readonly brandLink = computed<unknown[]>(() => {
    const state = this.hub.gameState();
    if (state) return ['/game', state.gameId];
    return this.session.isSignedIn() ? ['/lobby'] : ['/'];
  });

  constructor() {

    effect(() => {
      const message = this.hub.lastError();
      if (!message) return;
      untracked(() => {
        this.snackBar.open(message, this.transloco.translate('common.close'), { duration: 4000 });
        this.hub.clearError();
      });
    });

    let lastReplaced = 0;
    effect(() => {
      const replaced = this.hub.sessionReplaced();
      if (replaced === lastReplaced) return;
      lastReplaced = replaced;
      untracked(() => {
        // The same identity took over in another window — exit quietly without
        // forfeiting the game (it continues in the new window).
        this.clearSession();
        this.snackBar.open(
          this.transloco.translate('common.sessionReplaced'),
          this.transloco.translate('common.close'),
          { duration: 6000 },
        );
      });
    });

    let lastCancelled = 0;
    effect(() => {
      const cancelled = this.hub.gameCancelled();
      if (cancelled === lastCancelled) return;
      lastCancelled = cancelled;
      untracked(() => {
        this.hub.clearGame();
        this.snackBar.open(
          this.transloco.translate('battle.cancelled'),
          this.transloco.translate('common.close'),
          { duration: 5000 },
        );
        this.router.navigateByUrl('/lobby');
      });
    });
  }

  async signOut() {
    // Logging out while in a room must clear the room and notify the opponent
    // immediately — leave the game before tearing down the connection.
    const game = this.hub.gameState();
    if (game) await this.hub.leaveGame(game.gameId).catch(() => undefined);
    this.clearSession();
  }

  private clearSession() {
    this.hub.stop();
    this.session.signOut();
    this.router.navigateByUrl('/');
  }
}
