using MelonLoader;
using Rock_Cam_Essentials;
using BuildInfo = Rock_Cam_Essentials.BuildInfo;

[assembly: MelonInfo(typeof(Rock_Cam_Essentials.Main), BuildInfo.ModName, BuildInfo.ModVersion, BuildInfo.Author)]
[assembly: MelonGame("Buckethead Entertainment", "RUMBLE")]
[assembly: MelonColor(255, 200, 0, 200)]
[assembly: MelonAuthorColor(255, 255, 119, 255)]
[assembly: VerifyLoaderVersion(0, 6, 6, true)]

