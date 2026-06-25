export interface Player {
  id: string;
  name: string;
  gamesPlayed: number;
  wins: number;
  losses: number;
  winRate: number;
}

export interface LeaderboardEntry {
  rank: number;
  id: string;
  name: string;
  wins: number;
  losses: number;
  gamesPlayed: number;
  winRate: number;
}

export interface GameSummary {
  gameId: string;
  title: string;
  hostName: string;
  width: number;
  height: number;
  shipCount: number;
  shipsMayTouch: boolean;
  extraTurnOnHit: boolean;
  createdAt: string;
}

export interface Coordinate {
  x: number;
  y: number;
}

export interface Ship {
  cells: Coordinate[];
}

export type ShotResult = 'Miss' | 'Hit' | 'Sunk';

export interface CellShot {
  x: number;
  y: number;
  result: ShotResult;
}

export interface BoardView {
  width: number;
  height: number;
  ships: Ship[];
  shots: CellShot[];
}

export interface ShipSpec {
  length: number;
  count: number;
}

export type GameStatus = 'WaitingForOpponent' | 'PlacingShips' | 'InProgress' | 'Finished';

export interface GameState {
  gameId: string;
  title: string;
  status: GameStatus;
  width: number;
  height: number;
  fleet: ShipSpec[];
  shipsMayTouch: boolean;
  extraTurnOnHit: boolean;
  yourPlayerId: string;
  youAreHost: boolean;
  hostName: string;
  opponentName: string | null;
  youReady: boolean;
  opponentReady: boolean;
  currentTurnPlayerId: string | null;
  isYourTurn: boolean;
  winnerId: string | null;
  endedByForfeit: boolean;
  yourBoard: BoardView;
  enemyBoard: BoardView;
}

export type Orientation = 'Horizontal' | 'Vertical';

export interface ShipPlacementInput {
  x: number;
  y: number;
  orientation: Orientation;
  length: number;
}

export interface CreateGameRequest {
  width: number;
  height: number;
  fleet: ShipSpec[];
  shipsMayTouch: boolean;
  extraTurnOnHit: boolean;
  title: string | null;
}
