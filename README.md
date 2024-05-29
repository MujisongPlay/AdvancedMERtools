# How to Install AdvancedMERtools (AMERT)

![image](https://github.com/Maciupek/AdvancedMERtools/blob/main/AMERT.png?raw=true)

> [!IMPORTANT]  
> This plugin requires NaughtyAttributes, which can be installed from the Unity Asset Store: [NaughtyAttributes](https://assetstore.unity.com/packages/tools/utilities/naughtyattributes-129996)

## Installation Instructions
1. Install and import NaughtyAttributes from the link above
2. Download the necessary files from the specified folder above.
3. Drag and drop the files into the 'Project' tab in the Unity Editor. Preferably to `DONT TOUCH/Scripts`
4. In the release tab, you will find a plugin that facilitates the execution of functions and modeling for LCZ, HCZ, and EZ doors.

Additionally, download the SCPSLAudioAPI from the following link:
[SCPSLAudioAPI](https://github.com/CedModV2/SCPSLAudioApi)
Then, place the `AMERTAudioModule.dll` file in the `Roaming\SCPSL\PluginAPI\Global\Dependencies` directory.

## AdvancedMERtools Description
AdvancedMERtools offers advanced tools for the SCP: SL plugin called MapEditorReborn (MER). Please download the tools from the folder mentioned above and integrate them into your project. We plan to add more features in the future.

### Features
- **AutoScaler**: Scales grouped objects up or down automatically.
- **Rotator**: Rotates objects akin to the behavior of a magnetic field or sphere.
- **Mirror**: Creates a mirrored copy of objects.
- **Arrange**: Duplicates and arranges objects.
- **Health Object**: Adds a hitbox to your objects. **Requires additional plugin.**
- **Interactable Pickup**: Functions similarly to the Health Object. **Requires additional plugin.**
- **Custom Collider**: Adds custom collision properties to objects.
- **Interactable Teleporters**: 
- **Text**: Adds text to set empty object.
- **Custom RGBA**: Extends the minimum and maximum RGBA values beyond 0-255 to enhance object glow. **Requires additional plugin.**
- **Converter**: Converts Unity's primitives into MER's components if placed accidentally. Activating it will also automatically color your primitives based on the set material.

> [!NOTE]
> Applying the same script from parent to children objects is not recommended as it may lead to unexpected errors. Please use scripts individually.

## Tutorials
- Arrange Tutorial: [Watch Tutorial](https://youtu.be/adXuM0UINhE)
- Text Tutorial: [Watch Tutorial](https://youtu.be/UmkEbiVhDTE)

![image](https://github.com/MujisongPlay/AdvancedMERtools/assets/96275409/3249ec64-4bfc-4071-98fb-51d1052cc8e6)

## DummyDoorSpawner
The DummyDoorSpawner creates visible dummy doors to counteract the visibility issues caused by the culling system.

- Showcase Video: [Watch Showcase](https://youtu.be/TLkXputvKFc)
- Straightforward Installation Tutorial: [Watch Tutorial](https://youtu.be/-_IvE2kCHvU)
