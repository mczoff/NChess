# NChess

**NChess** is a lightweight, pure C# chess core focused on correctness, clarity, and extensibility.  
This is an **early alpha release (v0.0.1-alpha)** intended for engine development, analysis tools, and chess-related experiments.

---

## Status

⚠️ **Alpha**

The public API is unstable and may change without notice.

Currently implemented:
- FEN load / save (round-trip safe)
- Legal move generation
- Move application and deterministic undo
- SAN and UCI notation
- PGN import / export (single game)
- Castling, en-passant, promotion
- Debug-friendly board representation

---

## Project Structure

```
NChess
├─ NChess                // High-level Chess facade
├─ NChess.Console        // Console playground / scratchpad
├─ NChess.Core           // Core chess logic
│  ├─ Common             // Board, Position, Square, etc.
│  ├─ Engine             // Engine abstractions
│  │  └─ Classic         // Reference engine implementation
│  ├─ Fen                // FEN parsing and serialization
│  ├─ Moves              // Move, flags, undo
│  ├─ Notation           // SAN / UCI
│  └─ Pieces             // Piece, PieceType
└─ NChess.Core.Pgn       // PGN import / export
```

---

## Quick Example

```csharp
using NChess;

var chess = new Chess();

chess.TryMoveSan("e4", out _);
chess.TryMoveSan("e5", out _);
chess.TryMoveSan("Nf3", out _);

Console.WriteLine(chess.Fen);
Console.WriteLine(chess.ExportPgn());
```

---

## Supported Formats

- **FEN** — load / save
- **SAN** — parse and format (with disambiguation)
- **UCI** — parse and format
- **PGN** — import / export (single game, no variations)

---

## Design Goals

- Correctness first
- Deterministic move undo
- Engine-agnostic architecture
- Minimal allocations
- Easy debugging (ASCII board dump)

---

## Non-Goals (for now)

- Strong evaluation or search
- Opening books or endgame tables
- PGN variations
- GUI
- Aggressive performance tuning

---

## Target Framework

- Core library: **.NET Standard 2.1**
- Tests / tooling: modern .NET (net6+)

---

## License

MIT (planned)

---

**Version:** `0.0.1-alpha`  
**API stability:** ❌ not guaranteed  
**Intended use:** internal tools, experiments, learning, engine prototyping
