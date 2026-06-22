# BPExplorerTracker
### An automatic tracker for Blue Prince Explorer% runs

Go to [Releases](https://github.com/AudioPixie/BPExplorerTracker/releases) to download the latest build

## Autotracking setup

**NOTE: Autotracking is currently incompatible with MacOSX**

All builds come bundled with a compatible MelonLoader mod for autotracking hooking. After installing MelonLoader, place ExplorerTrackerHook.dll in your mods folder. Assuming your filepath is the default one steam uses, this should be located at `C:/Program Files (x86)/Steam/steamapps/common/Blue Prince/Mods`.

In the tracker, the default filepath will point at `C:/Program Files (x86)/Steam/steamapps/common/Blue Prince` for the json files the mod creates.

[Mod Repo to view source code](https://github.com/AudioPixie/ExplorerTrackerHook)

## To uninstall

If you would like to uninstall the mod, run the MelonLoader setup app (the same one you used to install) and follow the uninstall prompts. The mod generates two json files that will still be present in your game directory - `drafts.json` and `events.json` - that must be deleted manually. Note that they are just text files that do nothing on their own and are harmless to leave there.

## Manual Control

With Automatic unchecked, the tracker can be used manually. Icons can be clicked to turn them on and off, and the Found Floorplans and Studio Additions pages can be right-clicked to toggle greyscale. There is a Reset Run button present for manual tracking. The chess icon can be left or right clicked to cycle through the pieces.
