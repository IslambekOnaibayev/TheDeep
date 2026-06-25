import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { LeaderboardEntry, Player } from './models';

@Injectable({ providedIn: 'root' })
export class ApiService {
  private readonly http = inject(HttpClient);

  signIn(name: string, playerId: string | null) {
    return this.http.post<Player>('/api/players', { name, playerId });
  }

  getStats(playerId: string) {
    return this.http.get<Player>(`/api/players/${playerId}/stats`);
  }

  getLeaderboard(top = 20) {
    return this.http.get<LeaderboardEntry[]>(`/api/leaderboard?top=${top}`);
  }
}
