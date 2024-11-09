# Multiplayer 2D Arena Shooter - "Rock, Paper, Shoot"

This is a barebones prototype of a multiplayer 2D arena shooter that can be hosted on Unity Game Hosting! Currently in testing, this game is my **first attempt** at implementing multiplayer in Unity.

## 📝 Game Overview

"Rock, Paper, Shoot" combines a classic "Rock-Paper-Scissors" mechanic with an arena shooter format. Players are assigned one of three classes at the start, each with an ammo type that has distinct strengths and weaknesses:

- **Rock** beats **Scissors** class
- **Paper** beats **Rock** class
- **Scissors** beats **Paper** class

Players need to pick up the right ammo type to counter their opponents' hidden classes, using visual and audio cues to strategize and make the best moves. Points are earned by successfully defeating opponents, and the player with the highest score wins (Winning mechanic not yet implemented).

---

## 🎥 Gameplay Preview & 🎨 Game Logo

![GameLogo](https://github.com/user-attachments/assets/704cee52-73f0-471d-8175-4e5a0fdc991e)

![Gameplaypreview](https://github.com/user-attachments/assets/68f6210f-2f71-434b-8bb7-32d9e0ce154c)

---

## 🎮 Game Rules

1. **Collect Ammo and Power-Ups**: Move around the arena to collect ammo and power-ups.
2. **Guess and Shoot**: Use clues from your opponent's reactions to guess their class. Then, fire the right ammo to defeat them!
3. **Survive and Score**: Accumulate points by defeating as many opponents as possible. 

---

## Features

- **Multiplayer Functionality:** Powered by Unity Netcode for GameObjects and Unity Multiplayer Services.
- **Cross-Platform Compatibility:** Connect players across platforms, including Android and Linux server hosting.
- **Dynamic Ammo System:** Unique ammo types (rock, paper, scissors) tied to player classes, with corresponding interactions.
- **Server Authority:** Server-driven physics and logic for game balance and anti-cheat.
- **Radial Boundary Visualization:** A unique gradient boundary visualization as players approach the edges of the game area.

---


### Prerequisites

- **Unity** version 2022.3+ (or compatible version that supports Unity Netcode for GameObjects)
- **Git** for version control
- **Unity Multiplayer Services** account (for hosting in Unity's Multiplay service)

---

### Unity Package Installation

1. **Install Netcode for GameObjects** via Unity's Package Manager:
   - Go to *Window > Package Manager*
   - Search for **Netcode for GameObjects** and install the package.

2. **Install Unity Multiplayer Services SDK**:
   - Search for **Multiplay** and **Multiplayer Services** to install the relevant SDKs in Unity Package Manager.

3. **Other Packages:** Ensure **TMP Essentials** is imported for TextMeshPro support and **Input System** is enabled in project settings.

---

### Configuration

1. **NetworkManager:** 
   - Make sure the `NetworkManager` component is added to a game object in the scene.
   - Enable **Connection Approval** if using a dedicated server with connection validation.

2. **Server Configuration**:
   - Update the server settings in `MultiplayManager.cs` to align with your Multiplay setup.
   - Modify the `ServerConfig` in `MultiplayManager.cs` if needed for custom settings.

3. **Ammo Spawner Setup**:
   - Ensure the `AmmoSpawner` script is configured on a game object in the scene and connected to the prefab pool.

---

## 🚧 Current Issues and Known Bugs

Being a first pass at multiplayer development, there are still a few bugs:

- **Score Tracking**: Score does not always increase after defeating an opponent.
- **Multiple Shots**: Rapidly pressing the fire button sometimes results in multiple shots being fired, even when ammo is low.
- **Sync Issues**: Various small sync issues due to server-client lag.

Feedback and suggestions are highly welcome!

---

## Getting Started

### Installation

1. **Clone the Repository**:
   ```bash
   git clone https://github.com/yourusername/repo-name.git
   cd repo-name
   ```

2. **Install Unity Packages**:
   - Ensure **Netcode for GameObjects** and **Unity Multiplayer Services SDK** are installed via the Unity Package Manager.

### Running the Game

To run the server on Unity Game Hosting:
1. In the MultiplayManager script, uncomment the server side code in order to build for the LinuxServer.
2. Build for **Linux Headless Server** in Unity's Build Settings.
3. Follow Unity Game Hosting documentation to deploy.
4. Comment out the server side code in MultiplayManager script to make a successful build of your preferred system to run on

To connect as a client:
1. Use the **Join Server** button in the game UI and enter the server’s IP address and port.

P.S : If you want to run the multiplayer locally, just click the host/server button at the start in one Unity editor and in another play as a client!
---

## Controls

- **Move**: Joystick on mobile or arrow keys on desktop.
- **Shoot**: Fire button/Right Click to shoot your selected ammo.

---

## Contributing

Feel free to submit issues, fork the repository, and send pull requests. Contributions are welcome for additional features, bug fixes, and optimizations.

## Acknowledgments

- **Unity Multiplayer Services** for hosting.
- **Unity Netcode for GameObjects** for networking.
- **Kenney Assets** for game assets

---
