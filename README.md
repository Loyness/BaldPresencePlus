![Baldi's Basics Plus](.images/Plus_Logo.png)

# Rich Presence for Baldi's Basics Plus
Have you ever wanted to share information about the current status of your Baldi's Basics Plus run on your Discord profile? No? Well, now you can with the **BaldPresence+** BepInEx plugin!

---

## Requirements
* [Baldi's Basics Plus 0.5+](https://store.steampowered.com/app/1275890/Baldis_Basics_Plus/)
* [BepInEx 5 (64-Bit)](https://github.com/BepInEx/BepInEx/releases)
* [MissingTextureMan101's BB+ Dev API](https://gamebanana.com/mods/383711)
* [Discord Game SDK 3.2.1](https://dl-game-sdk.discordapp.net/3.2.1/discord_game_sdk.zip) — [(archive mirror)](https://web.archive.org/web/20260211110403/https://dl-game-sdk.discordapp.net/3.2.1/discord_game_sdk.zip)
* Discord running with Rich Presence enabled (see notes below for Linux)

---

## Setup

### Windows
1. From the Discord Game SDK zip, extract `lib/x86_64/discord_game_sdk.dll` and place it in the game's root folder (next to `BALDI.exe`).
2. Place `BaldPresencePlus.dll` in `BepInEx/plugins/`.
3. Start the game!

### Linux
1. From the Discord Game SDK zip, extract `lib/x86_64/discord_game_sdk.so` and place it in `BepInEx/plugins/`.
2. Place `BaldPresencePlus.dll` in `BepInEx/plugins/`.
3. Start the game!

---

## Linux — Discord / Rich Presence notes

The Discord Game SDK communicates with Discord over a local socket (`/tmp/discord-ipc-0`).
Depending on how you run Discord, this socket may not be available to the game. If you are running Discord in a sandbox (e.g. Flatpak), you will need to run the game in the same sandbox for Rich Presence to work. So **using Flatpak version of Discord is not recommended**.

---

## Building from source

Requirements: [.NET SDK 6+](https://dotnet.microsoft.com/download)

Set your game path by creating `BaldPresencePlus/BaldPresencePlus.csproj.user` (this file is gitignored):

**Linux**
```xml
<Project>
  <PropertyGroup>
    <GameDir>/home/yourname/.local/share/Steam/steamapps/common/Baldi's Basics Plus</GameDir>
  </PropertyGroup>
</Project>
```

**Windows**
```xml
<Project>
  <PropertyGroup>
    <GameDir>C:\Program Files (x86)\Steam\steamapps\common\Baldi's Basics Plus</GameDir>
  </PropertyGroup>
</Project>
```

Then build — the DLL is copied to `BepInEx/plugins/` automatically:
```sh
dotnet build
```
