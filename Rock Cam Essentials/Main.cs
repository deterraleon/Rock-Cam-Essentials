using Il2CppLiv.Lck;
using Il2CppLiv.Lck.Tablet;
using Il2CppRUMBLE.Managers;
using Il2CppRUMBLE.Players.Subsystems;
using Il2CppRUMBLE.Recording.LCK;
using MelonLoader;
using RumbleModdingAPI;
using System.Collections;
using System.Data.SqlTypes;
//using RumbleModUI;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Rendering;
namespace Rock_Cam_Essentials
{
    public static class BuildInfo
    {
        public const string ModName = "RockCamEssentials";
        public const string ModVersion = "1.2.1";
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
        public Il2CppRUMBLE.Recording.LCK.Extensions.LckDoubleButton TPFOVSetter;
        public Il2CppRUMBLE.Recording.LCK.Extensions.LckDoubleButton FPFOVSetter;
        public Il2CppRUMBLE.Recording.LCK.Extensions.LckDoubleButton HHFOVSetter;
        public int Smoothing = 0;
        public bool isFlippedTP = false;
        public bool isFlippedHH = false;
        public string POV = "Handheld";
        public bool isHorizontal = true;
        public int PhotoTimer = 0;
        public int PhotoTimerIncrement = 2;
        public int PhotoTimerMaxValue = 8;
        public bool isRecording = false;
        public int DetachedPreview = 0;
        public int MaxDespawnDistance = 2;
        public float SpawnYOffset = -0.1f;
        public float TabletSpawnDelay = 0.3f;
        public float MaximumRenderDistance = 2;
        public float ThirdPersonDistance = 1;
        public float ThirdPersonAngle = 15;
        public PlayerLIV Camera;
        public LCKTabletUtility Tablet;
        public Il2CppRUMBLE.Recording.LCK.Extensions.LCKCameraController CameraController;
        public Il2CppRUMBLE.Recording.LCK.Extensions.LCKSettingsButtonsController POVController;
        public float TPPositionalSmoothing = 0;
        public float TPRotationalSmoothing = 0;
        public float FPPositionalSmoothing = 0;
        public float FPRotationalSmoothing = 0;
        public float HHPositionalSmoothing = 0;
        public float HHRotationalSmoothing = 0;
        public Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppReferenceArray<Transform> DetachedMonitors;
        public Transform RockCamTransform;
        public LckCamera TPCamera;
        public LckCamera FPCamera;
        public LckCamera HHCamera;
        public UnityEngine.Rendering.Universal.UniversalAdditionalCameraData TPCameraSettings;
        public UnityEngine.Rendering.Universal.UniversalAdditionalCameraData FPCameraSettings;
        public UnityEngine.Rendering.Universal.UniversalAdditionalCameraData HHCameraSettings;

        public int isShown = 1;
        public bool POVChanged = false;
        public struct POVNames
        {
            public string ThirdPerson, FirstPerson, Handheld;
        }
        public POVNames POVs;
        public Rock_Cam()
        {
            POVs.ThirdPerson = "TP";
            POVs.FirstPerson = "FP";
            POVs.Handheld = "HH";
            Fix();
        }
        public bool Fix()
        {
            try
            {
                Camera = PlayerManager.instance.localPlayer.Controller.GetComponentInChildren<PlayerLIV>();
                MaxDespawnDistance = Camera.maxDespawnDistance;
                SpawnYOffset = Camera.spawnYOffset;
                TabletSpawnDelay = Camera.tabletSpawnDelay;
                Tablet = Camera.LckTablet;
                MaximumRenderDistance = Tablet.maximumRenderDistance;
                isRecording = Tablet.isRecording;
                CameraController = Tablet.LckCameraController;
                ThirdPersonDistance = CameraController._thirdPersonDistance;
                isHorizontal = CameraController.IsHorizontalMode;
                POVController = CameraController._settingsButtonsController;
                ThirdPersonAngle = CameraController._thirdPersonHeightAngle;
                TPPositionalSmoothing = CameraController._thirdPersonStabilizer.PositionalSmoothing;
                TPRotationalSmoothing = CameraController._thirdPersonStabilizer.RotationalSmoothing;
                FPPositionalSmoothing = CameraController._firstPersonStabilizer.PositionalSmoothing;
                FPRotationalSmoothing = CameraController._firstPersonStabilizer.RotationalSmoothing;
                HHPositionalSmoothing = CameraController._selfieStabilizer.PositionalSmoothing;
                HHRotationalSmoothing = CameraController._selfieStabilizer.RotationalSmoothing;
                DetachedMonitors = CameraController._monitorTransforms;
                TPFOVSetter = CameraController.ThirdPersonFOVDoubleButton;
                FPFOVSetter = CameraController.FirstPersonFOVDoubleButton;
                HHFOVSetter = CameraController.SelfieFOVDoubleButton;
                FOV = TPFOVSetter._currentValue;
                TPmaxFOV = TPFOVSetter._maxValue;
                TPminFOV = TPFOVSetter._minValue;
                TPFOVStep = TPFOVSetter._increment;
                FOV = FPFOVSetter._currentValue;
                FPmaxFOV = FPFOVSetter._maxValue;
                FPminFOV = FPFOVSetter._minValue;
                FPFOVStep = FPFOVSetter._increment;
                FOV = HHFOVSetter._currentValue;
                HHmaxFOV = HHFOVSetter._maxValue;
                HHminFOV = HHFOVSetter._minValue;
                HHFOVStep = HHFOVSetter._increment;
                PhotoTimer = Tablet.photoTimerCurrentValue;
                PhotoTimerIncrement = Tablet.photoTimerIncrementValue;
                PhotoTimerMaxValue = Tablet.photoTimerMaxValue;
                RockCamTransform = Tablet.transform;
                if (Camera.TabletVisualsAreActive) isShown = 1;
                else isShown = 0;
                TPCamera = Tablet.thirdPersonCamera;
                FPCamera = Tablet.firstPersonCamera;
                HHCamera = Tablet.selfieCamera;
                POVUpdate();
                POVChanged = false;
                TPCameraSettings = Tablet.thirdPersonCamera.gameObject.GetComponent<UnityEngine.Rendering.Universal.UniversalAdditionalCameraData>();
                FPCameraSettings = Tablet.firstPersonCamera.gameObject.GetComponent<UnityEngine.Rendering.Universal.UniversalAdditionalCameraData>();
                HHCameraSettings = Tablet.selfieCamera.gameObject.GetComponent<UnityEngine.Rendering.Universal.UniversalAdditionalCameraData>();
                return true;
            }
            catch(Exception ex)
            {
                MelonLogger.Error(ex);
                return false;
            }
        }
        public bool TakePhoto()
        {
            try
            {
                Tablet.TakePhoto();
            }
            catch (Exception ex)
            {
                MelonLogger.Error(ex);
                return false;
            }
            return true;
        }
        public bool HideTablet()
        {
            try
            {
                Camera.HideTablet();
            }
            catch (Exception ex)
            {
                MelonLogger.Error(ex);
                return false;
            }
            return true;
        }
        public bool ShowTablet()
        {
            try
            {
                Camera.ShowTablet();
            }
            catch (Exception ex)
            {
                MelonLogger.Error(ex);
                return false;
            }
            return true;
        }
        public bool StartRecording()
        {
            if (isRecording != Camera.LckTablet.isRecording)
            {
                MelonLogger.Msg("isRecording variable is desynched");
                isRecording = Camera.LckTablet.isRecording;
            }
            if (!isRecording)
            {
                isRecording = true;
                try
                {
                    Tablet.ToggleRecordingAndUpdateRecordStatus();
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
        public bool EndRecording()
        {
            if (isRecording != Camera.LckTablet.isRecording)
            {
                MelonLogger.Msg("isRecording variable is desynched");
                isRecording = Camera.LckTablet.isRecording;
            }
            if (isRecording)
            {
                isRecording = false;
                try
                {
                    Tablet.ToggleRecordingAndUpdateRecordStatus();
                }
                catch (Exception ex)
                {
                    MelonLogger.Error(ex);
                    return false;
                }
            }
            return true;
        }
        public bool SetVertiacalOrientation()
        {
            if (isHorizontal != CameraController.IsHorizontalMode)
            {
                MelonLogger.Msg("isHorizontal variable desynched");
                isHorizontal = CameraController.IsHorizontalMode;
            }
            if (isHorizontal)
            {
                isHorizontal = false;
                try
                {
                    CameraController.ToggleOrientation();
                }
                catch (Exception ex)
                {
                    MelonLogger.Error(ex);
                    return false;
                }
            }
            else
            {
                MelonLogger.Msg("Already vertical");
            }
            return true;
        }
        public bool SetHorizontalOrientation()
        {
            if (isHorizontal != CameraController.IsHorizontalMode)
            {
                MelonLogger.Msg("isHorizontal variable desynched");
                isHorizontal = CameraController.IsHorizontalMode;
            }
            if (!isHorizontal)
            {
                isHorizontal = true;
                try
                {
                    CameraController.ToggleOrientation();
                }
                catch (Exception ex)
                {
                    MelonLogger.Error(ex);
                    return false;
                }
            }
            else
            {
                MelonLogger.Msg("Already horizontal");
            }
            return true;
        }
        public bool SetHandheldPOV()
        {
            try
            {
                POVController.SwitchToSelfieMode();
                POV = "HH";
            }
            catch (Exception ex)
            {
                MelonLogger.Error(ex);
                return false;
            }
            return true;
        }
        public bool SetFirstPersonPOV()
        {
            try
            {
                POVController.SwitchToFPMode();
                POV = "FP";
            }
            catch (Exception ex)
            {
                MelonLogger.Error(ex);
                return false;
            }
            return true;
        }
        public bool SetThirdPersonPOV()
        {
            try
            {
                POVController.SwitchToTPMode();
                POV = "TP";
            }
            catch (Exception ex)
            {
                MelonLogger.Error(ex);
                return false;
            }
            return true;
        }
        public bool SetThirdPersonAngle(float angle)
        {
            try
            {
                CameraController._thirdPersonHeightAngle = angle;
                return true;
            }
            catch (Exception ex)
            {
                MelonLogger.Error(ex);
                return false;
            }
        }
        public bool SetThirdPersonDistance(float distance)
        {
            try
            {
                CameraController._thirdPersonDistance = distance;
                return true;
            }
            catch (Exception ex)
            {
                MelonLogger.Error(ex);
                return false;
            }
        }
        public bool SetThirdPersonDistanceMultiplier(float distanceMultiplier)
        {
            try
            {
                CameraController._thirdPersonDistanceMultiplier = distanceMultiplier;
                return true;
            }
            catch (Exception ex)
            {
                MelonLogger.Error(ex);
                return false;
            }
        }
        public bool FlipCameraThirdPerson(bool flip)
        {
            if (isFlippedTP != CameraController.IsThirdPersonFront)
            {
                MelonLogger.Msg("isFlippedTP variable desynched");
                isFlippedHH = CameraController.IsThirdPersonFront;
            }
            if (flip == isFlippedTP)
            {
                MelonLogger.Msg("TP camera already flipped correct way");
                return true;
            }
            try
            {
                isFlippedTP = flip;
                CameraController.IsThirdPersonFront = !CameraController.IsThirdPersonFront;
                return true;
            }
            catch (Exception ex)
            {
                MelonLogger.Error(ex);
                return false;
            }
        }
        public bool SetThirdPersonPositionalSmooting(float smooth)
        {
            if (smooth >= 1 && smooth != 99)
            {
                MelonLogger.Warning("Smoothing of 1 or greater will result in no movement at all, you can set smoothing to 99 to avoid this message");
            }
            try
            {
                CameraController._thirdPersonStabilizer.PositionalSmoothing = smooth;
                TPPositionalSmoothing = smooth;
            }
            catch (Exception ex)
            {
                MelonLogger.Error(ex);
                return false;
            }
            return true;
        }
        public bool SetThirdPersonRotationalSmooting(float smooth)
        {
            if (smooth >= 1 && smooth != 99)
            {
                MelonLogger.Warning("Smoothing of 1 or greater will result in no movement at all, you can set smoothing to 99 to avoid this message");
            }
            try
            {
                CameraController._thirdPersonStabilizer.RotationalSmoothing = smooth;
                TPRotationalSmoothing = smooth;
            }
            catch (Exception ex)
            {
                MelonLogger.Error(ex);
                return false;
            }
            return true;
        }
        public bool SetFirstPersonPositionalSmooting(float smooth)
        {
            if (smooth >= 1 && smooth != 99)
            {
                MelonLogger.Warning("Smoothing of 1 or greater will result in no movement at all, you can set smoothing to 99 to avoid this message");
            }
            try
            {
                CameraController._firstPersonStabilizer.PositionalSmoothing = smooth;
                FPPositionalSmoothing = smooth;
            }
            catch (Exception ex)
            {
                MelonLogger.Error(ex);
                return false;
            }
            return true;
        }
        public bool SetFirstPersonRotationalSmooting(float smooth)
        {
            if (smooth >= 1 && smooth != 99)
            {
                MelonLogger.Warning("Smoothing of 1 or greater will result in no movement at all, you can set smoothing to 99 to avoid this message");
            }
            try
            {
                CameraController._firstPersonStabilizer.RotationalSmoothing = smooth;
                FPRotationalSmoothing = smooth;
            }
            catch (Exception ex)
            {
                MelonLogger.Error(ex);
                return false;
            }
            return true;
        }
        public bool SetHandheldPositionalSmooting(float smooth)
        {
            if (smooth >= 1 && smooth != 99)
            {
                MelonLogger.Warning("Smoothing of 1 or greater will result in no movement at all, you can set smoothing to 99 to avoid this message");
            }
            try
            {
                CameraController._selfieStabilizer.PositionalSmoothing = smooth;
                HHPositionalSmoothing = smooth;
            }
            catch (Exception ex)
            {
                MelonLogger.Error(ex);
                return false;
            }
            return true;
        }
        public bool SetHandheldRotationalSmooting(float smooth)
        {
            if (smooth >= 1 && smooth != 99)
            {
                MelonLogger.Warning("Smoothing of 1 or greater will result in no movement at all, you can set smoothing to 99 to avoid this message");
            }
            try
            {
                CameraController._selfieStabilizer.RotationalSmoothing = smooth;
                HHRotationalSmoothing = smooth;
            }
            catch (Exception ex)
            {
                MelonLogger.Error(ex);
                return false;
            }
            return true;
        }
        public bool SetGlobalRotationalSmoothing(float smooth)
        {
            return SetThirdPersonRotationalSmooting(smooth) && SetFirstPersonRotationalSmooting(smooth) && SetHandheldRotationalSmooting(smooth);
        }
        public bool SetGlobalPositionalSmoothing(float smooth)
        {
            return SetThirdPersonPositionalSmooting(smooth) && SetFirstPersonPositionalSmooting(smooth) && SetHandheldPositionalSmooting(smooth);
        }
        public bool ResetCameraPosition()
        {
            try
            {
                if (POV == "TP")
                {
                    CameraController._thirdPersonStabilizer.ReachTargetInstantly();
                }
                if (POV == "FP")
                {
                    CameraController._firstPersonStabilizer.ReachTargetInstantly();
                }
                if (POV == "HH")
                {
                    CameraController._selfieStabilizer.ReachTargetInstantly();
                }
            }
            catch (Exception ex)
            {
                MelonLogger.Error(ex);
                return false;
            }
            return true;
        }
        public bool SetMaximumRenderDistance(float distance)
        {
            try
            {
                Tablet.maximumRenderDistance = distance;
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
                Tablet.lckDetachedPreview.SwitchPreview(index);
                DetachedPreview = index;
            }
            catch (Exception ex)
            {
                MelonLogger.Error(ex);
                return false;
            }
            return true;
        }
        public bool FlipCameraHandheld(bool flip)
        {
            if (isFlippedHH == flip)
            {
                MelonLogger.Msg("HH camera already flipped the right way");
                return true;
            }
            try
            {
                CameraController.ProcessSelfieFlip();
                isFlippedHH = flip;
                return true;
            }
            catch (Exception ex)
            {
                MelonLogger.Error(ex);
                return false;
            }

        }
        public bool SetThirdPersonFOV(int fov)
        {
            if(TPFOVSetter._currentValue != FOV)
            {
                MelonLogger.Msg("FOV value desynched");
                FOV = TPFOVSetter._currentValue;
            }
            try
            {
                TPFOVSetter._maxValue = 999999;
                TPFOVSetter._increment = fov - FOV;
                TPFOVSetter.OnPressDownIncrease();
                TPFOVSetter.OnPressUpIncrease();
                TPFOVSetter._maxValue = TPmaxFOV;
                TPFOVSetter._increment = TPFOVStep;
                FOV = fov;
                if(FOV != TPFOVSetter._currentValue)
                {
                    MelonLogger.Msg(FOV.ToString() + " " + TPFOVSetter._currentValue.ToString());
                }
                return true;
            }
            catch (Exception ex)
            {
                MelonLogger.Error(ex);
                return false;
            }
        }
        public bool SetFirstPersonFOV(int fov)
        {
            if (FPFOVSetter._currentValue != FOV)
            {
                MelonLogger.Msg("FOV value desynched");
                FOV = FPFOVSetter._currentValue;
            }
            try
            {
                FPFOVSetter._maxValue = 999999;
                FPFOVSetter._increment = fov - FOV;
                FPFOVSetter.OnPressDownIncrease();
                FPFOVSetter.OnPressUpIncrease();
                FPFOVSetter._maxValue = FPmaxFOV;
                FPFOVSetter._increment = FPFOVStep;
                FOV = fov;
                return true;
            }
            catch (Exception ex)
            {
                MelonLogger.Error(ex);
                return false;
            }
        }
        public bool SetHandheldFOV(int fov)
        {
            if (HHFOVSetter._currentValue != FOV)
            {
                MelonLogger.Msg("FOV value desynched");
                FOV = HHFOVSetter._currentValue;
            }
            try
            {
                HHFOVSetter._maxValue = 999999;
                HHFOVSetter._increment = fov - FOV;
                HHFOVSetter.OnPressDownIncrease();
                HHFOVSetter.OnPressUpIncrease();
                HHFOVSetter._maxValue = HHmaxFOV;
                HHFOVSetter._increment = HHFOVStep;
                FOV = fov;
                return true;
            }
            catch (Exception ex)
            {
                MelonLogger.Error(ex);
                return false;
            }
        }
        public bool SetThirdPersonFOVSettingsSettings(int maxFOV, int minFOV, int step)
        {
            try
            {
                TPFOVSetter._maxValue = maxFOV;
                TPFOVSetter._increment = step;
                TPFOVSetter._minValue = minFOV;
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
        public bool SetFirstPersonFOVSettingsSettings(int maxFOV, int minFOV, int step)
        {
            try
            {
                FPFOVSetter._maxValue = maxFOV;
                FPFOVSetter._increment = step;
                FPFOVSetter._minValue = minFOV;
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
        public bool SetHandheldFOVSettingsSettings(int maxFOV, int minFOV, int step)
        {
            try
            {
                HHFOVSetter._maxValue = maxFOV;
                HHFOVSetter._increment = step;
                HHFOVSetter._minValue = minFOV;
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
                Tablet.photoTimerMaxValue = 999999;
                Tablet.photoTimerIncrementValue = time - PhotoTimer;
                Tablet.SwapPhotoTimerDelay();
                Tablet.photoTimerMaxValue = PhotoTimerMaxValue;
                Tablet.photoTimerIncrementValue = PhotoTimerIncrement;
                return true;
            }
            catch (Exception ex)
            {
                MelonLogger.Error(ex);
                return false;
            }
        }
        public bool SetPhotoTimerIncrement(int increment)
        {
            try
            {
                Tablet.photoTimerIncrementValue = increment;
                PhotoTimerIncrement = increment;
                return true;
            }
            catch (Exception ex)
            {
                MelonLogger.Error(ex);
                return false;
            }
        }
        public bool SetPhotoTimerMaxValue(int maxValue)
        {
            try
            {
                Tablet.photoTimerMaxValue = maxValue;
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
                Tablet.transform.position = position;
                Tablet.transform.rotation = rotation;
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
                CameraController.SetSelfieCameraOrientation(position, rotation);
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
                CameraController.SetSelfieCameraOrientation(position2, rotation2.eulerAngles);
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
                Camera.spawnYOffset = offset;
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
                Camera.tabletSpawnDelay = delay;
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
                CameraController._thirdPersonStabilizer.ReachTargetInstantly();
                CameraController._firstPersonStabilizer.ReachTargetInstantly();
                CameraController._selfieStabilizer.ReachTargetInstantly();
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
                if(pov == "Null")
                {
                    pov = POV;
                }
                if(fov == -1)
                {
                    fov = FOV;
                }
                if(pov == "TP")
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
                    if(positional_smoothing != -1)
                    {
                        SetThirdPersonPositionalSmooting(positional_smoothing);
                    }
                    if (rotational_smoothing != -1)
                    {
                        SetThirdPersonRotationalSmooting(rotational_smoothing);
                    }
                }
                else if(pov == "FP")
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
                else if(pov == "HH")
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
                if (((isShown % 2) == 1) != Camera.TabletVisualsAreActive) { o += 2; }
                if (Camera.TabletVisualsAreActive)
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
                isRecording = Tablet.isRecording;
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
                if (CameraController.CurrentCameraMode == Il2CppRUMBLE.Recording.LCK.Extensions.CameraMode.ThirdPerson)
                {
                    if (POV != "TP")
                    {
                        povchanged = true;
                    }
                    POV = "TP";
                }
                else if (CameraController.CurrentCameraMode == Il2CppRUMBLE.Recording.LCK.Extensions.CameraMode.FirstPerson)
                {
                    if (POV != "FP")
                    {
                        povchanged = true;
                    }
                    POV = "FP";
                }
                else if (CameraController.CurrentCameraMode == Il2CppRUMBLE.Recording.LCK.Extensions.CameraMode.Selfie)
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
                FOV = (int)CameraController.GetCurrentModeFOV();
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
                    TPCameraSettings.renderPostProcessing = value;
                }
                else if (pov == "FP")
                {
                    FPCameraSettings.renderPostProcessing = value;
                }
                else if (pov == "HH")
                {
                    HHCameraSettings.renderPostProcessing = value;
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


    }
}
