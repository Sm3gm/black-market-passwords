# Black Market Passwords

Click into a sticky note, press F8, and the current passwords for the city's black market trader and weapons dealer are written into it.

![A sticky note with both passwords written in by the mod](https://raw.githubusercontent.com/sm3gm/black-market-passwords/main/images/note.png)

If it gets you into a shop you had given up on, please leave a like on the mod's [Thunderstore page](https://thunderstore.io/c/shadows-of-doubt/p/Sm3gm/BlackMarketPasswords/). It keeps me motivated to make more mods. Donations are welcome too:

[![Ko-fi](https://img.shields.io/badge/Ko--fi-sm3gm-FF5E5B?logo=kofi&logoColor=white)](https://ko-fi.com/sm3gm) [![Buy Me a Coffee](https://img.shields.io/badge/Buy%20Me%20a%20Coffee-sm3gm-FFDD00?logo=buymeacoffee&logoColor=black)](https://buymeacoffee.com/sm3gm)

## Why

The black market trader and the weapons dealer only serve you once you say the right password. The only place the game shows it is in graffiti tags painted on walls around town. Sometimes a tag has no letters on it. Sometimes it has moved, or reads a different word than last time. Miss it and you can go a whole city without either shop.

![A gun tag on a wall, with no password on it](https://raw.githubusercontent.com/sm3gm/black-market-passwords/main/images/wall-tag.png)

The game always knows the current password. This mod reads it and writes it down for you, so the tag stops being the only way in. It shows what the wall would show on a good day and nothing more.

## How to use it

1. Click into a sticky note on your case board so you are writing in it.
2. Press **F8**. One line is added per dealer, black market first. For example:

   ```
   Black market, Busy Building Supplies: Jasmine
   Weapons, Hornet Ironmongers: Buffalo
   ```

3. Go to the shop and give the dealer the password as usual.

The password is read fresh on every press. If it ever changes, press F8 again for the new one.

**Shift + F8** adds a time and place stamp instead: the in-game day and time, where you are standing, and who you are talking to, if anyone. Useful for case notes.

Both keys only work while a sticky note has focus. Anywhere else they do nothing, so they cannot clash with gameplay. On a Mac keyboard, press **Fn + F8**.

## Settings

The config file is `BepInEx/config/ta.sod.blackmarketpasswords.cfg`. Edit it with the game closed.

| Setting | Default | What it does |
|---|---|---|
| `PasswordKey` | `F8` | Key that writes the passwords |
| `PasswordModifier` | `None` | Key to hold with it. `None` means the key on its own |
| `StampKey` | `F8` | Key that writes the time and place stamp |
| `StampModifier` | `LeftShift` | Key to hold with it |

Left and right Shift, Ctrl and Alt count as the same key.

`Separator` (default `,`) goes between the parts of each line, such as the label and the shop name. Spaces are added automatically. The stamp's display options are in the same file, with a comment on each.

## Good to know

- **Nothing is saved.** The mod never touches your save file. The only thing it changes is the text of the note you pressed the key in, so you can add or remove it at any point in a playthrough.
- **You still do the talking.** The mod does not unlock anything or mark a password as known. You give the password to the dealer yourself.
- **A dealer who won't deal with you at all** is a separate matter. Knowing the password doesn't change that.
- **No other mods needed.** Only BepInEx. Tested next to SyncDiskSuite and a long list of other mods.

## Installing

**With Thunderstore Mod Manager or r2modman:** click *Install with Mod Manager* on this page. BepInEx comes with it.

**By hand:** install the BepInExPack IL2CPP for Shadows of Doubt first. Then open this mod's zip and copy its `BepInEx` folder into your game folder, the one with `Shadows of Doubt.exe` in it, merging with the `BepInEx` folder already there. The DLL should end up at `BepInEx/plugins/SoDBlackMarketPasswords/SoDBlackMarketPasswords.dll`.

## Source and bug reports

The source code is at [github.com/sm3gm/black-market-passwords](https://github.com/sm3gm/black-market-passwords). Bug reports and suggestions are welcome as GitHub issues.

## Also by sm3gm

[SyncDiskSuite](https://thunderstore.io/c/shadows-of-doubt/p/Sm3gm/SyncDiskSuite/): 12 new sync disks, 37 vanilla disk branches split into standalone disks, and configurable vendor stock.

## Support

This mod is free and always will be.

Likes are greatly appreciated and keep me motivated to release more mods, so if you enjoy it, please leave one. You give a like on the mod's [Thunderstore page](https://thunderstore.io/c/shadows-of-doubt/p/Sm3gm/BlackMarketPasswords/) while logged in to Thunderstore. If you installed through a mod manager, open that page in your browser to find the button.

And if you love what I do and want to support future projects, or just want to send a friendly thought, you can do so here:

[![Ko-fi](https://img.shields.io/badge/Ko--fi-sm3gm-FF5E5B?logo=kofi&logoColor=white)](https://ko-fi.com/sm3gm) [![Buy Me a Coffee](https://img.shields.io/badge/Buy%20Me%20a%20Coffee-sm3gm-FFDD00?logo=buymeacoffee&logoColor=black)](https://buymeacoffee.com/sm3gm)

Bug reports and ideas help as well. Post them as [GitHub issues](https://github.com/sm3gm/black-market-passwords/issues).

Thanks for playing.

## License

MIT.
