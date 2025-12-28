# Brut.NoEducationPopups

**Automatically completes child education popups in Mount & Blade II: Bannerlord.**

No more clicking through education screens for every child in your clan!

## Features

- Automatically selects education options when children reach ages 2, 5, 8, 11, 14, 16
- **Random Mode** (default) - picks random options
- **Attribute Priority Mode** - prefers options that give selected attributes
- Configurable via MCM (Mod Configuration Menu)
- Shows optional notifications when education is completed

## MCM Settings

| Setting | Description |
|---------|-------------|
| Enable Mod | Turn the mod on/off |
| Selection Mode | Random or Attribute Priority |
| Show Notifications | Display message when education completes |
| Preferred Attributes | Vigor, Control, Endurance, Cunning, Social, Intelligence |

## Requirements

- Mount & Blade II: Bannerlord (tested on v1.2.x)
- [Bannerlord.Harmony](https://www.nexusmods.com/mountandblade2bannerlord/mods/2006)
- [ButterLib](https://www.nexusmods.com/mountandblade2bannerlord/mods/2018)
- [Mod Configuration Menu (MCM)](https://www.nexusmods.com/mountandblade2bannerlord/mods/612)

## Installation

1. Install required mods (Harmony, ButterLib, MCM)
2. Download and extract to `Modules/` folder
3. Enable in launcher (load after MCM)

## Load Order

```
Bannerlord.Harmony
Bannerlord.ButterLib
Bannerlord.MBOptionScreen
Native
SandBoxCore
Sandbox
...
Brut.NoEducationPopups
```

## Building from Source

```bash
cd Brut.NoEducationPopups
dotnet build -c Release -p:Platform=x64
```

## License

MIT License - Open Source

## Author

**Brut** - [GitHub](https://github.com/markbrutx/Brut.NoEducationPopups)

---

*Part of Brut's Bannerlord Mods collection*
