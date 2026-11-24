# FO4 One-Click LIP Generator

A lightweight, standalone one-click `.lip` lip-sync generator for **Fallout 4** modders.

- **Download on Nexus Mods:** https://www.nexusmods.com/profile/lodmac

## How It Works
- Automatically creates an `Audio` folder next to the executable.
- Detects `FonixData.cdf` from your installed Fallout 4 or Skyrim Creation Kit via Windows Registry or Steam libraries.
- Batch-converts all 16-bit PCM Mono 44100 Hz `.wav` files in `Audio` into `.lip` files in one click.

## Build Instructions
1. Place `Program.cs` and the official `FaceFXWrapper.exe` (by Nukem9, MIT License, renamed to `fxwrap.exe`) in the same folder.
2. Compile using the standard Windows .NET C# compiler:
   `csc.exe /target:winexe /out:"FO4 LIP Generator by Lodmac.exe" /resource:fxwrap.exe Program.cs`

## Credits
- Created by **Lodmac**
- Powered by [FaceFXWrapper]([https://github.com](https://github.com/Nukem9/FaceFXWrapper)) by **Nukem9** (MIT License)
 
