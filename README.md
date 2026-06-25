# The Deep — Multiplayer Battleship

The Deep is an online Battleship game you can play against another person in real time.
There's no sign-up and no passwords — you just type a name and you're in. If someone is
already using that name, you simply become "John 2", "John 3", and so on. From the lobby
you can spin up your own game (choosing the board size and which ships are in play) or jump
into someone else's. Lots of games can run side by side, each between its own pair of players.

Every move travels over a live WebSocket connection, so shots land on your opponent's screen
the instant you fire them — no refreshing, no waiting.

## What you can do

- **Find a game fast.** Browse open games in the lobby and filter them by room name or by
  the host's name.
- **Set your own rules.** When you create a game you pick the grid size, the fleet, whether
  ships may touch, and whether a hit earns you another shot.
- **Place your fleet your way.** Drag ships onto the board, click to rotate, or hit "Random"
  to drop a valid layout instantly. Invalid placements are blocked as you go.
- **Play in real time.** Shots, hits, misses and sunk ships update live for both players.
- **Pick up where you left off.** Refresh the page mid-match and you land right back in your
  game.
- **Win and lose fairly.** If an opponent walks away mid-battle, you take the win; if they
  leave before the first shot, the game is simply cancelled — nobody's stats are touched.
- **Track your record.** Wins, losses and win-rate are saved per name, and there's a global
  leaderboard.
- **Play in your language.** English, Russian, Kazakh and Belarusian, switchable any time from
  the header.

## How it's built

| Layer | Technology |
|-------|------------|
| Backend | .NET 10, Clean Architecture, FastEndpoints, **SignalR**, Mediator, EF Core, Vogen |
| Database | PostgreSQL |
| Frontend | Angular 21 (standalone components + signals), Angular Material, Transloco |

The backend is split into four projects following Clean Architecture:

- **TheDeep.Core** — the domain: the game, board, ships, players, and the events they raise.
- **TheDeep.UseCases** — application logic (the commands and queries that drive a match).
- **TheDeep.Infrastructure** — PostgreSQL access and the in-memory store of live games.
- **TheDeep.Web** — the SignalR hub, the small REST surface, and the Angular app.

One thing worth knowing: **games in progress live in memory, not in the database.** Only
finished results and player stats are written to PostgreSQL. That keeps active gameplay fast,
at the cost of in-flight games not surviving a server restart.

## Running it locally

You'll need the .NET 10 SDK, Node.js, and Docker (for the database).

**1. Start PostgreSQL:**

```bash
docker compose up -d        # Postgres, reachable on localhost:15432
```

**2. Start the backend:**

```bash
dotnet run --project TheDeep/src/TheDeep.Web
```

The API runs on https://localhost:57679 and applies database migrations automatically in
development.

**3. Start the frontend:**

```bash
cd TheDeep/src/TheDeep.Web/ClientApp
npm install
npm start                   # http://localhost:4200
```

Open two browser windows to play against yourself — each window is treated as a separate
player.

### The whole stack in one command

If you'd rather not run the pieces separately, the included `docker-compose.yml` builds and
runs everything (the SPA is baked into the API image):

```bash
docker compose up --build   # app on http://localhost:8080
```

### Database migrations

```bash
dotnet ef migrations add <Name> --project TheDeep/src/TheDeep.Infrastructure --startup-project TheDeep/src/TheDeep.Web -o Data/Migrations
dotnet ef database update    --project TheDeep/src/TheDeep.Infrastructure --startup-project TheDeep/src/TheDeep.Web
```

## Deploying to Railway

The repository includes a multi-stage `Dockerfile` that builds the Angular app, publishes the
.NET backend, and serves both from a single origin.

1. Create a Railway project and add a **PostgreSQL** database — it provides a `DATABASE_URL`.
2. Point Railway at this repository; it picks up the `Dockerfile` automatically.
3. Set these environment variables on the app service:
   - `DATABASE_URL` — supplied by the PostgreSQL service (the app reads it directly).
   - `Database__ApplyMigrationsOnStartup=true` — runs migrations on first boot.

Railway supports WebSockets out of the box, so real-time play works with no extra setup.
