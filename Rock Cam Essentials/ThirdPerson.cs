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
using static MelonLoader.MelonLogger;
namespace Rock_Cam_Essentials
{
    //Everything related to the third person camera
    public class ThirdPerson
    {
        public Il2CppRUMBLE.Recording.LCK.Extensions.LCKCameraController _CameraController;
        public bool IsFlipped { get => _CameraController.IsThirdPersonFront; set => FlipCamera(value); }
        public float Distance { get => _CameraController._thirdPersonDistance; set => _CameraController._thirdPersonDistance = value; }
        public float DistanceMultipier { get => _CameraController._thirdPersonDistanceMultiplier; set => _CameraController._thirdPersonDistanceMultiplier = value; }
        public float Angle { get => _CameraController._thirdPersonHeightAngle; set => _CameraController._thirdPersonHeightAngle = value; }
        public UnityEngine.Rendering.Universal.UniversalAdditionalCameraData _CameraSettings;
        public LckCamera _Camera;
        public float PositionalSmoothing { get => _CameraController._selfieStabilizer.PositionalSmoothing; set => SetPositionalSmooting(value); }
        public float RotationalSmoothing { get => _CameraController._selfieStabilizer.RotationalSmoothing; set => SetRotationalSmooting(value); }
        public Il2CppRUMBLE.Recording.LCK.Extensions.LCKSettingsButtonsController _POVController;
        public Rock_Cam _rockcam;
        public ThirdPerson(Rock_Cam rockcam)
        {
            _rockcam = rockcam;
            _CameraController = rockcam._CameraController;
            _Camera = rockcam._Tablet.thirdPersonCamera;
            Distance = _CameraController._thirdPersonDistance;
            Angle = _CameraController._thirdPersonHeightAngle;
            PositionalSmoothing = _CameraController._thirdPersonStabilizer.PositionalSmoothing;
            RotationalSmoothing = _CameraController._thirdPersonStabilizer.RotationalSmoothing;
            _CameraSettings = rockcam._Tablet.thirdPersonCamera.gameObject.GetComponent<UnityEngine.Rendering.Universal.UniversalAdditionalCameraData>();
            _POVController = rockcam._POVController;

        }
        /// <summary>
        ///Sets the POV to third person mode
        /// </summary>
        public bool SetPOV()
        {
            try
            {
                _POVController.SwitchToTPMode();
                _rockcam.pov = "TP";
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
        ///Sets the angle at which the third person camera follows the player
        /// </summary>
        [Obsolete("Just set the variable directly")]
        public bool SetAngle(float angle)
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
        /// <summary>
        /// JUST SET THE VARIABLE DIRECTLY
        ///Sets the distance at which the third person camera follows the player, this value is the one that gets changed via the ingame buttons
        ///Also gets multiplied by the third person distance multiplier
        /// </summary>
        [Obsolete("Just set the variable directly")]
        public bool SetDistance(float distance)
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
        /// <summary>
        /// JUST SET THE VARIABLE DIRECTLY
        ///Sets the distance multiplier that is multiplier, that is multiplied by the distance to get the actual distance at which the camera will follow the player
        /// </summary>
        [Obsolete("Just set the variable directly")]
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
        /// <summary>
        /// JUST SET THE VARIABLE DIRECTLY
        ///Flips the third person camera, as of now very buggy
        /// </summary>
        [Obsolete("Just set the variable directly")]
        public bool FlipCamera(bool flip)
        {
            if (flip == IsFlipped)
            {
                MelonLogger.Msg("TP camera already flipped correct way");
            }
            try
            {
                _CameraController.IsThirdPersonFront = flip;
                return true;
            }
            catch (Exception ex)
            {
                MelonLogger.Error(ex);
                return false;
            }
        }
        /// <summary>
        /// JUST SET THE VARIABLE DIRECTLY
        ///Sets the positional smoothing of the third person camera
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
                _CameraController._thirdPersonStabilizer.PositionalSmoothing = smooth;
            }
            catch (Exception ex)
            {
                MelonLogger.Error(ex);
                return false;
            }
            return true;
        }
        /// <summary>
        ///Sets the rotational smoothing of the third person camera
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
                _CameraController._thirdPersonStabilizer.RotationalSmoothing = smooth;
            }
            catch (Exception ex)
            {
                MelonLogger.Error(ex);
                return false;
            }
            return true;
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
