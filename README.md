# Slipstream

## About
Slipstream is an in-dev mod that expands on RoR2 with stages, enemies, items, and survivors. (hopefully)

## Developing/Contributions
Slipstream is open source but please inform the main team of any contributions you plan to make. If you are familiar with contributing to Starstorm 2, the process of contribution will be very similar. In fact the "How to clone and develop" section is copied directly from Starstorm 2's github page.

## How to clone and develop

* You'll need:
    * Unity Hub
    * Unity version 2019.4.26f1
    * A GIT client (IE: GithubDesktop, Gitkraken, etc)

* Begin by cloning the repository to your hard drive
* Once the project downloads, open the `Slipstream` folder with Unity 2019.4.26f1, keep in mind that opening the project for the first time will take time, so patience is key
* During your importing or opening the project, or any other scenario, there is a chance you'll see this box pop up, Always hit "No Thanks", as hitting the other option WILL cause issues with specific assemblies.
* Once the project opens, you'll have a bunch of errors, these errors **are normal**, and are caused by missing ror2 assemblies. To fix this, Go to ``Assets/ThunderKitSettings`` folder, and look for the "ImportConfiguration" file.
    * If there are no Configuration Executors, delete the ImportConfiguration so Thunderkit can regenerate it.
    * If no configurations exist after this, please contact Jace and ask for help
* Once the import configurations are created, go to Tools/Thunderkit/Settings
* Select the Import Configurations, ensure that your configurations match the one from the image.
    * If you can't find one of the configurations, delete the import configuration.
* Make sure your import config matches the following:

| Importer Name | Enabled or Disabled | Extra Config |
|--|--|--|
| Check Unity Version | Enabled |  |
| Disable Assembly Updater | Enabled |  |
| PostProcessing Unity Package Installer | Enabled |  |
| TextMeshPro Uninstaller | Enabled |  |
| Unity GUI Uninstaller | Enabled |  |
| Wwise Blacklister | Disabled | At the moment we don't have WWise in our project |
| Assembly Publicizer | Enabled | Publicize at least RoR2.dll and KinematicCharacterController.dll |
| MMHook Generator | Enabled | Generate MMHook assemblies for at least RoR2.dll and KinematicCharacterController.dll |
| Import Assemblies | Enabled |  |
| RoR2 LegacyResourceAPI Patcher | Enabled |  |
| Import Project Settings | Enabled | Set the enum to Everything |
| Set Deferred Shading | Enabled |  |
| Create Game Package | Enabled |  |
| Import Addressable Catalog | Enabled |  |
| Configure Addressable Graphics Settings | Enabled |  |
| Ensure RoR2 Thunderstore Source | Enabled |  |
| Install BepInEx | Enabled |  |
| R2API Submodule Installer | Enabled | Hit disable all button, the Serialized Hard Dependencies will stay enabled |
| Install RoR2MultiplayerHLAPI | Disabled |  |
| Install RoR2EditorKit | Disabled |  |
| Install Risk of Options | Disabled | We don't have Risk of Options Support Right now |
| Get Bitness | Enabled |  |
| Beep | Enabled |  |
| Prompt Restart | Enabled |  |

* Select `ThunderKit Settings` and click Browse.
    * Find your RoR2 Executable, select it
    * Hit Import
* You're now ready to start development.
