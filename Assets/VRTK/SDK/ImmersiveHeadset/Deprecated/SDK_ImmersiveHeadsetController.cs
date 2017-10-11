// Immersive Headset Motion Controller|SDK_ImmersiveHeadset|004
namespace VRTK
{
    //#if VRTK_DEFINE_IMMERSIVEHEADSET
    using UnityEngine;
    using System.Collections.Generic;
    using UnityEngine.XR;
    using UnityEngine.XR.WSA.Input;
    using UnityEngine.Experimental.XR;
    using System;

    //#endif

    /// <summary>
    /// The Immersive Headset Motion Controller SDK script provides a bridge to SDK methods that deal with the input devices.
    /// </summary>
    [SDK_Description(typeof(SDK_ImmersiveHeadsetSystem))]
    public class SDK_ImmersiveHeadsetController
        //#if VRTK_DEFINE_IMMERSIVEHEADSET
        : SDK_BaseController
    //#else
    //: SDK_FallbackController
    //#endif
    {
        //#if VRTK_DEFINE_IMMERSIVEHEADSET

        protected VRTK_TrackedController cachedLeftController;
        protected VRTK_TrackedController cachedRightController;

        protected struct HandSourceState
        {
            public InteractionSourceState last;
            public InteractionSourceState current;
        }

        protected HandSourceState leftHandState;
        protected HandSourceState rightHandState;


        /// <summary>
        /// This method is called just after loading the <see cref="VRTK_SDKSetup"/> that's using this SDK.
        /// </summary>
        /// <param name="setup">The SDK Setup which is using this SDK.</param>
        public override void OnAfterSetupLoad(VRTK_SDKSetup setup)
        {
            base.OnAfterSetupLoad(setup);
            InteractionManager.InteractionSourceUpdated += UpdateInteractionStates;
        }

        protected void UpdateInteractionStates(InteractionSourceUpdatedEventArgs args)
        {
            switch (args.state.source.handedness)
            {
                case InteractionSourceHandedness.Left:
                    leftHandState.last = leftHandState.current;
                    leftHandState.current = args.state;
                    break;
                case InteractionSourceHandedness.Right:
                    rightHandState.last = rightHandState.current;
                    rightHandState.current = args.state;
                    break;
            }
        }

        protected HandSourceState GetController(VRTK_ControllerReference controller)
        {
            switch (controller.hand)
            {
                case ControllerHand.None:
                    throw new ArgumentException(string.Format("invalid argument {0}", controller.hand));
                case ControllerHand.Left:
                    return leftHandState;
                case ControllerHand.Right:
                    return rightHandState;
            }
            return new HandSourceState();
        }

        protected virtual void SetTrackedControllerCaches(bool forceRefresh = false)
        {
            if (forceRefresh)
            {
                cachedLeftController = null;
                cachedRightController = null;
            }

            VRTK_SDKManager sdkManager = VRTK_SDKManager.instance;
            if (sdkManager != null)
            {
                if (cachedLeftController == null && sdkManager.loadedSetup.actualLeftController)
                {
                    cachedLeftController = sdkManager.loadedSetup.actualLeftController.GetComponent<VRTK_TrackedController>();
                    if (cachedLeftController != null)
                    {
                        cachedLeftController.index = 0;
                    }
                }
                if (cachedRightController == null && sdkManager.loadedSetup.actualRightController)
                {
                    cachedRightController = sdkManager.loadedSetup.actualRightController.GetComponent<VRTK_TrackedController>();
                    if (cachedRightController != null)
                    {
                        cachedRightController.index = 1;
                    }
                }

            }
        }

        protected virtual VRTK_TrackedController GetTrackedObject(GameObject controller)
        {
            SetTrackedControllerCaches();
            VRTK_TrackedController trackedObject = null;

            if (IsControllerLeftHand(controller))
            {
                trackedObject = cachedLeftController;
            }
            else if (IsControllerRightHand(controller))
            {
                trackedObject = cachedRightController;
            }
            return trackedObject;
        }

        public override Transform GenerateControllerPointerOrigin(GameObject parent)
        {
            return null;
        }

        /// <summary>
        /// The GetAngularVelocity method is used to determine the current angular velocity of the tracked object on the given controller reference.
        /// </summary>
        /// <param name="controllerReference">The reference to the tracked object to check for.</param>
        /// <returns>A Vector3 containing the current angular velocity of the tracked object.</returns>
        public override Vector3 GetAngularVelocity(VRTK_ControllerReference controllerReference)
        {
            if (!VRTK_ControllerReference.IsValid(controllerReference))
            {
                return Vector3.zero;
            }

            Vector3 result = Vector3.zero;
            GetController(controllerReference).current.sourcePose.TryGetAngularVelocity(out result);
            return result;
        }


        public override Vector2 GetButtonAxis(ButtonTypes buttonType, VRTK_ControllerReference controllerReference)
        {
            if (!VRTK_ControllerReference.IsValid(controllerReference))
            {
                return Vector2.zero;
            }

            InteractionSourceState input = GetController(controllerReference).current;

            switch (buttonType)
            {
                case ButtonTypes.Trigger:
                    return new Vector2(input.selectPressedAmount, 0f);
                case ButtonTypes.Touchpad:
                    return input.touchpadTouched ? input.touchpadPosition : input.thumbstickPosition;
                default:
                    return Vector2.zero;
            }
        }


        public override float GetButtonHairlineDelta(ButtonTypes buttonType, VRTK_ControllerReference controllerReference)
        {
            //TODO: This doesn't seem correct, surely this should be storing the previous button press value and getting the delta.
            return (VRTK_ControllerReference.IsValid(controllerReference) ? 0.1f : 0f);
        }

        /// <summary>
        /// The GetControllerButtonState method is used to determine if the given controller button for the given press type on the given controller reference is currently taking place.
        /// see https://developer.microsoft.com/en-us/windows/mixed-reality/gestures_and_motion_controllers_in_unity for more information
        /// </summary>
        /// <param name="buttonType">The type of button to check for the state of.</param>
        /// <param name="pressType">The button state to check for.</param>
        /// <param name="controllerReference">The reference to the controller to check the button state on.</param>
        /// <returns>Returns true if the given button is in the state of the given press type on the given controller reference.</returns>
        public override bool GetControllerButtonState(ButtonTypes buttonType, ButtonPressTypes pressType, VRTK_ControllerReference controllerReference)
        {
            if (!VRTK_ControllerReference.IsValid(controllerReference))
            {
                return false;
            }

            HandSourceState input = GetController(controllerReference);

            switch (buttonType)
            {
                case ButtonTypes.Grip:
                    switch (pressType)
                    {
                        case ButtonPressTypes.Press:
                            return input.last.grasped && input.current.grasped;
                        case ButtonPressTypes.PressDown:
                            return !input.last.grasped && input.current.grasped;
                        case ButtonPressTypes.PressUp:
                            return input.last.grasped && !input.current.grasped;
                    }
                    break;
                case ButtonTypes.StartMenu:
                    switch (pressType)
                    {
                        case ButtonPressTypes.Press:
                            return input.last.menuPressed && input.current.menuPressed;
                        case ButtonPressTypes.PressDown:
                            return !input.last.menuPressed && input.current.menuPressed;
                        case ButtonPressTypes.PressUp:
                            return input.last.menuPressed && !input.current.menuPressed;
                    }
                    break;
                case ButtonTypes.Trigger:
                    switch (pressType)
                    {
                        case ButtonPressTypes.Press:
                            return input.last.selectPressed && input.current.selectPressed;
                        case ButtonPressTypes.PressDown:
                            return !input.last.selectPressed && input.current.selectPressed;
                        case ButtonPressTypes.PressUp:
                            return input.last.selectPressed && !input.current.selectPressed;
                    }
                    break;
                case ButtonTypes.Touchpad:
                    switch (pressType)
                    {
                        case ButtonPressTypes.Press:
                            return input.last.touchpadPressed && input.current.touchpadPressed;
                        case ButtonPressTypes.PressDown:
                            return !input.last.touchpadPressed && input.current.touchpadPressed;
                        case ButtonPressTypes.PressUp:
                            return input.last.touchpadPressed && !input.current.touchpadPressed;
                        case ButtonPressTypes.Touch:
                            return input.last.touchpadTouched && input.current.touchpadTouched;
                        case ButtonPressTypes.TouchDown:
                            return !input.last.touchpadTouched && input.current.touchpadTouched;
                        case ButtonPressTypes.TouchUp:
                            return input.last.touchpadTouched && !input.current.touchpadTouched;
                    }
                    break;
            }
            return false;
        }

        public override GameObject GetControllerByIndex(uint index, bool actual = false)
        {
            SetTrackedControllerCaches();
            VRTK_SDKManager sdkManager = VRTK_SDKManager.instance;
            if (sdkManager != null)
            {
                if (cachedLeftController != null && cachedLeftController.index == index)
                {
                    return (actual ? sdkManager.loadedSetup.actualLeftController : sdkManager.scriptAliasLeftController);
                }

                if (cachedRightController != null && cachedRightController.index == index)
                {
                    return (actual ? sdkManager.loadedSetup.actualRightController : sdkManager.scriptAliasRightController);
                }
            }
            return null;
        }

        public override string GetControllerDefaultColliderPath(ControllerHand hand)
        {
            return "ControllerColliders/Fallback";
        }

        public override string GetControllerElementPath(ControllerElements element, ControllerHand hand, bool fullPath = false)
        {
            return null;
        }

        public override uint GetControllerIndex(GameObject controller)
        {
            VRTK_TrackedController trackedObject = GetTrackedObject(controller);
            return (trackedObject != null ? trackedObject.index : uint.MaxValue);
        }

        public override GameObject GetControllerLeftHand(bool actual = false)
        {
            GameObject controller = GetSDKManagerControllerLeftHand(actual);
            if (controller == null && actual)
            {
                controller = null; // TODO
            }
            return controller;
        }

        protected override GameObject GetControllerModelFromController(GameObject controller)
        {
            return GetControllerModel(VRTK_DeviceFinder.GetControllerHand(controller));
        }

        public override GameObject GetControllerModel(GameObject controller)
        {
            return GetControllerModelFromController(controller);
        }

        public override GameObject GetControllerModel(ControllerHand hand)
        {
            GameObject model = GetSDKManagerControllerModelForHand(hand);
            if (model == null)
            {
                GameObject avatarObject = null; // = GetHeadset();
                switch (hand)
                {
                    case ControllerHand.Left:
                        if (avatarObject != null)
                        {
                            model = avatarObject.transform.Find("controller_left").gameObject;
                        }
                        else
                        {
                            model = GetControllerLeftHand(true);
                            model = (model != null && model.transform.childCount > 0 ? model.transform.GetChild(0).gameObject : null);
                        }
                        break;
                    case ControllerHand.Right:
                        if (avatarObject != null)
                        {
                            model = avatarObject.transform.Find("controller_right").gameObject;
                        }
                        else
                        {
                            model = GetControllerRightHand(true);
                            model = (model != null && model.transform.childCount > 0 ? model.transform.GetChild(0).gameObject : null);
                        }
                        break;
                }
            }
            return model;
        }

        public override Transform GetControllerOrigin(VRTK_ControllerReference controllerReference)
        {
            return VRTK_SDK_Bridge.GetPlayArea();
        }

        public override GameObject GetControllerRenderModel(VRTK_ControllerReference controllerReference)
        {
            // TODO
            return null;
        }

        public override GameObject GetControllerRightHand(bool actual = false)
        {
            GameObject controller = GetSDKManagerControllerRightHand(actual);
            if (controller == null && actual)
            {
                controller = null; // TODO
            }
            return controller;
        }

        public override ControllerType GetCurrentControllerType()
        {
            return ControllerType.ImmersiveHeadset_Motioncontroller;
        }

        public override SDK_ControllerHapticModifiers GetHapticModifiers()
        {
            SDK_ControllerHapticModifiers modifiers = new SDK_ControllerHapticModifiers();
            modifiers.durationModifier = 0.8f;
            modifiers.intervalModifier = 1f;
            return modifiers;
        }

        public override Vector3 GetVelocity(VRTK_ControllerReference controllerReference)
        {
            throw new NotImplementedException();
        }

        public override void HapticPulse(VRTK_ControllerReference controllerReference, float strength = 0.5F)
        {
            throw new NotImplementedException();
        }

        public override bool HapticPulse(VRTK_ControllerReference controllerReference, AudioClip clip)
        {
            throw new NotImplementedException();
        }

        public override bool IsControllerLeftHand(GameObject controller)
        {
            throw new NotImplementedException();
        }

        public override bool IsControllerLeftHand(GameObject controller, bool actual)
        {
            throw new NotImplementedException();
        }

        public override bool IsControllerRightHand(GameObject controller)
        {
            throw new NotImplementedException();
        }

        public override bool IsControllerRightHand(GameObject controller, bool actual)
        {
            throw new NotImplementedException();
        }

        public override bool IsTouchpadStatic(bool isTouched, Vector2 currentAxisValues, Vector2 previousAxisValues, int compareFidelity)
        {
            throw new NotImplementedException();
        }

        public override void ProcessFixedUpdate(VRTK_ControllerReference controllerReference, Dictionary<string, object> options)
        {
            throw new NotImplementedException();
        }

        public override void ProcessUpdate(VRTK_ControllerReference controllerReference, Dictionary<string, object> options)
        {
            throw new NotImplementedException();
        }

        public override void SetControllerRenderModelWheel(GameObject renderModel, bool state)
        {
            throw new NotImplementedException();
        }

        //#endif
    }
}
