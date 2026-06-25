import { inject, Injectable, signal } from '@angular/core';
import {
  HubConnection,
  HubConnectionBuilder,
  HubConnectionState,
  LogLevel,
} from '@microsoft/signalr';
import { SessionService } from './session.service';
import { CreateGameRequest, GameState, GameSummary, ShipPlacementInput } from './models';

export type ConnState = 'connecting' | 'connected' | 'reconnecting' | 'disconnected';

@Injectable({ providedIn: 'root' })
export class GameHubService {
  private readonly session = inject(SessionService);
  private connection?: HubConnection;
  private starting?: Promise<void>;

  readonly connState = signal<ConnState>('disconnected');
  readonly lobby = signal<GameSummary[]>([]);
  readonly gameState = signal<GameState | null>(null);
  readonly lastError = signal<string | null>(null);
  readonly sessionReplaced = signal(0);
  readonly gameCancelled = signal(0);

  async ensureStarted(): Promise<void> {
    if (this.connection?.state === HubConnectionState.Connected) return;
    if (this.starting) return this.starting;

    const playerId = this.session.playerId ?? '';
    const name = this.session.player()?.name ?? '';
    const url = `/hub/game?playerId=${encodeURIComponent(playerId)}&name=${encodeURIComponent(name)}`;

    this.connection = new HubConnectionBuilder()
      .withUrl(url)
      .withAutomaticReconnect()
      .configureLogging(LogLevel.Warning)
      .build();

    this.connection.on('ReceiveState', (state: GameState) => this.gameState.set(state));
    this.connection.on('LobbyUpdated', (games: GameSummary[]) => this.lobby.set(games));
    this.connection.on('GameError', (message: string) => this.lastError.set(message));
    this.connection.on('SessionReplaced', () => this.sessionReplaced.update((n) => n + 1));
    this.connection.on('GameCancelled', () => this.gameCancelled.update((n) => n + 1));

    this.connection.onreconnecting(() => this.connState.set('reconnecting'));
    this.connection.onreconnected(() => {
      this.connState.set('connected');
      const current = this.gameState();
      if (current) this.requestState(current.gameId).catch(() => undefined);
    });
    this.connection.onclose(() => this.connState.set('disconnected'));

    this.connState.set('connecting');
    this.starting = this.connection
      .start()
      .then(() => {
        this.connState.set('connected');
      })
      .finally(() => {
        this.starting = undefined;
      });

    return this.starting;
  }

  async stop(): Promise<void> {
    this.lobby.set([]);
    this.gameState.set(null);
    this.lastError.set(null);
    if (this.connection) {
      try {
        await this.connection.stop();
      } catch {
        this.connState.set('disconnected');
      }
      this.connection = undefined;
    }
    this.connState.set('disconnected');
  }

  clearError() {
    this.lastError.set(null);
  }

  clearGame() {
    this.gameState.set(null);
  }

  async createGame(request: CreateGameRequest): Promise<string> {
    return this.invoke<string>('CreateGame', request);
  }

  joinGame(gameId: string) {
    return this.invoke<void>('JoinGame', gameId);
  }

  placeShips(gameId: string, ships: ShipPlacementInput[]) {
    return this.invoke<void>('PlaceShips', gameId, { ships });
  }

  fireShot(gameId: string, x: number, y: number) {
    return this.invoke<void>('FireShot', gameId, x, y);
  }

  leaveGame(gameId: string) {
    return this.invoke<void>('LeaveGame', gameId);
  }

  async requestState(gameId: string): Promise<GameState> {
    const state = await this.invoke<GameState>('RequestState', gameId);
    this.gameState.set(state);
    return state;
  }

  refreshLobby() {
    return this.invoke<void>('RefreshLobby');
  }

  private async invoke<T>(method: string, ...args: unknown[]): Promise<T> {
    await this.ensureStarted();
    try {
      return await this.connection!.invoke<T>(method, ...args);
    } catch (err) {
      const message = err instanceof Error ? this.cleanHubError(err.message) : 'Something went wrong.';
      this.lastError.set(message);
      throw err;
    }
  }

  private cleanHubError(raw: string): string {
    const marker = 'HubException:';
    const idx = raw.indexOf(marker);
    return idx >= 0 ? raw.slice(idx + marker.length).trim() : raw;
  }
}
