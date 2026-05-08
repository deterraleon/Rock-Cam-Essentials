using Il2CppLiv.Lck;
using Il2CppLiv.Lck.Recorder;
using Il2CppLiv.Lck.UI;
using Il2CppRUMBLE.Interactions.InteractionBase.Extensions;
using Il2CppRUMBLE.Managers;
using Il2CppRUMBLE.Players.Subsystems;
using Il2CppRUMBLE.Recording.LCK;
using Il2CppSystem.IO;
using MelonLoader;
using RumbleModdingAPI;
using System;
using System.Collections;
using System.ComponentModel;

//using RumbleModUI;
using UnityEngine;
using UnityEngine.Playables;
using static Il2CppRootMotion.FinalIK.HitReactionVRIK;
namespace Rock_Cam_Essentials
{
    public static class BuildInfo
    {
        public const string ModName = "RockCamEssentials";
        public const string ModVersion = "2.0.0";
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
            //RumbleModdingAPI.RMAPI.Actions.onMapInitialized += mapLoaded;
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
    }
    //A unility that allows for easier modding of the rockcam
    //The functions of the main class the actions you can do with the rockcam, while subclasses are mainly used for settings
    public class Rock_Cam
    {
        public ThirdPerson thirdPerson;
        public FirstPerson firstPerson;
        public Handheld handheld;
        public HandleBars handleBars;
        public Il2CppRUMBLE.Recording.LCK.Extensions.LckDoubleButton _FOVSetter;
        public int FOV { get => (int)_CameraController.GetCurrentModeFOV(); set => SetFOV(value); }
        public int PhotoTimer { get => _Tablet.photoTimerCurrentValue; set => SetPhotoTimerValue(value); }
        public int PhotoTimerIncrement { get => _Tablet.photoTimerIncrementValue; set => _Tablet.photoTimerIncrementValue = value; }
        public int PhotoTimerMaxValue { get => _Tablet.photoTimerMaxValue; set => _Tablet.photoTimerMaxValue = value; }
        public bool IsRecording { get => _GetRecordingStatus(); set => SetRecordingStatus(value); }
        public int DetachedPreview { get => _DetachedPreviewManager.ActivePreviewNo; set => _Tablet.lckDetachedPreview.SwitchPreview(value);  }
        public bool detachedPreviewChanged = false; 
        public int MaxDespawnDistance { get => _Camera.maxDespawnDistance; set => _Camera.maxDespawnDistance = value; }
        public float SpawnYOffset { get => _Camera.spawnYOffset; set => _Camera.spawnYOffset = value; }
        public float TabletSpawnDelay { get => _Camera.tabletSpawnDelay; set => _Camera.tabletSpawnDelay = value; }
        public float MaximumRenderDistance { get => _Tablet.maximumRenderDistance; set => _Tablet.maximumRenderDistance = value; }
        public int MaxFOV { get => _FOVSetter._maxValue; set => _FOVSetter._maxValue = value; }
        public int MinFOV { get => _FOVSetter._minValue; set => _FOVSetter._minValue = value; }
        public int FOVStep { get => _FOVSetter._increment; set => _FOVSetter._increment = value; }
        public int _oldDetachedPreview = 0;
        public PlayerLIV _Camera;
        public LCKTabletUtility _Tablet;
        public Il2CppRUMBLE.Recording.LCK.Extensions.LCKCameraController _CameraController;
        public Il2CppRUMBLE.Recording.LCK.Extensions.LCKSettingsButtonsController _POVController;
        public Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppReferenceArray<Transform> DetachedMonitors;
        public Transform RockCamTransform;
        public ILckService _LckService;
        public int isShown = 1;
        public string pov = "HH";
        public bool povChanged = false;
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

        private IEnumerator innertester()
        {
            TabletSpawnDelay = 5;
            Melon<Main>.Instance.LoggerInstance.Msg("Commensing part one of the general RCE test, firstly check that the tablet takes 5 seconds to appear after getting despawned");
            while(isShown%2 == 1)
            {
                IsShownUpdate();
                yield return null;
            }
            while(isShown%2 == 0)
            {
                IsShownUpdate();
                yield return null;
            }
            Melon<Main>.Instance.LoggerInstance.Msg("check that FOV is at 10, increasing the FOV makes it go to 110 and you can't increase it further," +
                " decreasing the fov makes it go to 10, then to 5");
            Melon<Main>.Instance.LoggerInstance.Msg("Check that photo timer is at 15, pressing it makes it go to 18, then to 0");
            MaxFOV = 110;
            FOVStep = 100;
            MinFOV = 5;
            FOV = 10;
            PhotoTimerMaxValue = 19;
            PhotoTimerIncrement = 3;
            PhotoTimer = 15;
            Melon<Main>.Instance.LoggerInstance.Msg("Check that detached preview is at the latest one, press the detached preview button to go to the next step");
            DetachedPreview = 5;
            while(detachedPreviewChanged == false)
            {
                UpdateDetachedPreview();
                yield return null;
            }
            TakePhoto();
            Melon<Main>.Instance.LoggerInstance.Msg("Assert that the photo has been taken, start a recording to go to the next step");
            while(IsRecording == false) { yield return null; }
            yield return new WaitForSeconds(2f);
            IsRecording = false;
            Melon<Main>.Instance.LoggerInstance.Msg("Assert that the recording has ended after 2 seconds");
            MaxDespawnDistance = 10000;
            Melon<Main>.Instance.LoggerInstance.Msg("Assert that the tablet takes despawns instead of teleporting to you if you are far away");
            while (isShown%2 == 1)
            {
                IsShownUpdate();
                yield return null;
            }
            SpawnYOffset = 10;
            Melon<Main>.Instance.LoggerInstance.Msg("Assert that the camera spawns rly high");
            while (isShown % 2 == 0)
            {
                IsShownUpdate();
                yield return null;
            }
            ShowTablet();
            yield return new WaitForSeconds(2);
            HideTablet();
            yield return new WaitForSeconds(2); 
            ShowTablet();
            Melon<Main>.Instance.LoggerInstance.Msg("Assert that the tablet was shown, then hidden, then shown again in 2 second intervals");
            yield return new WaitForSeconds(2);
            SetGlobalPositionalSmoothing(1);
            SetGlobalRotationalSmoothing(0.9f);
            handheld.SetPOV();
            Melon<Main>.Instance.LoggerInstance.Msg("Assert that the camera is in handheld," +
                " while not following your hand movements and turning very slowly, the next step will begin when you switch the pov");
            while(pov == "HH")
            {
                POVUpdate();
                yield return null;
            }
            if(povChanged == false)
            {
                Melon<Main>.Instance.LoggerInstance.Warning("povChanged is false, should be true");
            }
            ResetCameraPosition();
            handheld.SetPOV();
            Melon<Main>.Instance.LoggerInstance.Msg("Assert that the camera has snapped to the tablet");
            MaximumRenderDistance = 0;
            Melon<Main>.Instance.LoggerInstance.Msg("Assert that the image on rockcam has stopped updating, switch to first person to go to first person testing");
            POVUpdate();
            while(pov != POVs.FirstPerson)
            {
                POVUpdate();
                yield return null;
            }
            Melon<Main>.Instance.LoggerInstance.Msg("This concludes the general RCE test.");
            Melon<Main>.Instance.LoggerInstance.Msg("Assert that the camera doesn't move and rotates slowly");
            Melon<Main>.Instance.LoggerInstance.Msg("To go to the next step switch to any other pov");
            POVUpdate();
            while(pov == POVs.FirstPerson)
            {
                POVUpdate();
                yield return null;
            }
            yield return new WaitForSeconds(1.5f);
            firstPerson.SetPOV();
            Melon<Main>.Instance.LoggerInstance.Msg("Assert that the camera switched to first person after 1.5 seconds");
            Melon<Main>.Instance.LoggerInstance.Msg("This concludes first person testing");
            Melon<Main>.Instance.LoggerInstance.Msg("Switch to handheld pov to go to the next step");
            POVUpdate();
            while(pov != POVs.Handheld)
            {
                POVUpdate(); 
                yield return null;
            }
            Melon<Main>.Instance.LoggerInstance.Msg("Assert that the camera doesn't move and rotates slowly");
            Melon<Main>.Instance.LoggerInstance.Msg("To go to the next step switch to any other pov");
            POVUpdate();
            while (pov == POVs.Handheld)
            {
                POVUpdate();
                yield return null;
            }
            yield return new WaitForSeconds(1.5f);
            handheld.SetPOV();
            Melon<Main>.Instance.LoggerInstance.Msg("Assert that the camera switched to first person after 1.5 seconds");
            handheld.PositionalSmoothing = 0;
            handheld.SetRelativeCameraPosition(Vector3.one * 5, Vector3.zero);
            Melon<Main>.Instance.LoggerInstance.Msg("Assert that the camera moves normally and is also offset from the tablet");
            Melon<Main>.Instance.LoggerInstance.Msg("To go to the next step switch to any other pov");
            POVUpdate();
            while (pov == POVs.Handheld)
            {
                POVUpdate();
                yield return null;
            }
            yield return new WaitForSeconds(0.75f);
            handheld.SetPOV();
            handheld.PositionalSmoothing = 1;
            handheld.SetGlobalCameraPosition(Vector3.zero, Quaternion.identity);
            Melon<Main>.Instance.LoggerInstance.Msg("Assert that the camera is at 0,0,0");
            Melon<Main>.Instance.LoggerInstance.Msg("To go to the next step switch to any other pov");
            POVUpdate();
            while (pov == POVs.Handheld)
            {
                POVUpdate();
                yield return null;
            }
            yield return new WaitForSeconds(0.75f);
            handheld.SetPOV();
            handheld.SetRelativeCameraPosition(Vector3.zero, Vector3.zero);
            handheld.IsFlipped = true;
            Melon<Main>.Instance.LoggerInstance.Msg("Assert that the camera is flipped");
            Melon<Main>.Instance.LoggerInstance.Msg("This concludes the handheld tests");
            Melon<Main>.Instance.LoggerInstance.Msg("To go to the next step switch to any other pov");
            POVUpdate();
            while (pov == POVs.Handheld)
            {
                POVUpdate();
                yield return null;
            }
            yield return new WaitForSeconds(1.5f);
            thirdPerson.SetPOV();
            Melon<Main>.Instance.LoggerInstance.Msg("Assert that the camera is in third person and isn't moving and very slowly turns");
            thirdPerson.Distance = 5;
            thirdPerson.Angle = 50;
            thirdPerson.DistanceMultipier = 5;
            Melon<Main>.Instance.LoggerInstance.Msg("Assert that the camera is kinda far, at a steep angle and changing the distance setting greately affects the camera distance");
            Melon<Main>.Instance.LoggerInstance.Msg("To go to the next step switch to any other pov");
            POVUpdate();
            while (pov == POVs.ThirdPerson)
            {
                POVUpdate();
                yield return null;
            }
            yield return new WaitForSeconds(0.75f);
            thirdPerson.SetPOV();
            thirdPerson.IsFlipped = true;
            Melon<Main>.Instance.LoggerInstance.Msg("Assert that the camera is flipped");
            Melon<Main>.Instance.LoggerInstance.Msg("This concludes the third person tests");
            yield break;
        }

       public void GeneralTest()
        {
            MelonCoroutines.Start(innertester());
        }

        /// <summary>
        /// Basically just redefines every variable
        /// </summary>
        public bool Fix()
        {
            try
            {
                _Camera = PlayerManager.instance.localPlayer.Controller.GetComponentInChildren<PlayerLIV>();
                _Tablet = _Camera.LckTablet;
                MaximumRenderDistance = _Tablet.maximumRenderDistance;
                _CameraController = _Tablet.LckCameraController;
                _FOVSetter = _CameraController.ThirdPersonFOVDoubleButton;
                _POVController = _CameraController._settingsButtonsController;
                thirdPerson = new ThirdPerson(this);
                firstPerson = new FirstPerson(this);
                handheld = new Handheld(this);
                handleBars = new HandleBars(this);
                DetachedMonitors = _CameraController._monitorTransforms;
                PhotoTimer = _Tablet.photoTimerCurrentValue;
                PhotoTimerIncrement = _Tablet.photoTimerIncrementValue;
                PhotoTimerMaxValue = _Tablet.photoTimerMaxValue;
                RockCamTransform = _Tablet.transform;
                if (_Camera.TabletVisualsAreActive) isShown = 1;
                else isShown = 0;
                POVUpdate();
                povChanged = false;
                _LckService = _CameraController._lckService;
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
        /// <summary>
        ///Sets the settings settings, aka the bounds for the FOV setting ingame, for the third person camera
        /// </summary>
        [Obsolete("Just set the variable directly")]
        public bool SetFOVSettingsSettings(int maxFOV, int minFOV, int step)
        {
            try
            {
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
        /// <summary>
        /// DO NOT USE, JUST SET THE VARIABLE DIRECTLY
        ///Sets the FOV of the third person camera, written in a way where it will also update the ingame values shown
        /// </summary>
        [Obsolete("Just set the variable directly")]
        public bool SetFOV(int fov)
        {
            try
            {
                _FOVSetter._maxValue = 999999;
                _FOVSetter._increment = fov - FOV;
                _FOVSetter.OnPressDownIncrease();
                _FOVSetter.OnPressUpIncrease();
                _FOVSetter._maxValue = MaxFOV;
                _FOVSetter._increment = FOVStep;
                return true;
            }
            catch (Exception ex)
            {
                MelonLogger.Error(ex);
                return false;
            }
        }
        /// <summary>
        ///Takes a photo
        /// </summary>
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
        /// <summary>
        ///Hides the tablet, same function used when trying to summon the table while being close to it
        /// </summary>
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
        /// <summary>
        ///Shows the tablet, same function used when trying to summon it regularly
        ///During the spawn delay the tablet is not active, which could cause issues when trying to modify its position continuously, where it kinda just freezes for a moment.
        /// </summary>
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
        /// <summary>
        /// JUST SET THE VARIABLE DIRECTLY
        ///Starts the recording, won't do anything if already recording
        /// </summary> 
        [Obsolete("Just set the variable direclty")]
        public bool StartRecording()
        {
            if (!IsRecording)
            {
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
        /// <summary>
        /// JUST SET THE VARIALBE DIRECTLY
        ///Ends the recording, won't do anything if already not recording
        /// </summary>
        [Obsolete("Just set the variable direclty")]
        public bool EndRecording()
        {
            if (IsRecording)
            {
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
        /// <summary>
        /// Toggles recording status
        /// </summary>
        public bool ToggleRecording()
        {
            try
            {
                _Tablet.ToggleRecordingAndUpdateRecordStatus();
                return true;
            }
            catch (Exception e)
            {
                MelonLogger.Error(e);
                return false;
            }
        }
        /// <summary>
        /// JUST SET THE VARIABLE DIRECTLY(IsRecording)
        ///Sets the recording status to the value
        /// </summary>
        [Obsolete("Just set the variable directly")]
        public bool SetRecordingStatus(bool value)
        {
            try
            {
                if (value == IsRecording)
                {
                    return true;
                }
                if (value) EndRecording();
                else StartRecording();
                return true;
            }
            catch (Exception e)
            {
                MelonLogger.Error(e);
                return false;
            }
        }
        /// <summary>
        ///Sets the rotational smoothing for all cameras
        /// </summary>
        public bool SetGlobalRotationalSmoothing(float smooth)
        {
            thirdPerson.RotationalSmoothing = smooth;
            firstPerson.RotationalSmoothing = smooth;
            handheld.RotationalSmoothing = smooth;
            return true;
        }
        /// <summary>
        ///Sets the positional smoothing for all cameras
        /// </summary>
        public bool SetGlobalPositionalSmoothing(float smooth)
        {
            thirdPerson.PositionalSmoothing = smooth;
            firstPerson.PositionalSmoothing = smooth;
            handheld.PositionalSmoothing = smooth;
            return true;
        }
        /// <summary>
        ///Makes the camera instantly reach its destination ignoring smoothing, no idea why i wrote the same function twice lmao
        /// </summary>
        public bool ResetCameraPosition()
        {
            try
            {
                if (pov == "TP")
                {
                    _CameraController._thirdPersonStabilizer.ReachTargetInstantly();
                }
                if (pov == "FP")
                {
                    _CameraController._firstPersonStabilizer.ReachTargetInstantly();
                }
                if (pov == "HH")
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
        /// <summary>
        ///Sets the distance at which the camera screen will update the camera image, deprecated
        /// </summary>
        [Obsolete("Just set the variable directly")]
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
        [Obsolete("Just set the variable directly")]
        /// <summary>
        /// Deprecated, update the variable directly.
        /// Sets the detached preview
        /// </summary>
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
        /// <summary>
        /// JUST SET THE VARIABLE DIRECTLY
        /// </summary>
        [Obsolete("Just set the variable directly")]
        public bool SetPhotoTimerValue(int time)
        {
            try
            {
                int oldmaxtimervalue = PhotoTimerMaxValue;
                int oldincrement = PhotoTimerIncrement;
                PhotoTimerMaxValue = 999999;
                PhotoTimerIncrement = time - PhotoTimer;
                _Tablet.SwapPhotoTimerDelay();
                PhotoTimerMaxValue = oldmaxtimervalue;
                PhotoTimerIncrement = oldincrement;
                return true;
            }
            catch (Exception ex)
            {
                MelonLogger.Error(ex);
                return false;
            }
        }
        /// <summary>
        ///Sets the increment by which it will increase the photo timer upon pressing the ingame button
        /// </summary>
        [Obsolete("Just set the variable directly")]
        public bool SetPhotoTimerIncrement(int increment)
        {
            try
            {
                _Tablet.photoTimerIncrementValue = increment;
                return true;
            }
            catch (Exception ex)
            {
                MelonLogger.Error(ex);
                return false;
            }
        }
        /// <summary>
        ///Sets the max value of the ingame photo timer, upon exceeding the max value the photo timer will drop to 0.
        /// </summary> 
        [Obsolete("Just set the variable directly")]
        public bool SetPhotoTimerMaxValue(int maxValue)
        {
            try
            {
                _Tablet.photoTimerMaxValue = maxValue;
                return true;
            }
            catch (Exception ex)
            {
                MelonLogger.Error(ex);
                return false;
            }
        }
        /// <summary>
        ///Literally just changes the transform of the rockcam object
        /// </summary>
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
        /// <summary>
        ///Set the y offset upon spawn
        /// </summary>
        [Obsolete("Just set the variable directly")]
        public bool SetSpawnYOffset(float offset)
        {
            try
            {
                _Camera.spawnYOffset = offset;
                return true;
            }
            catch (Exception ex)
            {
                MelonLogger.Error(ex);
                return false;
            }
        }
        /// <summary>
        ///Set for how long the camera isn't active upon spawning
        /// </summary>
        [Obsolete("Just set the variable directly")]
        public bool SetRockCamSpawnDelay(float delay)
        {
            try
            {
                _Camera.tabletSpawnDelay = delay;
                return true;
            }
            catch (Exception ex)
            {
                MelonLogger.Error(ex);
                return false;
            }
        }
        /// <summary>
        ///Makes the camera instantly reach its destination ignoring smoothing, no idea why i wrote the same function twice lmao
        /// </summary>
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
        /// <summary>
        ///A macros to just set a bunch of variables at once
        /// </summary>
        public bool FullCameraSetup(string pov = "Null", float positional_smoothing = -1, float rotational_smoothing = -1, int fov = -1,
            float TPCameraDistance = -1, float TPCameraAngle = -1)
        {
            try
            {
                FOV = fov;
                if (pov == "Null")
                {
                    pov = this.pov;
                }
                if (fov == -1)
                {
                    fov = FOV;
                }
                if (pov == "TP")
                {
                    thirdPerson.SetPOV();
                    if (TPCameraDistance == -1)
                    {
                        TPCameraDistance = thirdPerson.Distance;
                    }
                    thirdPerson.Distance = TPCameraDistance;
                    if (TPCameraAngle != -1)
                    {
                        thirdPerson.Angle = TPCameraAngle;
                    }
                    if (positional_smoothing != -1)
                    {
                        thirdPerson.PositionalSmoothing = positional_smoothing;
                    }
                    if (rotational_smoothing != -1)
                    {
                        thirdPerson.RotationalSmoothing = rotational_smoothing;
                    }
                }
                else if (pov == "FP")
                {
                    firstPerson.SetPOV();
                    if (positional_smoothing != -1)
                    {
                        firstPerson.PositionalSmoothing = positional_smoothing;
                    }
                    if (rotational_smoothing != -1)
                    {
                        firstPerson.RotationalSmoothing = rotational_smoothing;
                    }
                }
                else if (pov == "HH")
                {
                    handheld.SetPOV();
                    if (positional_smoothing != -1)
                    {
                        handheld.PositionalSmoothing = positional_smoothing;
                    }
                    if (rotational_smoothing != -1)
                    {
                        handheld.RotationalSmoothing = rotational_smoothing;
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
        /// <summary>
        ///This function is required to be run every frame if you want the isShown variable to be accurate.
        ///This variable will be 0 if the camera isn't shown, 1 if it became hidden just now, 2 if it shown and 3 if it became shown just now
        /// </summary>
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
        /// <summary>
        /// Gets the recording status, just get the variable directly
        /// </summary>
        public bool _GetRecordingStatus()
        {
            try
            {
                return !(_Tablet.recordLightCoroutine == null);
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        ///This function is required to be run every frame if you want the POVChanged to be accurate.
        ///The function updates the POV and POVChanged variable.
        ///POVChanged will be true iff the POV is different from what it was on the previous frame.
        /// </summary>
        public bool POVUpdate()
        {
            try
            {
                bool povchanged = false;
                if (_CameraController.CurrentCameraMode == Il2CppRUMBLE.Recording.LCK.Extensions.CameraMode.ThirdPerson)
                {
                    if (pov != "TP")
                    {
                        povchanged = true;
                    }
                    pov = "TP";
                }
                else if (_CameraController.CurrentCameraMode == Il2CppRUMBLE.Recording.LCK.Extensions.CameraMode.FirstPerson)
                {
                    if (pov != "FP")
                    {
                        povchanged = true;
                    }
                    pov = "FP";
                }
                else if (_CameraController.CurrentCameraMode == Il2CppRUMBLE.Recording.LCK.Extensions.CameraMode.Selfie)
                {
                    if (pov != "HH")
                    {
                        povchanged = true;
                    }
                    pov = "HH";
                }
                else
                {
                    MelonLogger.Error("Error determining the POV");
                    return false;
                }
                povChanged = povchanged;
                return true;
            }
            catch (Exception ex)
            {
                MelonLogger.Error(ex);
                return false;
            }
        }
        /// <summary>
        ///Returns the spawn position the camera would have if it were spawned right now
        /// </summary>
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
        /// <summary>
        ///Returns the spawn rotation the camera would have if it were spawned right now
        /// </summary>
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
        public bool SetResolutionDEPRICATED(uint width, uint height)
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
        public bool SetVideoBitrateDEPRICATED(uint bitrate)
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
        public bool SetAudioBitrateDEPRICATED(uint bitrate)
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
        public bool SetFramerateDEPRICATED(uint framerate)
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
        /// <summary>
        ///No idea what this does, maybe mic volume?
        /// </summary>
        public bool SetMicrophoneGainUntestedDEPRICATED(float gain)
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
        /// <summary>
        ///Macros to set up several parameters at once
        /// </summary>
        public bool FullRecordingSetupTempDEPRICATED(uint width = 1920, uint height = 1080, uint framerate = 60, uint videoBitrate = 10485760, uint audioBitrate = 1048576)
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
        public bool FullRecordingSetupTempDEPRICATED(RecordingSettings recordingSettings)
        {
            try
            {
                return FullRecordingSetupTempDEPRICATED(recordingSettings.width, recordingSettings.height,
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
                qualityOption.RecordingCameraTrackDescriptor = settings;
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
        /// <summary>
        ///Probably controls whether the mic is recorded or not, but haven't checked yet
        /// </summary>
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
        /// <summary>
        ///Using some voodoo magic to set the pov without the usual function that does that, could result in weird behaviour
        /// </summary>
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
                    pov = this.pov;
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
        /// <summary>
        ///This function is required to be run every frame for the DetachedPreviewChanged variable to be accurate.
        ///Updates the DetachedPreviewChanged variable
        /// </summary>
        public bool UpdateDetachedPreview()
        {
            try
            {
                if (DetachedPreview != _oldDetachedPreview) detachedPreviewChanged = true;
                else detachedPreviewChanged = false;
                _oldDetachedPreview = DetachedPreview;
                return true;
            }
            catch (Exception ex)
            {
                MelonLogger.Error(ex);
                return false;
            }
        }
        /// <summary>
        ///Recording settings for a camera in the current mode
        /// </summary> 
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
        /// <summary>
        ///Recording settings for a camera
        /// </summary>
        public RecordingSettings GetRecordingSettingsDEPRICATEDKINDA()
        {
            try
            {
                return new();
                //return CameraTrackDescriptorToRecordingSettings(_CameraController.GetDescriptorForCurrentOrientation());
            }
            catch (Exception ex)
            {
                MelonLogger.Error(ex);
                return new();
            }
        }
        /// <summary>
        ///Convert the CameraTrackDescriptor type(the one used ingame) to RecordingSettings(type defined within RCE), to allow for easier usage
        /// </summary>
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
