using Il2CppLiv.Lck;
using Il2CppLiv.Lck.Recorder;
using Il2CppLiv.Lck.UI;
using Il2CppRUMBLE.Managers;
using Il2CppRUMBLE.Players.Subsystems;
using Il2CppRUMBLE.Recording.LCK;
using MelonLoader;
using RumbleModdingAPI;
//using RumbleModUI;
using UnityEngine;
namespace Rock_Cam_Essentials
{
    public static class BuildInfo
    {
        public const string ModName = "RockCamEssentials";
        public const string ModVersion = "1.5.3";
        public const string Author = "Deterraleon";
    }

    public class Main : MelonMod
    {
        private string currentScene = "Loader";
        //private Mod Rock_Cam_Essentials = new Mod();
        private bool enabled = true;
        //List<ModSetting> settings = new List<ModSetting>();

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            currentScene = sceneName;
        }

        public override void OnLateInitializeMelon()
        {
            Calls.onMapInitialized += mapLoaded;
            //UI.instance.UI_Initialized += UIInit;
        }

        //private void UIInit()
        //{
        //    Rock_Cam_Essentials.ModName = BuildInfo.ModName;
        //    Rock_Cam_Essentials.ModVersion = BuildInfo.ModVersion;
        //    Rock_Cam_Essentials.SetFolder(BuildInfo.ModName);
        //    settings.Add(Rock_Cam_Essentials.AddToList("Enabled", false, 0, "Toggles Mod On/Off", new Tags { }));
        //    settings.Add(Rock_Cam_Essentials.AddToList("Detached Preview", 0, "Changes the detached preview setting", new Tags { }));
        //    Rock_Cam_Essentials.GetFromFile();
        //    Rock_Cam_Essentials.ModSaved += Save;
        //    UI.instance.AddMod(Rock_Cam_Essentials);
        //}

        //public void Save()
        //{
        //    enabled = (bool)settings[0].SavedValue;
        //}

        public void mapLoaded()
        {
            
        }
    }
    //A unility that allows for easier modding of the rockcam
    //The functions of the main class the actions you can do with the rockcam, while subclasses are mainly used for settings
    public class Rock_Cam
    {
        public int FOV = 50;
        public ThirdPerson thirdPerson;
        public FirstPerson firstPerson;
        public Handheld handheld;
        public string POV = "HH";
        public bool isHorizontal = true;
        public int PhotoTimer = 0;
        public int PhotoTimerIncrement = 2;
        public int PhotoTimerMaxValue = 8;
        public bool isRecording = false;
        public int DetachedPreview = 0;
        public bool DetachedPreviewChanged = false;
        public int MaxDespawnDistance = 2;
        public float SpawnYOffset = -0.1f;
        public float TabletSpawnDelay = 0.3f;
        public float MaximumRenderDistance = 2;
        public PlayerLIV _Camera;
        public LCKTabletUtility _Tablet;
        public Il2CppRUMBLE.Recording.LCK.Extensions.LCKCameraController _CameraController;
        public Il2CppRUMBLE.Recording.LCK.Extensions.LCKSettingsButtonsController _POVController;
        public Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppReferenceArray<Transform> DetachedMonitors;
        public Transform RockCamTransform;
        public LckService _LckService;
        public int isShown = 1;
        public bool POVChanged = false;
        public LCKTabletDetachedPreview _DetachedPreviewManager;
        public Il2CppTMPro.TMP_Text Nameplate;
        public LckQualitySelector _QualitySelector;
        public struct POVNames
        {
            public string ThirdPerson, FirstPerson, Handheld;
        }
        public POVNames POVs;
        public struct RecordingSettings
        {
            public uint width, height, audioBitrate, Bitrate, framerate;
            public static bool operator ==(RecordingSettings a, RecordingSettings b)
            {
                return a.width == b.width && a.height == b.height && a.audioBitrate == b.audioBitrate && a.Bitrate == b.Bitrate && a.framerate == b.framerate;
            }
            public static bool operator !=(RecordingSettings a, RecordingSettings b)
            {
                return !(a == b);
            }
        }
        public Rock_Cam()
        {
            POVs.ThirdPerson = "TP";
            POVs.FirstPerson = "FP";
            POVs.Handheld = "HH";
            Fix();
        }
        //Everything related to the third person camera
        public class ThirdPerson
        {
            public Il2CppRUMBLE.Recording.LCK.Extensions.LCKCameraController _CameraController;
            public Il2CppRUMBLE.Recording.LCK.Extensions.LckDoubleButton _FOVSetter;
            public int MaxFOV = 100;
            public int MinFOV = 30;
            public int FOVStep = 10;
            public bool isFlipped = false;
            public float Distance = 1;
            public float Angle = 15;
            public UnityEngine.Rendering.Universal.UniversalAdditionalCameraData _CameraSettings;
            public LckCamera _Camera;
            public float PositionalSmoothing = 0;
            public float RotationalSmoothing = 0;
            public Il2CppRUMBLE.Recording.LCK.Extensions.LCKSettingsButtonsController _POVController;
            public Rock_Cam _rockcam;
            public ThirdPerson(Rock_Cam rockcam)
            {
                _rockcam = rockcam;
                _CameraController = rockcam._CameraController;
                _Camera = rockcam._Tablet.thirdPersonCamera;
                Distance = _CameraController._thirdPersonDistance;
                Angle = _CameraController._thirdPersonHeightAngle;
                _FOVSetter = _CameraController.ThirdPersonFOVDoubleButton;
                MaxFOV = _FOVSetter._maxValue;
                MinFOV = _FOVSetter._minValue;
                FOVStep = _FOVSetter._increment;
                PositionalSmoothing = _CameraController._thirdPersonStabilizer.PositionalSmoothing;
                RotationalSmoothing = _CameraController._thirdPersonStabilizer.RotationalSmoothing;
                _CameraSettings = rockcam._Tablet.thirdPersonCamera.gameObject.GetComponent<UnityEngine.Rendering.Universal.UniversalAdditionalCameraData>();
                _POVController = rockcam._POVController;

            }
            //Sets the POV to third person mode
            public bool SetPOV()
            {
                try
                {
                    _POVController.SwitchToTPMode();
                    _rockcam.POV = "TP";
                }
                catch (Exception ex)
                {
                    MelonLogger.Error(ex);
                    return false;
                }
                return true;
            }
            //Sets the angle at which the third person camera follows the player
            public bool SetAngle(float angle)
            {
                try
                {
                    _CameraController._thirdPersonHeightAngle = angle;
                    Angle = angle;
                    return true;
                }
                catch (Exception ex)
                {
                    MelonLogger.Error(ex);
                    return false;
                }
            }
            //Sets the distance at which the third person camera follows the player, this value is the one that gets changed via the ingame buttons
            //Also gets multiplied by the third person distance multiplier
            public bool SetDistance(float distance)
            {
                try
                {
                    _CameraController._thirdPersonDistance = distance;
                    Distance = distance;
                    return true;
                }
                catch (Exception ex)
                {
                    MelonLogger.Error(ex);
                    return false;
                }
            }
            //Sets the distance multiplier that is multiplier, that is multiplied by the distance to get the actual distance at which the camera will follow the player
            public bool SetDistanceMultiplier(float distanceMultiplier)
            {
                try
                {
                    _CameraController._thirdPersonDistanceMultiplier = distanceMultiplier;
                    return true;
                }
                catch (Exception ex)
                {
                    MelonLogger.Error(ex);
                    return false;
                }
            }
            //Flips the third person camera, as of now very buggy
            public bool FlipCamera(bool flip)
            {
                if (isFlipped != _CameraController.IsThirdPersonFront)
                {
                    MelonLogger.Msg("isFlippedTP variable desynched");
                    isFlipped = _CameraController.IsThirdPersonFront;
                }
                if (flip == isFlipped)
                {
                    MelonLogger.Msg("TP camera already flipped correct way");
                    return true;
                }
                try
                {
                    isFlipped = flip;
                    _CameraController.IsThirdPersonFront = !_CameraController.IsThirdPersonFront;
                    return true;
                }
                catch (Exception ex)
                {
                    MelonLogger.Error(ex);
                    return false;
                }
            }
            //Sets the positional smoothing of the third person camera
            public bool SetPositionalSmooting(float smooth)
            {
                if (smooth >= 1 && smooth != 99)
                {
                    MelonLogger.Warning("Smoothing of 1 or greater will result in no movement at all, you can set smoothing to 99 to avoid this message");
                }
                try
                {
                    _CameraController._thirdPersonStabilizer.PositionalSmoothing = smooth;
                    PositionalSmoothing = smooth;
                }
                catch (Exception ex)
                {
                    MelonLogger.Error(ex);
                    return false;
                }
                return true;
            }
            //Sets the rotational smoothing of the third person camera
            public bool SetRotationalSmooting(float smooth)
            {
                if (smooth >= 1 && smooth != 99)
                {
                    MelonLogger.Warning("Smoothing of 1 or greater will result in no movement at all, you can set smoothing to 99 to avoid this message");
                }
                try
                {
                    _CameraController._thirdPersonStabilizer.RotationalSmoothing = smooth;
                    RotationalSmoothing = smooth;
                }
                catch (Exception ex)
                {
                    MelonLogger.Error(ex);
                    return false;
                }
                return true;
            }
            //Sets the FOV of the third person camera, written in a way where it will also update the ingame values shown
            public bool SetFOV(int fov)
            {
                if (_FOVSetter._currentValue != _rockcam.FOV)
                {
                    MelonLogger.Msg("FOV value desynched");
                    _rockcam.FOV = _FOVSetter._currentValue;
                }
                try
                {
                    _FOVSetter._maxValue = 999999;
                    _FOVSetter._increment = fov - _rockcam.FOV;
                    _FOVSetter.OnPressDownIncrease();
                    _FOVSetter.OnPressUpIncrease();
                    _FOVSetter._maxValue = MaxFOV;
                    _FOVSetter._increment = FOVStep;
                    _rockcam.FOV = fov;
                    if (_rockcam.FOV != _FOVSetter._currentValue)
                    {
                        MelonLogger.Msg(_rockcam.FOV.ToString() + " " + _FOVSetter._currentValue.ToString());
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    MelonLogger.Error(ex);
                    return false;
                }
            }
            //Sets the settings settings, aka the bounds for the FOV setting ingame, for the third person camera
            public bool SetFOVSettingsSettings(int maxFOV, int minFOV, int step)
            {
                try
                {
                    _FOVSetter._maxValue = maxFOV;
                    _FOVSetter._increment = step;
                    _FOVSetter._minValue = minFOV;
                    MaxFOV = maxFOV;
                    MinFOV = minFOV;
                    FOVStep = step;
                    return true;
                }
                catch (Exception ex)
                {
                    MelonLogger.Error(ex);
                    return false;
                }
            }
            public bool DoPostProcessing(bool value)
            {
                try
                {
                    _CameraSettings.renderPostProcessing = value;
                    return true;
                }
                catch (Exception ex)
                {
                    MelonLogger.Error(ex);
                    return false;
                }
            }
        }
        //Everything related to the first person camera
        public class FirstPerson
        {
            public Il2CppRUMBLE.Recording.LCK.Extensions.LCKCameraController _CameraController;
            public Il2CppRUMBLE.Recording.LCK.Extensions.LckDoubleButton _FOVSetter;
            public int MaxFOV = 100;
            public int MinFOV = 30;
            public int FOVStep = 10;
            public UnityEngine.Rendering.Universal.UniversalAdditionalCameraData _CameraSettings;
            public LckCamera _Camera;
            public float PositionalSmoothing = 0;
            public float RotationalSmoothing = 0;
            public Il2CppRUMBLE.Recording.LCK.Extensions.LCKSettingsButtonsController _POVController;
            public Rock_Cam _rockcam;
            public FirstPerson(Rock_Cam rockcam)
            {
                _rockcam = rockcam;
                _CameraController = rockcam._CameraController;
                _Camera = rockcam._Tablet.firstPersonCamera;
                _FOVSetter = _CameraController.FirstPersonFOVDoubleButton;
                MaxFOV = _FOVSetter._maxValue;
                MinFOV = _FOVSetter._minValue;
                FOVStep = _FOVSetter._increment;
                PositionalSmoothing = _CameraController._firstPersonStabilizer.PositionalSmoothing;
                RotationalSmoothing = _CameraController._firstPersonStabilizer.RotationalSmoothing;
                _CameraSettings = rockcam._Tablet.firstPersonCamera.gameObject.GetComponent<UnityEngine.Rendering.Universal.UniversalAdditionalCameraData>();
                _POVController = rockcam._POVController;
            }
            //Sets the POV to first person mode
            public bool SetPOV()
            {
                try
                {
                    _POVController.SwitchToFPMode();
                    _rockcam.POV = "FP";
                }
                catch (Exception ex)
                {
                    MelonLogger.Error(ex);
                    return false;
                }
                return true;
            }
            //Sets the positional smoothing of the first person camera
            public bool SetPositionalSmooting(float smooth)
            {
                if (smooth >= 1 && smooth != 99)
                {
                    MelonLogger.Warning("Smoothing of 1 or greater will result in no movement at all, you can set smoothing to 99 to avoid this message");
                }
                try
                {
                    _CameraController._firstPersonStabilizer.PositionalSmoothing = smooth;
                    PositionalSmoothing = smooth;
                }
                catch (Exception ex)
                {
                    MelonLogger.Error(ex);
                    return false;
                }
                return true;
            }
            //Sets the rotational smoothing of the first person camera
            public bool SetRotationalSmooting(float smooth)
            {
                if (smooth >= 1 && smooth != 99)
                {
                    MelonLogger.Warning("Smoothing of 1 or greater will result in no movement at all, you can set smoothing to 99 to avoid this message");
                }
                try
                {
                    _CameraController._firstPersonStabilizer.RotationalSmoothing = smooth;
                    RotationalSmoothing = smooth;
                }
                catch (Exception ex)
                {
                    MelonLogger.Error(ex);
                    return false;
                }
                return true;
            }
            //Sets the FOV of the first person camera, written in a way where it will also update the ingame values shown
            public bool SetFOV(int fov)
            {
                if (_FOVSetter._currentValue != _rockcam.FOV)
                {
                    MelonLogger.Msg("FOV value desynched");
                    _rockcam.FOV = _FOVSetter._currentValue;
                }
                try
                {
                    _FOVSetter._maxValue = 999999;
                    _FOVSetter._increment = fov - _rockcam.FOV;
                    _FOVSetter.OnPressDownIncrease();
                    _FOVSetter.OnPressUpIncrease();
                    _FOVSetter._maxValue = MaxFOV;
                    _FOVSetter._increment = FOVStep;
                    _rockcam.FOV = fov;
                    if (_rockcam.FOV != _FOVSetter._currentValue)
                    {
                        MelonLogger.Msg(_rockcam.FOV.ToString() + " " + _FOVSetter._currentValue.ToString());
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    MelonLogger.Error(ex);
                    return false;
                }
            }
            //Sets the settings settings, aka the bounds for the FOV setting ingame, for the first person camera
            public bool SetFOVSettingsSettings(int maxFOV, int minFOV, int step)
            {
                try
                {
                    _FOVSetter._maxValue = maxFOV;
                    _FOVSetter._increment = step;
                    _FOVSetter._minValue = minFOV;
                    MaxFOV = maxFOV;
                    MinFOV = minFOV;
                    FOVStep = step;
                    return true;
                }
                catch (Exception ex)
                {
                    MelonLogger.Error(ex);
                    return false;
                }
            }
            public bool DoPostProcessing(bool value)
            {
                try
                {
                    _CameraSettings.renderPostProcessing = value;
                    return true;
                }
                catch (Exception ex)
                {
                    MelonLogger.Error(ex);
                    return false;
                }
            }
        }
        //Everything related to the handheld(selfie) camera
        public class Handheld
        {
            public Il2CppRUMBLE.Recording.LCK.Extensions.LCKCameraController _CameraController;
            public Il2CppRUMBLE.Recording.LCK.Extensions.LckDoubleButton _FOVSetter;
            public int MaxFOV = 100;
            public int MinFOV = 30;
            public int FOVStep = 10;
            public UnityEngine.Rendering.Universal.UniversalAdditionalCameraData _CameraSettings;
            public LckCamera _Camera;
            public float PositionalSmoothing = 0;
            public float RotationalSmoothing = 0;
            public Il2CppRUMBLE.Recording.LCK.Extensions.LCKSettingsButtonsController _POVController;
            public Rock_Cam _rockcam;
            public bool isFlipped = false;
            public Handheld(Rock_Cam rockcam)
            {
                _rockcam = rockcam;
                _CameraController = rockcam._CameraController;
                _Camera = rockcam._Tablet.selfieCamera;
                _FOVSetter = _CameraController.SelfieFOVDoubleButton;
                MaxFOV = _FOVSetter._maxValue;
                MinFOV = _FOVSetter._minValue;
                FOVStep = _FOVSetter._increment;
                PositionalSmoothing = _CameraController._selfieStabilizer.PositionalSmoothing;
                RotationalSmoothing = _CameraController._selfieStabilizer.RotationalSmoothing;
                _CameraSettings = rockcam._Tablet.selfieCamera.gameObject.GetComponent<UnityEngine.Rendering.Universal.UniversalAdditionalCameraData>();
                _POVController = rockcam._POVController;
            }
            //Sets the POV to handheld mode
            public bool SetPOV()
            {
                try
                {
                    _POVController.SwitchToSelfieMode();
                    _rockcam.POV = "HH";
                }
                catch (Exception ex)
                {
                    MelonLogger.Error(ex);
                    return false;
                }
                return true;
            }
            //Sets the positional smoothing of the handheld camera
            public bool SetPositionalSmooting(float smooth)
            {
                if (smooth >= 1 && smooth != 99)
                {
                    MelonLogger.Warning("Smoothing of 1 or greater will result in no movement at all, you can set smoothing to 99 to avoid this message");
                }
                try
                {
                    _CameraController._selfieStabilizer.PositionalSmoothing = smooth;
                    PositionalSmoothing = smooth;
                }
                catch (Exception ex)
                {
                    MelonLogger.Error(ex);
                    return false;
                }
                return true;
            }
            //Sets the rotational smoothing of the handheld camera
            public bool SetRotationalSmooting(float smooth)
            {
                if (smooth >= 1 && smooth != 99)
                {
                    MelonLogger.Warning("Smoothing of 1 or greater will result in no movement at all, you can set smoothing to 99 to avoid this message");
                }
                try
                {
                    _CameraController._selfieStabilizer.RotationalSmoothing = smooth;
                    RotationalSmoothing = smooth;
                }
                catch (Exception ex)
                {
                    MelonLogger.Error(ex);
                    return false;
                }
                return true;
            }
            //Sets the FOV of the handheld camera, written in a way where it will also update the ingame values shown
            public bool SetFOV(int fov)
            {
                if (_FOVSetter._currentValue != _rockcam.FOV)
                {
                    MelonLogger.Msg("FOV value desynched");
                    _rockcam.FOV = _FOVSetter._currentValue;
                }
                try
                {
                    _FOVSetter._maxValue = 999999;
                    _FOVSetter._increment = fov - _rockcam.FOV;
                    _FOVSetter.OnPressDownIncrease();
                    _FOVSetter.OnPressUpIncrease();
                    _FOVSetter._maxValue = MaxFOV;
                    _FOVSetter._increment = FOVStep;
                    _rockcam.FOV = fov;
                    if (_rockcam.FOV != _FOVSetter._currentValue)
                    {
                        MelonLogger.Msg(_rockcam.FOV.ToString() + " " + _FOVSetter._currentValue.ToString());
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    MelonLogger.Error(ex);
                    return false;
                }
            }
            //Sets the settings settings, aka the bounds for the FOV setting ingame, for the handheld camera
            public bool SetFOVSettingsSettings(int maxFOV, int minFOV, int step)
            {
                try
                {
                    _FOVSetter._maxValue = maxFOV;
                    _FOVSetter._increment = step;
                    _FOVSetter._minValue = minFOV;
                    MaxFOV = maxFOV;
                    MinFOV = minFOV;
                    FOVStep = step;
                    return true;
                }
                catch (Exception ex)
                {
                    MelonLogger.Error(ex);
                    return false;
                }
            }
            //Flips the handheld camera, very buggy
            public bool FlipCamera(bool flip)
            {
                if (isFlipped == flip)
                {
                    MelonLogger.Msg("HH camera already flipped the right way");
                    return true;
                }
                try
                {
                    _CameraController.ProcessSelfieFlip();
                    isFlipped = flip;
                    return true;
                }
                catch (Exception ex)
                {
                    MelonLogger.Error(ex);
                    return false;
                }
            }
            //Sets the position of the handheld camera relative to the rockcam
            public bool SetRelativeCameraPosition(Vector3 position, Vector3 rotation)
            {
                try
                {
                    _CameraController.SetSelfieCameraOrientation(position, rotation);
                    return true;
                }
                catch (Exception ex)
                {
                    MelonLogger.Error(ex);
                    return false;
                }
            }
            //Sets the position of the handheld camera in global coords
            public bool SetGlobalCameraPosition(Vector3 position, Quaternion rotation)
            {
                try
                {
                    var position2 = _rockcam.RockCamTransform.InverseTransformPoint(position);
                    Quaternion rotation2 = Quaternion.Inverse(_rockcam.RockCamTransform.rotation) * rotation;
                    _CameraController.SetSelfieCameraOrientation(position2, rotation2.eulerAngles);
                    return true;
                }
                catch (Exception ex)
                {
                    MelonLogger.Error(ex);
                    return false;
                }
            }
            public bool DoPostProcessing(bool value)
            {
                try
                {
                    _CameraSettings.renderPostProcessing = value;
                    return true;
                }
                catch (Exception ex)
                {
                    MelonLogger.Error(ex);
                    return false;
                }
            }
        }
        //Basically just redefines every variable
        public bool Fix()
        {
            try
            {
                _Camera = PlayerManager.instance.localPlayer.Controller.GetComponentInChildren<PlayerLIV>();
                MaxDespawnDistance = _Camera.maxDespawnDistance;
                SpawnYOffset = _Camera.spawnYOffset;
                TabletSpawnDelay = _Camera.tabletSpawnDelay;
                _Tablet = _Camera.LckTablet;
                MaximumRenderDistance = _Tablet.maximumRenderDistance;
                isRecording = _Tablet.isRecording;
                _CameraController = _Tablet.LckCameraController;
                isHorizontal = _CameraController.IsHorizontalMode;
                _POVController = _CameraController._settingsButtonsController;
                thirdPerson = new ThirdPerson(this);
                firstPerson = new FirstPerson(this);
                handheld = new Handheld(this);
                DetachedMonitors = _CameraController._monitorTransforms;
                FOV = thirdPerson._FOVSetter._currentValue;
                PhotoTimer = _Tablet.photoTimerCurrentValue;
                PhotoTimerIncrement = _Tablet.photoTimerIncrementValue;
                PhotoTimerMaxValue = _Tablet.photoTimerMaxValue;
                RockCamTransform = _Tablet.transform;
                if (_Camera.TabletVisualsAreActive) isShown = 1;
                else isShown = 0;
                POVUpdate();
                POVChanged = false;
                _LckService = _Camera.lckService;
                _DetachedPreviewManager = _Tablet.lckDetachedPreview;
                Nameplate = _Tablet.gameObject.GetComponent<LCKTabletNameplate>().nameplateTextComponent.textObject;
                _QualitySelector = _CameraController._qualitySelector;
                return true;
            }
            catch (Exception ex)
            {
                MelonLogger.Error(ex);
                return false;
            }
        } 
        //Takes a photo
        public bool TakePhoto()
        {
            try
            {
                _Tablet.TakePhoto();
            }
            catch (Exception ex)
            {
                MelonLogger.Error(ex);
                return false;
            }
            return true;
        }
        //Hides the tablet, same function used when trying to summon the table while being close to it
        public bool HideTablet()
        {
            try
            {
                _Camera.HideTablet();
            }
            catch (Exception ex)
            {
                MelonLogger.Error(ex);
                return false;
            }
            return true;
        }
        //Shows the tablet, same function used when trying to summon it regularly
        //During the spawn delay the tablet is not active, which could cause issues when trying to modify its position continuously, where it kinda just freezes for a moment.
        public bool ShowTablet()
        {
            try
            {
                _Camera.ShowTablet();
            }
            catch (Exception ex)
            {
                MelonLogger.Error(ex);
                return false;
            }
            return true;
        }
        //Starts the recording, won't do anything if already recording
        public bool StartRecording()
        {
            if (isRecording != _Camera.LckTablet.isRecording)
            {
                MelonLogger.Msg("isRecording variable is desynched");
                isRecording = _Camera.LckTablet.isRecording;
            }
            if (!isRecording)
            {
                isRecording = true;
                try
                {
                    _Tablet.ToggleRecordingAndUpdateRecordStatus();
                }
                catch (Exception ex)
                {
                    MelonLogger.Error(ex);
                    return false;
                }
            }
            else
            {
                MelonLogger.Msg("Already recording");
                return true;
            }
            return true;
        }
        //Ends the recording, won't do anything if already not recording
        public bool EndRecording()
        {
            if (isRecording != _Camera.LckTablet.isRecording)
            {
                MelonLogger.Msg("isRecording variable is desynched");
                isRecording = _Camera.LckTablet.isRecording;
            }
            if (isRecording)
            {
                isRecording = false;
                try
                {
                    _Tablet.ToggleRecordingAndUpdateRecordStatus();
                }
                catch (Exception ex)
                {
                    MelonLogger.Error(ex);
                    return false;
                }
            }
            return true;
        }
        //Sets the rotational smoothing for all cameras
        public bool SetGlobalRotationalSmoothing(float smooth)
        {
            return thirdPerson.SetRotationalSmooting(smooth) && firstPerson.SetRotationalSmooting(smooth) && handheld.SetRotationalSmooting(smooth);
        }
        //Sets the positional smoothing for all cameras
        public bool SetGlobalPositionalSmoothing(float smooth)
        {
            return thirdPerson.SetPositionalSmooting(smooth) && firstPerson.SetPositionalSmooting(smooth) && handheld.SetPositionalSmooting(smooth);
        }
        //Makes the camera instantly reach its destination ignoring smoothing, no idea why i wrote the same function twice lmao
        public bool ResetCameraPosition()
        {
            try
            {
                if (POV == "TP")
                {
                    _CameraController._thirdPersonStabilizer.ReachTargetInstantly();
                }
                if (POV == "FP")
                {
                    _CameraController._firstPersonStabilizer.ReachTargetInstantly();
                }
                if (POV == "HH")
                {
                    _CameraController._selfieStabilizer.ReachTargetInstantly();
                }
            }
            catch (Exception ex)
            {
                MelonLogger.Error(ex);
                return false;
            }
            return true;
        }
        //Sets the distance at which the camera screen will update the camera image
        public bool SetMaximumRenderDistance(float distance)
        {
            try
            {
                _Tablet.maximumRenderDistance = distance;
                MaximumRenderDistance = distance;
            }
            catch (Exception ex)
            {
                MelonLogger.Error(ex);
                return false;
            }
            return true;
        }
        public bool SetDetachedPreview(int index)
        {
            try
            {
                _Tablet.lckDetachedPreview.SwitchPreview(index);
                UpdateDetachedPreview();
            }
            catch (Exception ex)
            {
                MelonLogger.Error(ex);
                return false;
            }
            return true;
        }
        public bool SetPhotoTimerValue(int time)
        {
            try
            {
                _Tablet.photoTimerMaxValue = 999999;
                _Tablet.photoTimerIncrementValue = time - PhotoTimer;
                _Tablet.SwapPhotoTimerDelay();
                _Tablet.photoTimerMaxValue = PhotoTimerMaxValue;
                _Tablet.photoTimerIncrementValue = PhotoTimerIncrement;
                return true;
            }
            catch (Exception ex)
            {
                MelonLogger.Error(ex);
                return false;
            }
        }
        //Sets the increment by which it will increase the photo timer upon pressing the ingame button
        public bool SetPhotoTimerIncrement(int increment)
        {
            try
            {
                _Tablet.photoTimerIncrementValue = increment;
                PhotoTimerIncrement = increment;
                return true;
            }
            catch (Exception ex)
            {
                MelonLogger.Error(ex);
                return false;
            }
        }
        //Sets the max value of the ingame photo timer, upon exceeding the max value the photo timer will drop to 0.
        public bool SetPhotoTimerMaxValue(int maxValue)
        {
            try
            {
                _Tablet.photoTimerMaxValue = maxValue;
                PhotoTimerMaxValue = maxValue;
                return true;
            }
            catch (Exception ex)
            {
                MelonLogger.Error(ex);
                return false;
            }
        }
        //Literally just changes the transform of the rockcam object
        public bool SetRockCamPosition(Vector3 position, Quaternion rotation)
        {
            try
            {
                _Tablet.transform.position = position;
                _Tablet.transform.rotation = rotation;
                return true;
            }
            catch (Exception ex)
            {
                MelonLogger.Error(ex);
                return false;
            }
        }
        //Set the y offset upon spawn
        public bool SetSpawnYOffset(float offset)
        {
            try
            {
                _Camera.spawnYOffset = offset;
                SpawnYOffset = offset;
                return true;
            }
            catch (Exception ex)
            {
                MelonLogger.Error(ex);
                return false;
            }
        }
        //Set for how long the camera isn't active upon spawning
        public bool SetRockCamSpawnDelay(float delay)
        {
            try
            {
                _Camera.tabletSpawnDelay = delay;
                TabletSpawnDelay = delay;
                return true;
            }
            catch (Exception ex)
            {
                MelonLogger.Error(ex);
                return false;
            }
        }
        //Makes the camera instantly reach its destination ignoring smoothing, no idea why i wrote the same function twice lmao
        public bool SnapCameraStabilizerToTarget()
        {
            try
            {
                _CameraController._thirdPersonStabilizer.ReachTargetInstantly();
                _CameraController._firstPersonStabilizer.ReachTargetInstantly();
                _CameraController._selfieStabilizer.ReachTargetInstantly();
                return true;
            }
            catch (Exception ex)
            {
                MelonLogger.Error(ex);
                return false;
            }
        }
        //A macros to just set a bunch of variables at once
        public bool FullCameraSetup(string pov = "Null", float positional_smoothing = -1, float rotational_smoothing = -1, int fov = -1,
            float TPCameraDistance = -1, float TPCameraAngle = -1)
        {
            try
            {
                if (pov == "Null")
                {
                    pov = POV;
                }
                if (fov == -1)
                {
                    fov = FOV;
                }
                if (pov == "TP")
                {
                    thirdPerson.SetPOV();
                    thirdPerson.SetFOV(fov);
                    if (TPCameraDistance == -1)
                    {
                        TPCameraDistance = thirdPerson.Distance;
                    }
                    thirdPerson.SetDistance(TPCameraDistance);
                    if (TPCameraAngle != -1)
                    {
                        thirdPerson.SetAngle(TPCameraAngle);
                    }
                    if (positional_smoothing != -1)
                    {
                        thirdPerson.SetPositionalSmooting(positional_smoothing);
                    }
                    if (rotational_smoothing != -1)
                    {
                        thirdPerson.SetRotationalSmooting(rotational_smoothing);
                    }
                }
                else if (pov == "FP")
                {
                    firstPerson.SetPOV();
                    firstPerson.SetFOV(fov);
                    if (positional_smoothing != -1)
                    {
                        firstPerson.SetPositionalSmooting(positional_smoothing);
                    }
                    if (rotational_smoothing != -1)
                    {
                        firstPerson.SetRotationalSmooting(rotational_smoothing);
                    }
                }
                else if (pov == "HH")
                {
                    handheld.SetPOV();
                    handheld.SetFOV(fov);
                    if (positional_smoothing != -1)
                    {
                        handheld.SetPositionalSmooting(positional_smoothing);
                    }
                    if (rotational_smoothing != -1)
                    {
                        handheld.SetRotationalSmooting(rotational_smoothing);
                    }
                }
                else
                {
                    MelonLogger.Error("Failed to find correct pov");
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                MelonLogger.Error(ex);
                return false;
            }
        }
        //This function is required to be run every frame if you want the isShown variable to be accurate.
        //This variable will be 0 if the camera isn't shown, 1 if it became hidden just now, 2 if it shown and 3 if it became shown just now
        public int IsShownUpdate()
        {
            try
            {
                int o = 0;
                if (((isShown % 2) == 1) != _Camera.TabletVisualsAreActive) { o += 2; }
                if (_Camera.TabletVisualsAreActive)
                {
                    o++;
                }
                isShown = o;
                return o;
            }
            catch (Exception ex)
            {
                MelonLogger.Error(ex);
                return -1;
            }
        }
        //Returns whether the camera is recording or not, will probs change later to work the same way as isShown
        public bool IsRecording()
        {
            try
            {
                isRecording = _Tablet.isRecording;
                return isRecording;
            }
            catch (Exception ex)
            {
                MelonLogger.Error(ex);
                return false;
            }
        }
        //This function is required to be run every frame if you want the POVChanged to be accurate
        //The function updates the POV and POVChanged variable
        //POVChanged will be true iff the POV is different from what it was on the previous frame
        public bool POVUpdate()
        {
            try
            {
                bool povchanged = false;
                if (_CameraController.CurrentCameraMode == Il2CppRUMBLE.Recording.LCK.Extensions.CameraMode.ThirdPerson)
                {
                    if (POV != "TP")
                    {
                        povchanged = true;
                    }
                    POV = "TP";
                }
                else if (_CameraController.CurrentCameraMode == Il2CppRUMBLE.Recording.LCK.Extensions.CameraMode.FirstPerson)
                {
                    if (POV != "FP")
                    {
                        povchanged = true;
                    }
                    POV = "FP";
                }
                else if (_CameraController.CurrentCameraMode == Il2CppRUMBLE.Recording.LCK.Extensions.CameraMode.Selfie)
                {
                    if (POV != "HH")
                    {
                        povchanged = true;
                    }
                    POV = "HH";
                }
                else
                {
                    MelonLogger.Error("Error determining the POV");
                    return false;
                }
                POVChanged = povchanged;
                return true;
            }
            catch (Exception ex)
            {
                MelonLogger.Error(ex);
                return false;
            }
        }
        //Updates the FOV, maybe i'll make it work like POVUpdate()
        public bool FOVUpdate()
        {
            try
            {
                FOV = (int)_CameraController.GetCurrentModeFOV();
                return true;
            }
            catch (Exception ex)
            {
                MelonLogger.Error(ex);
                return false;
            }
        }
        //Returns the spawn position the camera would have if it were spawned right now
        public Vector3 GetSpawnPosition()
        {
            try
            {
                var player = PlayerManager.Instance.localPlayer.Controller.transform.GetChild(2).GetChild(0).GetChild(0);
                return player.position + new Vector3(player.forward.x, 0, player.forward.z).normalized + new Vector3(0, SpawnYOffset, 0);
            }
            catch (Exception ex)
            {
                MelonLogger.Error(ex);
                return Vector3.zero;
            }
        }
        //Returns the spawn rotation the camera would have if it were spawned right now
        public Quaternion GetSpawnRotation()
        {
            try
            {
                var player = PlayerManager.Instance.localPlayer.Controller.transform.GetChild(2).GetChild(0).GetChild(0);
                return Quaternion.LookRotation(-new Vector3(player.forward.x, 0, player.forward.z).normalized - new Vector3(0, SpawnYOffset, 0));
            }
            catch (Exception ex)
            {
                MelonLogger.Error(ex);
                return Quaternion.identity;
            }
        }
        public bool SetResolution(uint width, uint height)
        {
            try
            {
                CameraResolutionDescriptor resolution = new(width, height);
                _LckService.SetTrackResolution(resolution);
                return true;
            }
            catch (Exception ex)
            {
                MelonLogger.Error(ex);
                return false;
            }
        }
        public bool SetVideoBitrate(uint bitrate)
        {
            try
            {
                _LckService.SetTrackBitrate(bitrate);
                return true;
            }
            catch (Exception ex)
            {
                MelonLogger.Error(ex);
                return false;
            }
        }
        public bool SetAudioBitrate(uint bitrate)
        {
            try
            {
                _LckService.SetTrackAudioBitrate(bitrate);
                return true;
            }
            catch (Exception ex)
            {
                MelonLogger.Error(ex);
                return false;
            }
        }
        public bool SetFramerate(uint framerate)
        {
            try
            {
                _LckService.SetTrackFramerate(framerate);
                return true;
            }
            catch (Exception ex)
            {
                MelonLogger.Error(ex);
                return false;
            }
        }
        //No idea what this does, maybe mic volume?
        public bool SetMicrophoneGainUntested(float gain)
        {
            try
            {
                _LckService.SetMicrophoneGain(gain);
                return true;
            }
            catch (Exception ex)
            {
                MelonLogger.Error(ex);
                return false;
            }
        }
        //Macros to set up several parameters at once
        public bool FullRecordingSetupTemp(uint width = 1920, uint height = 1080, uint framerate = 60, uint videoBitrate = 10485760, uint audioBitrate = 1048576)
        {
            try
            {
                CameraResolutionDescriptor resolution = new(width, height);
                CameraTrackDescriptor settings = new(resolution, videoBitrate, framerate, audioBitrate);
                _LckService.SetTrackDescriptor(settings);
                return true;
            }
            catch (Exception ex)
            {
                MelonLogger.Error(ex);
                return false;
            }
        }
        public bool FullRecordingSetupTemp(RecordingSettings recordingSettings)
        {
            try
            {
                return FullRecordingSetupTemp(recordingSettings.width, recordingSettings.height,
                    recordingSettings.framerate, recordingSettings.Bitrate, recordingSettings.audioBitrate);
            }
            catch (Exception ex)
            {
                MelonLogger.Error(ex);
                return false;
            }
        }
        public bool FullRecordingSetup(uint width = 1920, uint height = 1080, uint framerate = 60, uint videoBitrate = 10485760, uint audioBitrate = 1048576)
        {
            try
            {
                CameraResolutionDescriptor resolution = new(width, height);
                CameraTrackDescriptor settings = new(resolution, videoBitrate, framerate, audioBitrate);
                QualityOption qualityOption = new QualityOption();
                qualityOption.CameraTrackDescriptor = settings;
                qualityOption.IsDefault = true;
                _QualitySelector._qualityOptions[1]= qualityOption;
                _QualitySelector.UpdateCurrentTrackDescriptor(1);
                return true;
            }
            catch (Exception ex)
            {
                MelonLogger.Error(ex);
                return false;
            }
        }
        public bool FullRecordingSetup(RecordingSettings recordingSettings)
        {
            try
            {
                return FullRecordingSetup(recordingSettings.width, recordingSettings.height,
                    recordingSettings.framerate, recordingSettings.Bitrate, recordingSettings.audioBitrate);
            }
            catch (Exception ex)
            {
                MelonLogger.Error(ex);
                return false;
            }
        }
        //Probably controls whether the mic is recorded or not, but haven't checked yet
        public bool RecordMicUntested(bool record)
        {
            try
            {
                _LckService.SetMicrophoneCaptureActive(record);
                return true;
            }
            catch (Exception ex)
            {
                MelonLogger.Error(ex);
                return false;
            }
        }
        //Using some voodoo magic to set the pov without the usual function that does that, could result in weird behaviour
        public bool SetPOVBypassUntested(string pov = "Null", int detachedpreview = -1)
        {
            try
            {
                string monitorid = "";
                if(detachedpreview == -1)
                {
                    monitorid = null;
                }
                else
                {
                    monitorid = _DetachedPreviewManager.availablePreviews[detachedpreview].AttachingMonitor.MonitorId;
                }
                if(pov == "Null")
                {
                    pov = POV;
                }
                if (pov == "TP")
                {

                    _LckService.SetActiveCamera(thirdPerson._Camera.CameraId, monitorid);
                }
                else if (pov == "FP")
                {
                    _LckService.SetActiveCamera(firstPerson._Camera.CameraId, monitorid);
                }
                else if (pov == "HH")
                {
                    _LckService.SetActiveCamera(handheld._Camera.CameraId, monitorid);
                }
                else
                {
                    MelonLogger.Error("pov variable must either be TP, FP or HH, you can use the POVs variable for readability if needed.");
                    return false;
                }
                UpdateDetachedPreview();
                return true;
            }
            catch (Exception ex)
            {
                MelonLogger.Error(ex);
                return false;
            }
        }
        //This function is required to be run every frame for the DetachedPreviewChanged variable to be accurate.
        //Updates the DetachedPreview and DetachedPreviewChanged variables
        public bool UpdateDetachedPreview()
        {
            try
            {
                if (DetachedPreview != _DetachedPreviewManager.ActivePreviewNo) DetachedPreviewChanged = true;
                else DetachedPreviewChanged = false;
                DetachedPreview = _DetachedPreviewManager.ActivePreviewNo;
                return true;
            }
            catch (Exception ex)
            {
                MelonLogger.Error(ex);
                return false;
            }
        }
        //Recording settings for a camera in the current mode
        public RecordingSettings GetRecordingSettings()
        {
            try
            {
                return CameraTrackDescriptorToRecordingSettings(_LckService.GetDescriptor().Result.cameraTrackDescriptor);
            }
            catch (Exception ex)
            {
                MelonLogger.Error(ex);
                return new();
            }
        }
        //Recording settings for a camera in vertical mode
        public RecordingSettings GetVerticalRecordingSettings()
        {
            try
            {
                return CameraTrackDescriptorToRecordingSettings(_CameraController._verticalCameraTrackDescriptor);
            }
            catch (Exception ex)
            {
                MelonLogger.Error(ex);
                return new();
            }
        }
        //Recording settings for a camera in horizontal mode
        public RecordingSettings GetHorizontalSettings()
        {
            try
            {
                return CameraTrackDescriptorToRecordingSettings(_CameraController._horizontalCameraTrackDescriptor);
            }
            catch (Exception ex)
            {
                MelonLogger.Error(ex);
                return new();
            }
        }
        //Convert the CameraTrackDescriptor type(the one used ingame) to RecordingSettings(type defined within RCE), to allow for easier usage
        public RecordingSettings CameraTrackDescriptorToRecordingSettings(CameraTrackDescriptor descriptor)
        {
            var settings = new RecordingSettings();
            settings.width = descriptor.CameraResolutionDescriptor.Width;
            settings.height = descriptor.CameraResolutionDescriptor.Height;
            settings.framerate = descriptor.Framerate;
            settings.audioBitrate = descriptor.AudioBitrate;
            settings.Bitrate = descriptor.Bitrate;
            return settings;
        }

    }
}
