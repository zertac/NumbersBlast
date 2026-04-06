# Numbers Blast

A puzzle game prototype inspired by Block Blast, where pieces carry numbers (1-4) that merge when placed next to matching values.

## How to Play

1. Drag a piece from the bottom tray onto the board
2. Adjacent blocks with the same value merge and their values are summed
3. Merges can chain — a new sum may match another neighbor
4. Fill a complete row or column to clear it and earn points
5. Game ends when no piece can fit on the board

## Features

- Core puzzle gameplay with merge and chain reaction mechanics
- 3-step interactive tutorial (place, merge, clear)
- Drag-and-drop with placement, merge, and line clear previews
- Fake real-time multiplayer with AI opponent (turn-based, 20s timer)
- ScriptableObject-driven configuration (board, pieces, theme, tutorial, AI)
- Audio, haptic, and visual feedback

## Tech Stack

- Unity 6 (URP 2D)
- VContainer (Dependency Injection)
- DOTween Pro (Animations)
- UniTask (Async/Await)
- TextMeshPro (UI Text)

## Project Structure

```
Scripts/
  Audio/        - Sound management
  Board/        - Board model, view, cell logic
  Core/         - Boot, events, constants, initializer
  Data/         - ScriptableObject configs
  DI/           - VContainer lifetime scopes
  Editor/       - Custom inspectors and tools
  Feedback/     - Visual effects, haptics
  Gameplay/     - Placement, merge, line clear resolvers
  Input/        - Drag-and-drop handling
  Multiplayer/  - AI opponent, turns, HUD
  Piece/        - Piece model, view, tray
  StateMachine/ - Game state management
  Tutorial/     - Tutorial flow, overlay, step data
  UI/           - Popups, HUD, buttons, settings
```

## Build

Open in Unity 6, switch platform to Android/iOS, build.
