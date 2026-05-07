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
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Playables;
using static Il2CppRootMotion.FinalIK.HitReactionVRIK;
namespace Rock_Cam_Essentials
{
    //Everything related to the handheld(selfie) camera
    public class Handheld
    {
        public Il2CppRUMBLE.Recording.LCK.Extensions.LCKCameraController _CameraController;
        public UnityEngine.Rendering.Universal.UniversalAdditionalCameraData _CameraSettings;
        public LckCamera _Camera;
        public float PositionalSmoothing { get => _CameraController._selfieStabilizer.PositionalSmoothing; set => SetPositionalSmooting(value); }
        public float RotationalSmoothing { get => _CameraController._selfieStabilizer.RotationalSmoothing; set => SetRotationalSmooting(value); }
        public Il2CppRUMBLE.Recording.LCK.Extensions.LCKSettingsButtonsController _POVController;
        public Rock_Cam _rockcam;
        public bool IsFlipped { get => _CameraController.IsSelfieFront; set => FlipCamera(value); }
        public Handheld(Rock_Cam rockcam)
        {
            _rockcam = rockcam;
            _CameraController = rockcam._CameraController;
            _Camera = rockcam._Tablet.selfieCamera;
            _CameraSettings = rockcam._Tablet.selfieCamera.gameObject.GetComponent<UnityEngine.Rendering.Universal.UniversalAdditionalCameraData>();
            _POVController = rockcam._POVController;
        }
        /// <summary>
        ///Sets the POV to handheld mode
        /// </summary>
        public bool SetPOV()
        {
            try
            {
                _POVController.SwitchToSelfieMode();
                _rockcam.pov = "HH";
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
        ///Sets the positional smoothing of the handheld camera
        /// </summary>
        [Obsolete("Just set the variable directly")]
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
        /// <summary>
        /// JUST SET THE VARIABLE DIRECTLY
        ///Sets the rotational smoothing of the handheld camera
        /// </summary>
        [Obsolete("Just set the variable directly")]
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
        /// <summary>
        ///Flips the handheld camera, very buggy
        /// </summary>
        [Obsolete("Just set the variable directly")]
        public bool FlipCamera(bool flip)
        {
            if (IsFlipped == flip)
            {
                MelonLogger.Msg("HH camera already flipped the right way");
                return true;
            }
            try
            {
                _CameraController.ProcessSelfieFlip();
                return true;
            }
            catch (Exception ex)
            {
                MelonLogger.Error(ex);
                return false;
            }
        }
        /// <summary>
        ///Sets the position of the handheld camera relative to the rockcam
        /// </summary>
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
        /// <summary>
        ///Sets the position of the handheld camera in global coords
        /// </summary>
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
}
