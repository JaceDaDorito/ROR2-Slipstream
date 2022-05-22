### BepInEx Framework + API

This is the pack of all the things you need to both start using mods, and start making mods using the BepInEx framework.

To install, just extract the .zip so that the `winhttp.dll` file is sitting next to your `Risk of Rain 2.exe` file. That's the only installation step.
I'm aware of some issues related to logging, so I'll release a second version sometime later (and maybe with more included plugins).

### What each folder is for:
`BepInEx/plugins` - This is where normal mods/plugins are placed to be loaded.
For developers: There's no set format for what you need to name your plugins to load; if they're a valid plugin .dll file, they'll be loaded.
However please be considerate and isolate your files in their own folders, to prevent clutter, confusion, and in general, dependency hell. For example: `BepInEx/plugins/YourMod/Plugin.dll`

`BepInEx/patchers` - These are more advanced types of plugins that need to access Mono.Cecil to edit .dll files during runtime. Only copy paste your plugins here if the author tells you to.
For developers: More info here: <https://github.com/BepInEx/BepInEx/wiki/Writing-preloader-patchers>

`BepInEx/monomod` - MonoMod patches get placed in here. Only copy paste your plugins here if the author tells you to.

`BepInEx/config` - If your plugin has support for configuration, you can find the config file here to edit it.

`BepInEx/core` - Core BepInEx .dll files, you'll usually never want to touch these files (unless you're updating)


### What is included in this pack

**BepInEx 5.0** - <https://github.com/BepInEx/BepInEx>
This is what loads all of your plugins/mods. Read here for more information if you are a mod maker: <https://github.com/BepInEx/BepInEx/wiki>

**BepInEx.MonoMod.Loader** - <https://github.com/BepInEx/BepInEx.MonoMod.Loader>
Loads MonoMod patchers from `BepInEx/monomod`, read the monomod documentation for more info

**MonoMod.RuntimeDetour** - <https://github.com/0x0ade/MonoMod>
This is the library used to hook into and patch existing methods at runtime, greatly increasing what can be done within a mod.
For more info: <https://github.com/0x0ade/MonoMod/blob/master/README-RuntimeDetour.md>
The appropriate MMHOOK_Assembly-CSharp.dll is included in this pack too, so you don't need to make your own (until the game updates, at least)