using Il2CppLiv.Lck;
using Il2CppLiv.Lck.Recorder;
using Il2CppLiv.Lck.Tablet;
using Il2CppRUMBLE.Managers;
using Il2CppRUMBLE.Players.Subsystems;
using Il2CppRUMBLE.Recording.LCK;
using Il2CppTMPro;
using MelonLoader;
using RumbleModdingAPI;
using System.Collections;
using System.Data.SqlTypes;
//using RumbleModUI;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Rendering;
using Il2CppSmartLocalization;
namespace Rock_Cam_Essentials
{
    public static class BuildInfo
    {
        public const string ModName = "RockCamEssentials";
        public const string ModVersion = "1.4.1";
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
        public int TPmaxFOV = 100;
        public int FPmaxFOV = 100;
        public int HHmaxFOV = 100;
        public int TPminFOV = 30;
        public int FPminFOV = 30;
        public int HHminFOV = 30;
        public int TPFOVStep = 10;
        public int FPFOVStep = 10;
        public int HHFOVStep = 10;
        public Il2CppRUMBLE.Recording.LCK.Extensions.LckDoubleButton _TPFOVSetter;
        public Il2CppRUMBLE.Recording.LCK.Extensions.LckDoubleButton _FPFOVSetter;
        public Il2CppRUMBLE.Recording.LCK.Extensions.LckDoubleButton _HHFOVSetter;
        public int Smoothing = 0;
        public bool isFlippedTP = false;
        public bool isFlippedHH = false;
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
        public float ThirdPersonDistance = 1;
        public float ThirdPersonAngle;
        public PlayerLIV _Camera;
        public LCKTabletUtility _Tablet;
        public Il2CppRUMBLE.Recording.LCK.Extensions.LCKCameraController _CameraController;
        public Il2CppRUMBLE.Recording.LCK.Extensions.LCKSettingsButtonsController _POVController;
        public float TPPositionalSmoothing = 0;
        public float TPRotationalSmoothing = 0;
        public float FPPositionalSmoothing = 0;
        public float FPRotationalSmoothing = 0;
        public float HHPositionalSmoothing = 0;
        public float HHRotationalSmoothing = 0;
        public Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppReferenceArray<Transform> DetachedMonitors;
        public Transform RockCamTransform;
        public LckCamera _TPCamera;
        public LckCamera _FPCamera;
        public LckCamera _HHCamera;
        public UnityEngine.Rendering.Universal.UniversalAdditionalCameraData _TPCameraSettings;
        public UnityEngine.Rendering.Universal.UniversalAdditionalCameraData _FPCameraSettings;
        public UnityEngine.Rendering.Universal.UniversalAdditionalCameraData _HHCameraSettings;
        public LckService _LckService;
        public int isShown = 1;
        public bool POVChanged = false;
        public LCKTabletDetachedPreview _DetachedPreviewManager;
        public Il2CppTMPro.TMP_Text Nameplate;
        public struct POVNames
        {
            public string ThirdPerson, FirstPerson, Handheld;
        }
        public POVNames POVs;
        public struct RecordingSettings
        {
            public uint width, height, audioBitrate, Bitrate, framerate;
        }
        public struct NameplateSettings
        {
            public Color color;

        }
        public Rock_Cam()
        {
            POVs.ThirdPerson = "TP";
            POVs.FirstPerson = "FP";
            POVs.Handheld = "HH";
            Fix();
        }
        //Everything related to operations on the physical rockcam
        public class ThirdPersonCamera
        {
            
        }
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
                ThirdPersonDistance = _CameraController._thirdPersonDistance;
                isHorizontal = _CameraController.IsHorizontalMode;
                _POVController = _CameraController._settingsButtonsController;
                ThirdPersonAngle = _CameraController._thirdPersonHeightAngle;
                TPPositionalSmoothing = _CameraController._thirdPersonStabilizer.PositionalSmoothing;
                TPRotationalSmoothing = _CameraController._thirdPersonStabilizer.RotationalSmoothing;
                FPPositionalSmoothing = _CameraController._firstPersonStabilizer.PositionalSmoothing;
                FPRotationalSmoothing = _CameraController._firstPersonStabilizer.RotationalSmoothing;
                HHPositionalSmoothing = _CameraController._selfieStabilizer.PositionalSmoothing;
                HHRotationalSmoothing = _CameraController._selfieStabilizer.RotationalSmoothing;
                DetachedMonitors = _CameraController._monitorTransforms;
                _TPFOVSetter = _CameraController.ThirdPersonFOVDoubleButton;
                _FPFOVSetter = _CameraController.FirstPersonFOVDoubleButton;
                _HHFOVSetter = _CameraController.SelfieFOVDoubleButton;
                FOV = _TPFOVSetter._currentValue;
                TPmaxFOV = _TPFOVSetter._maxValue;
                TPminFOV = _TPFOVSetter._minValue;
                TPFOVStep = _TPFOVSetter._increment;
                FOV = _FPFOVSetter._currentValue;
                FPmaxFOV = _FPFOVSetter._maxValue;
                FPminFOV = _FPFOVSetter._minValue;
                FPFOVStep = _FPFOVSetter._increment;
                FOV = _HHFOVSetter._currentValue;
                HHmaxFOV = _HHFOVSetter._maxValue;
                HHminFOV = _HHFOVSetter._minValue;
                HHFOVStep = _HHFOVSetter._increment;
                PhotoTimer = _Tablet.photoTimerCurrentValue;
                PhotoTimerIncrement = _Tablet.photoTimerIncrementValue;
                PhotoTimerMaxValue = _Tablet.photoTimerMaxValue;
                RockCamTransform = _Tablet.transform;
                if (_Camera.TabletVisualsAreActive) isShown = 1;
                else isShown = 0;
                _TPCamera = _Tablet.thirdPersonCamera;
                _FPCamera = _Tablet.firstPersonCamera;
                _HHCamera = _Tablet.selfieCamera;
                POVUpdate();
                POVChanged = false;
                _TPCameraSettings = _Tablet.thirdPersonCamera.gameObject.GetComponent<UnityEngine.Rendering.Universal.UniversalAdditionalCameraData>();
                _FPCameraSettings = _Tablet.firstPersonCamera.gameObject.GetComponent<UnityEngine.Rendering.Universal.UniversalAdditionalCameraData>();
                _HHCameraSettings = _Tablet.selfieCamera.gameObject.GetComponent<UnityEngine.Rendering.Universal.UniversalAdditionalCameraData>();
                _LckService = _Camera.lckService;
                _DetachedPreviewManager = _Tablet.lckDetachedPreview;
                Nameplate = _Tablet.gameObject.GetComponent<LCKTabletNameplate>().nameplateTextComponent.textObject;
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
        //Hides the table, same function used when trying to summon the table while being close to it
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
        //Ends recording, won't do anything if already not recording
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
        //Sets the POV to handheld(selfie) mode
        public bool SetHandheldPOV()
        {
            try
            {
                _POVController.SwitchToSelfieMode();
                POV = "HH";
            }
            catch (Exception ex)
            {
                MelonLogger.Error(ex);
                return false;
            }
            return true;
        }
        //Sets the POV to first person mode
        public bool SetFirstPersonPOV()
        {
            try
            {
                _POVController.SwitchToFPMode();
                POV = "FP";
            }
            catch (Exception ex)
            {
                MelonLogger.Error(ex);
                return false;
            }
            return true;
        }
        //Sets the POV to third person mode.
        public bool SetThirdPersonPOV()
        {
            try
            {
                _POVController.SwitchToTPMode();
                POV = "TP";
            }
            catch (Exception ex)
            {
                MelonLogger.Error(ex);
                return false;
            }
            return true;
        }
        //Sets the angle at which the third person camera follows the player
        public bool SetThirdPersonAngle(float angle)
        {
            try
            {
                _CameraController._thirdPersonHeightAngle = angle;
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
        public bool SetThirdPersonDistance(float distance)
        {
            try
            {
                _CameraController._thirdPersonDistance = distance;
                return true;
            }
            catch (Exception ex)
            {
                MelonLogger.Error(ex);
                return false;
            }
        }
        //Sets the distance multiplier that is multiplier, that is multiplied by the distance to get the actual distance at which the camera will follow the player
        public bool SetThirdPersonDistanceMultiplier(float distanceMultiplier)
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
        public bool FlipCameraThirdPerson(bool flip)
        {
            if (isFlippedTP != _CameraController.IsThirdPersonFront)
            {
                MelonLogger.Msg("isFlippedTP variable desynched");
                isFlippedHH = _CameraController.IsThirdPersonFront;
            }
            if (flip == isFlippedTP)
            {
                MelonLogger.Msg("TP camera already flipped correct way");
                return true;
            }
            try
            {
                isFlippedTP = flip;
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
        public bool SetThirdPersonPositionalSmooting(float smooth)
        {
            if (smooth >= 1 && smooth != 99)
            {
                MelonLogger.Warning("Smoothing of 1 or greater will result in no movement at all, you can set smoothing to 99 to avoid this message");
            }
            try
            {
                _CameraController._thirdPersonStabilizer.PositionalSmoothing = smooth;
                TPPositionalSmoothing = smooth;
            }
            catch (Exception ex)
            {
                MelonLogger.Error(ex);
                return false;
            }
            return true;
        }
        //Sets the rotational smoothing of the third person camera
        public bool SetThirdPersonRotationalSmooting(float smooth)
        {
            if (smooth >= 1 && smooth != 99)
            {
                MelonLogger.Warning("Smoothing of 1 or greater will result in no movement at all, you can set smoothing to 99 to avoid this message");
            }
            try
            {
                _CameraController._thirdPersonStabilizer.RotationalSmoothing = smooth;
                TPRotationalSmoothing = smooth;
            }
            catch (Exception ex)
            {
                MelonLogger.Error(ex);
                return false;
            }
            return true;
        }
        //Sets the positional smoothing of the first person camera
        public bool SetFirstPersonPositionalSmooting(float smooth)
        {
            if (smooth >= 1 && smooth != 99)
            {
                MelonLogger.Warning("Smoothing of 1 or greater will result in no movement at all, you can set smoothing to 99 to avoid this message");
            }
            try
            {
                _CameraController._firstPersonStabilizer.PositionalSmoothing = smooth;
                FPPositionalSmoothing = smooth;
            }
            catch (Exception ex)
            {
                MelonLogger.Error(ex);
                return false;
            }
            return true;
        }
        //Sets the rotational smoothing of the first person camera
        public bool SetFirstPersonRotationalSmooting(float smooth)
        {
            if (smooth >= 1 && smooth != 99)
            {
                MelonLogger.Warning("Smoothing of 1 or greater will result in no movement at all, you can set smoothing to 99 to avoid this message");
            }
            try
            {
                _CameraController._firstPersonStabilizer.RotationalSmoothing = smooth;
                FPRotationalSmoothing = smooth;
            }
            catch (Exception ex)
            {
                MelonLogger.Error(ex);
                return false;
            }
            return true;
        }
        //Sets the positional smoothing of the handheld camera
        public bool SetHandheldPositionalSmooting(float smooth)
        {
            if (smooth >= 1 && smooth != 99)
            {
                MelonLogger.Warning("Smoothing of 1 or greater will result in no movement at all, you can set smoothing to 99 to avoid this message");
            }
            try
            {
                _CameraController._selfieStabilizer.PositionalSmoothing = smooth;
                HHPositionalSmoothing = smooth;
            }
            catch (Exception ex)
            {
                MelonLogger.Error(ex);
                return false;
            }
            return true;
        }
        //Sets the rotational smoothing of the handheld camera
        public bool SetHandheldRotationalSmooting(float smooth)
        {
            if (smooth >= 1 && smooth != 99)
            {
                MelonLogger.Warning("Smoothing of 1 or greater will result in no movement at all, you can set smoothing to 99 to avoid this message");
            }
            try
            {
                _CameraController._selfieStabilizer.RotationalSmoothing = smooth;
                HHRotationalSmoothing = smooth;
            }
            catch (Exception ex)
            {
                MelonLogger.Error(ex);
                return false;
            }
            return true;
        }
        //Sets the rotational smoothing for all cameras
        public bool SetGlobalRotationalSmoothing(float smooth)
        {
            return SetThirdPersonRotationalSmooting(smooth) && SetFirstPersonRotationalSmooting(smooth) && SetHandheldRotationalSmooting(smooth);
        }
        //Sets the positional smoothing for all cameras
        public bool SetGlobalPositionalSmoothing(float smooth)
        {
            return SetThirdPersonPositionalSmooting(smooth) && SetFirstPersonPositionalSmooting(smooth) && SetHandheldPositionalSmooting(smooth);
        }
        //Makes the camera instantly reach its destination ignoring smoothing
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
        //Flips the handheld camera, very buggy
        public bool FlipCameraHandheld(bool flip)
        {
            if (isFlippedHH == flip)
            {
                MelonLogger.Msg("HH camera already flipped the right way");
                return true;
            }
            try
            {
                _CameraController.ProcessSelfieFlip();
                isFlippedHH = flip;
                return true;
            }
            catch (Exception ex)
            {
                MelonLogger.Error(ex);
                return false;
            }

        }
        //Sets the FOV of the third person camera, written in a way where it will also update the ingame values shown
        public bool SetThirdPersonFOV(int fov)
        {
            if (_TPFOVSetter._currentValue != FOV)
            {
                MelonLogger.Msg("FOV value desynched");
                FOV = _TPFOVSetter._currentValue;
            }
            try
            {
                _TPFOVSetter._maxValue = 999999;
                _TPFOVSetter._increment = fov - FOV;
                _TPFOVSetter.OnPressDownIncrease();
                _TPFOVSetter.OnPressUpIncrease();
                _TPFOVSetter._maxValue = TPmaxFOV;
                _TPFOVSetter._increment = TPFOVStep;
                FOV = fov;
                if (FOV != _TPFOVSetter._currentValue)
                {
                    MelonLogger.Msg(FOV.ToString() + " " + _TPFOVSetter._currentValue.ToString());
                }
                return true;
            }
            catch (Exception ex)
            {
                MelonLogger.Error(ex);
                return false;
            }
        }
        //Sets the FOV of the first person camera, written in a way where it will also update the ingame values shown
        public bool SetFirstPersonFOV(int fov)
        {
            if (_FPFOVSetter._currentValue != FOV)
            {
                MelonLogger.Msg("FOV value desynched");
                FOV = _FPFOVSetter._currentValue;
            }
            try
            {
                _FPFOVSetter._maxValue = 999999;
                _FPFOVSetter._increment = fov - FOV;
                _FPFOVSetter.OnPressDownIncrease();
                _FPFOVSetter.OnPressUpIncrease();
                _FPFOVSetter._maxValue = FPmaxFOV;
                _FPFOVSetter._increment = FPFOVStep;
                FOV = fov;
                return true;
            }
            catch (Exception ex)
            {
                MelonLogger.Error(ex);
                return false;
            }
        }
        //Sets the FOV of the handheld camera, written in a way where it will also update the ingame values shown
        public bool SetHandheldFOV(int fov)
        {
            if (_HHFOVSetter._currentValue != FOV)
            {
                MelonLogger.Msg("FOV value desynched");
                FOV = _HHFOVSetter._currentValue;
            }
            try
            {
                _HHFOVSetter._maxValue = 999999;
                _HHFOVSetter._increment = fov - FOV;
                _HHFOVSetter.OnPressDownIncrease();
                _HHFOVSetter.OnPressUpIncrease();
                _HHFOVSetter._maxValue = HHmaxFOV;
                _HHFOVSetter._increment = HHFOVStep;
                FOV = fov;
                return true;
            }
            catch (Exception ex)
            {
                MelonLogger.Error(ex);
                return false;
            }
        }
        //Sets the settings settings, aka the bounds for the FOV setting ingame, for the third person camera
        public bool SetThirdPersonFOVSettingsSettings(int maxFOV, int minFOV, int step)
        {
            try
            {
                _TPFOVSetter._maxValue = maxFOV;
                _TPFOVSetter._increment = step;
                _TPFOVSetter._minValue = minFOV;
                TPmaxFOV = maxFOV;
                TPminFOV = minFOV;
                TPFOVStep = step;
                return true;
            }
            catch (Exception ex)
            {
                MelonLogger.Error(ex);
                return false;
            }
        }
        //Sets the settings settings, aka the bounds for the FOV setting ingame, for the first person camera
        public bool SetFirstPersonFOVSettingsSettings(int maxFOV, int minFOV, int step)
        {
            try
            {
                _FPFOVSetter._maxValue = maxFOV;
                _FPFOVSetter._increment = step;
                _FPFOVSetter._minValue = minFOV;
                FPmaxFOV = maxFOV;
                FPminFOV = minFOV;
                FPFOVStep = step;
                return true;
            }
            catch (Exception ex)
            {
                MelonLogger.Error(ex);
                return false;
            }
        }
        //Sets the settings settings, aka the bounds for the FOV setting ingame, for the handheld camera
        public bool SetHandheldFOVSettingsSettings(int maxFOV, int minFOV, int step)
        {
            try
            {
                _HHFOVSetter._maxValue = maxFOV;
                _HHFOVSetter._increment = step;
                _HHFOVSetter._minValue = minFOV;
                HHmaxFOV = maxFOV;
                HHminFOV = minFOV;
                HHFOVStep = step;
                return true;
            }
            catch (Exception ex)
            {
                MelonLogger.Error(ex);
                return false;
            }
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
        public bool SetRelativeHandheldCameraPosition(Vector3 position, Vector3 rotation)
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
        public bool SetGlobalHandheldCameraPosition(Vector3 position, Quaternion rotation)
        {
            try
            {
                var position2 = RockCamTransform.InverseTransformPoint(position);
                Quaternion rotation2 = Quaternion.Inverse(RockCamTransform.rotation) * rotation;
                _CameraController.SetSelfieCameraOrientation(position2, rotation2.eulerAngles);
                return true;
            }
            catch (Exception ex)
            {
                MelonLogger.Error(ex);
                return false;
            }
        }
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
                    SetThirdPersonPOV();
                    SetThirdPersonFOV(fov);
                    if (TPCameraDistance == -1)
                    {
                        TPCameraDistance = ThirdPersonDistance;
                    }
                    SetThirdPersonDistance(TPCameraDistance);
                    if (TPCameraAngle != -1)
                    {
                        SetThirdPersonAngle(TPCameraAngle);
                    }
                    if (positional_smoothing != -1)
                    {
                        SetThirdPersonPositionalSmooting(positional_smoothing);
                    }
                    if (rotational_smoothing != -1)
                    {
                        SetThirdPersonRotationalSmooting(rotational_smoothing);
                    }
                }
                else if (pov == "FP")
                {
                    SetFirstPersonPOV();
                    SetFirstPersonFOV(fov);
                    if (positional_smoothing != -1)
                    {
                        SetFirstPersonPositionalSmooting(positional_smoothing);
                    }
                    if (rotational_smoothing != -1)
                    {
                        SetFirstPersonRotationalSmooting(rotational_smoothing);
                    }
                }
                else if (pov == "HH")
                {
                    SetHandheldPOV();
                    SetHandheldFOV(fov);
                    if (positional_smoothing != -1)
                    {
                        SetHandheldPositionalSmooting(positional_smoothing);
                    }
                    if (rotational_smoothing != -1)
                    {
                        SetHandheldRotationalSmooting(rotational_smoothing);
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
        public bool DoPostProcessing(string pov, bool value)
        {
            try
            {
                if (pov == "TP")
                {
                    _TPCameraSettings.renderPostProcessing = value;
                }
                else if (pov == "FP")
                {
                    _FPCameraSettings.renderPostProcessing = value;
                }
                else if (pov == "HH")
                {
                    _HHCameraSettings.renderPostProcessing = value;
                }
                else
                {
                    MelonLogger.Error("pov variable must either be TP, FP or HH, you can use the POVs variable for readability if needed.");
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
        public bool FullRecordingSetup(uint width = 1920, uint height = 1080, uint framerate = 60, uint videoBitrate = 10485760, uint audioBitrate = 1048576)
        {
            try
            {
                CameraResolutionDescriptor resolution = new(width, height);
                CameraTrackDescriptor settings = new(resolution, framerate, videoBitrate, audioBitrate);
                _LckService.SetTrackDescriptor(settings);
                return true;
            }
            catch (Exception ex)
            {
                MelonLogger.Error(ex);
                return false;
            }
        }
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

                    _LckService.SetActiveCamera(_TPCamera.CameraId, monitorid);
                }
                else if (pov == "FP")
                {
                    _LckService.SetActiveCamera(_FPCamera.CameraId, monitorid);
                }
                else if (pov == "HH")
                {
                    _LckService.SetActiveCamera(_HHCamera.CameraId, monitorid);
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
