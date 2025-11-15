# Lizeria02 Game Specification

## Overview
"Lizeria02" is a small top-down arcade game built with Unity. The player navigates a pilot drone inside a square arena, collecting floating energy crystals while avoiding patrolling sentry drones. Sessions are time-limited, and the objective is to gather as many crystals as possible (at least 10) before time runs out or the drone is hit by an enemy.

## Core Gameplay
- **Perspective:** 2D top-down view rendered with an orthographic camera.
- **Player Controls:**
  - Movement with WASD or arrow keys.
  - Movement is analog, allowing diagonal movement with normalized speed.
- **Objective:**
  - Collect randomly spawned crystals to increase the score.
  - Reach a target score of 10 crystals before the timer reaches zero.
  - Getting hit by an enemy ends the run immediately.
- **Boundaries:** The arena is a square playfield approximately 16 units wide. Pickups and enemies spawn inside this region.

## Game Loop
1. When the scene loads, the player starts in the center of the arena with an instruction message.
2. Four crystals are spawned at random positions.
3. Enemies start appearing a few seconds after the round begins, continuing at regular intervals.
4. The player collects crystals while avoiding enemies until one of the following conditions is met:
   - The player reaches the target score (win).
   - The timer reaches zero (lose).
   - The player collides with an enemy (lose).
5. After the round ends, a message appears with the result and prompts the player to press **R** to restart the game.

## Entities
- **Player Drone**
  - Components: `SpriteRenderer`, `Rigidbody2D`, `CircleCollider2D`, `PlayerController`.
  - Visual: Built-in square sprite tinted blue.
  - Tag: `Player`.
- **Crystal Pickup**
  - Components: `SpriteRenderer`, `CircleCollider2D` (trigger), `Pickup`.
  - Visual: Built-in square sprite tinted yellow, rotates slowly.
  - Tag: `Pickup`.
- **Enemy Drone**
  - Components: `SpriteRenderer`, `CircleCollider2D` (trigger), `EnemyController`.
  - Visual: Built-in square sprite tinted red.
  - Tag: `Enemy`.
- **Game Controller**
  - Component: `GameController` (on an empty GameObject).
  - Responsible for round lifecycle, spawning crystals/enemies, scorekeeping, and UI updates.

## User Interface
- **Score Text:** Displays `Score: <value>`.
- **Timer Text:** Displays `Time: <seconds>` truncated to whole seconds.
- **Message Text:**
  - Shows instructions during play.
  - Displays win/lose messages and restart prompt after the round ends.
- The UI is presented on a `Canvas` in Screen Space - Overlay with anchored text elements at the top (score, timer) and center (message).

## Audio & Visual Feedback
- Placeholder visuals use Unity's built-in sprites. No audio is required for this iteration.
- Pickups rotate to differentiate them from enemies.
- Enemy drones continuously move toward the player.

## Restart Flow
- Pressing **R** while the game is over restarts the round immediately.
- Pressing **R** during play has no effect.

## Extensibility Notes
- Game parameters such as arena size, spawn intervals, and target score are adjustable via serialized fields on the `GameController`.
- Enemy speed and spawn frequency can be tuned without code changes.

