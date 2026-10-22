# Metal Gear Snake

Prototype made from the Unity snake tutorial. Inspired by Metal Gear Solid: the snake is Solid Snake sneaking past rooms.

## How to play

- Arrow keys / WASD to move. Snake starts moving when you press a key.
- Crawl into a goon to take him down. Your bandana gets longer every time (like eating food in Snake).
- If you hit a wall or your own bandana, you get spotted (!) and the room restarts.
- The exit (green arrows) is locked until all the goons are down.
- Get to the exit to go to a new random room.

## Scripts

- Snake.cs - based on the tutorial snake code. Snake still moves one square at a time but slides between squares so it looks smooth, and it remembers up to 2 turns so quick key presses don't get lost. The tail is Snake's bandana.
- Goon.cs - changes the goon to the knocked out sprite.
- VentLevel.cs - counts the goons in the room, opens the exit once they're all down, and restarts the room if you get spotted. There are 4 rooms (Room 1 to Room 4 in the Scenes folder) and it loads a random different one when you clear a room.
