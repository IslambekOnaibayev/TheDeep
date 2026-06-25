import { computed, inject, Injectable, signal } from '@angular/core';
import { tap } from 'rxjs';
import { ApiService } from './api.service';
import { Player } from './models';

const PLAYER_ID_KEY = 'thedeep.playerId';
const PLAYER_NAME_KEY = 'thedeep.name';

// Identity is stored per browser window (sessionStorage), NOT per profile
// (localStorage). Two windows of the same browser are therefore two distinct
// players, so signing in with the same name yields "Matt" and "Matt 2" rather
// than the second window hijacking the first one's identity. A reload keeps the
// same identity because sessionStorage survives it.
const store = sessionStorage;

@Injectable({ providedIn: 'root' })
export class SessionService {
  private readonly api = inject(ApiService);

  private readonly _player = signal<Player | null>(this.restore());
  readonly player = this._player.asReadonly();
  readonly isSignedIn = computed(() => this._player() !== null);

  get playerId(): string | null {
    return this._player()?.id ?? store.getItem(PLAYER_ID_KEY);
  }

  signIn(name: string) {
    return this.api.signIn(name, this.playerId).pipe(tap((player) => this.persist(player)));
  }

  refreshStats() {
    const id = this.playerId;
    if (!id) return;
    this.api.getStats(id).subscribe((player) => this.persist(player));
  }

  signOut() {
    store.removeItem(PLAYER_ID_KEY);
    store.removeItem(PLAYER_NAME_KEY);
    this._player.set(null);
  }

  private persist(player: Player) {
    store.setItem(PLAYER_ID_KEY, player.id);
    store.setItem(PLAYER_NAME_KEY, player.name);
    this._player.set(player);
  }

  private restore(): Player | null {
    const id = store.getItem(PLAYER_ID_KEY);
    const name = store.getItem(PLAYER_NAME_KEY);
    if (!id || !name) return null;
    return { id, name, gamesPlayed: 0, wins: 0, losses: 0, winRate: 0 };
  }
}
