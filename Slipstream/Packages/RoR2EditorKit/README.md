# RoR2EditorKit

The Risk of Rain 2 Editor Kit is a Thunderkit Extension designed specifically for helping mod creators create content for Risk of Rain 2.

Our main goal is to bring a friendly, easy to use editor experience for creating content, ranging from items, all the way to ideally bodies.

Features:

## Inspectors

RoR2EditorKit comes bundled with custom Inspectors that overwrite the default view of certain Scriptable Objects in RoR2, specifically annoying to work with ones, either with new easier to use inspectors, or editor windows that break down the default inspector for a more viewable experience. Examples of these include:

* Serializable Content Pack: Simply click one of the buttons and the inspector will show it's corresponding list. allowing for easy managment of the content of your mod
![](https://i.gyazo.com/7d9a746fe9386cfe68f1c1a0d2a44c78.png)

* Entity State Configuration: Easily select an entity state from the target type, when selected, the inspector will automatically populate the serialized fields array with the necesary fields to serialize.

![](https://i.gyazo.com/bb05950708255bbb39c7efb923adea4f.png)

## Property Drawers

RoR2EditorKit comes with custom property drawers for handling certain types inside Risk of Rain 2, the main example is our SerializableEntityStateType drawer. which allows you to easily search, find and select an entity state for your skill def or entity state machine.

![](https://cdn.discordapp.com/attachments/575431803523956746/903754837940916234/unknown.png)

## Asset Creator Windows

RoR2EditorKit comes with special editor windows designed specifically for creating Assets for Risk of Rain 2, so far it only comes bundled with editor windows for creating scriptable objects. but we plan on adding more and even complex ones for creating projectiles, or maybe even full body boilerplates.

* ItemDef: Easily create an item def by only giving the name, tier, and tags. you can also automatically create pickup and display prefabs with the correct needed components and proper naming scheme of HopooGames. You can specify more things by clicking the extra settings or prefab settings buttons.

* EquipmentDef: Create an equipment by simply giving it a name, cooldown, and wether or not its a lunar equipment/engima compatible. Like the itemDef window, it can also automatically create display and pickup prefabs. if you need more specification, you can click the extra settings or prefab settings buttons.

* ArtifactDef: Create an artifact quickly by just giving it a name, thats it. it'll also create a pickup prefab if desired.

* Sha256HashAsset: Create a hash asset for use in R2API's ArtifactCodeAPI. simply input the code in the 3 vector3Int fields and hit create. RoR2EditorKit will automatically create a hash asset with the correct hash values.

![](https://cdn.discordapp.com/attachments/567852222419828736/903719556894326785/a10578cadaeaa9ad1fbaedbfb8a158b2.png)

## Other:

* UnlockableDef script: Fixes a bug in the base game where unlockableDefs can't be created in the createAsset menu.

## Credits

* Coding: Nebby, Passive Picasso (Twiner), KingEnderBrine, Kevin from HP Customer Service
* Models & Sprite: Nebby
* Mod Icon: SOM

## Changelog

### 0.1.4

* Separated the Enabled and Disabled inspector settings to its own setting file. allowing projects to git ignore it.
* The Toggle for enabling and disabling the inspector is now on its header GUI for a more pleasant experience.

### 0.1.2

* Fixed no assembly definition being packaged with the toolkit, whoops.

### 0.1.1

- RoR2EditorKitSettings:
    * Removed the "EditorWindowsEnabled" setting.
    * Added an EnabledInspectors setting.
        * Lets the user choose what inspectors to enable/disable.
    * Added a MainManifest setting.
        * Lets RoR2EditorKit know the main manifest it'll work off, used in the SerializableContentPackWindow.

- Inspectors:
    * Added InspectorSetting property
        * Automatically Gets the inspector's settings, or creates one if none are found.
    * Inspectors can now be toggled on or off at the top of the inspector window.
    
- Editor Windows: 
    * Cleaned up and documented the Extended Editor Window class.
    * Updated the SerializableContentPack editor window:
        * Restored function for Drag and Dropping multiple files
        * Added a button to each array to auto-populate the arrays using the main manifest of the project.

### 0.1.0

- Reorganized CreateAsset Menu
- Added EntityStateConfiguration creator, select state type and hit create. Optional checkbox for setting the asset name to the state's name.
- Added SurvivorDef creator, currently halfway implemented.
- Added BuffDef creator, can automatically create a networked sound event for the start sfx.
- Removed EntityStateConfiguration editor window.
- Implemented a new EntityStateConfiguration inspector
- Internal Changes

### 0.0.1

- Initial Release