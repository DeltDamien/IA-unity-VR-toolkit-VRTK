namespace VRTK
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    using UnityEngine.XR.WSA.Input;

    [SDK_Description(typeof(SDK_WindowsMR))]
    public class SDK_WindowsMRController : SDK_BaseController
    {
        protected WindowsMR_TrackedObject cachedLeftTrackedObject;
        protected WindowsMR_TrackedObject cachedRightTrackedObject;

        protected Dictionary<GameObject, WindowsMR_TrackedObject> cachedTrackedObjectsByGameObject = new Dictionary<GameObject, WindowsMR_TrackedObject>();
        protected Dictionary<uint, WindowsMR_TrackedObject> cachedTrackedObjectsByIndex = new Dictionary<uint, WindowsMR_TrackedObject>();

        #region Overriden base functions
        public override Transform GenerateControllerPointerOrigin(GameObject parent)
        {
            //TODO: Implement
            return null;
        }

        public override Vector3 GetAngularVelocity(VRTK_ControllerReference controllerReference)
        {
            if (!VRTK_ControllerReference.IsValid(controllerReference))
            {
                return Vector3.zero;
            }

            uint index = VRTK_ControllerReference.GetRealIndex(controllerReference);
            WindowsMR_TrackedObject device = GetControllerByIndex(index).GetComponent<WindowsMR_TrackedObject>();
            return device.AngularVelocity;
        }

        public override Vector2 GetButtonAxis(ButtonTypes buttonType, VRTK_ControllerReference controllerReference)
        {
            uint index = VRTK_ControllerReference.GetRealIndex(controllerReference);
            WindowsMR_TrackedObject device = GetControllerByIndex(index, true).GetComponent<WindowsMR_TrackedObject>();

            switch (buttonType)
            {
                case ButtonTypes.Touchpad:
                    if(device.GetAxis(InteractionSourcePressType.Touchpad) == Vector2.zero)
                    {
                        return device.GetAxis(InteractionSourcePressType.Thumbstick);
                    }
                    return device.GetAxis(InteractionSourcePressType.Touchpad);
                case ButtonTypes.Thumbstick:
                    return device.GetAxis(InteractionSourcePressType.Thumbstick);
            }

            return Vector2.zero;
        }

        public override float GetButtonHairlineDelta(ButtonTypes buttonType, VRTK_ControllerReference controllerReference)
        {
            //TODO: Implement
            return 0;
        }

        public override bool GetControllerButtonState(ButtonTypes buttonType, ButtonPressTypes pressType, VRTK_ControllerReference controllerReference)
        {
            if (!VRTK_ControllerReference.IsValid(controllerReference))
            {
                return false;
            }

            uint index = VRTK_ControllerReference.GetRealIndex(controllerReference);

            switch (buttonType)
            {
                case ButtonTypes.Trigger:
                    return IsButtonPressed(index, pressType, InteractionSourcePressType.Select);
                case ButtonTypes.TriggerHairline:
                    WindowsMR_TrackedObject device = GetControllerByIndex(index, true).GetComponent<WindowsMR_TrackedObject>();

                    if (pressType == ButtonPressTypes.PressDown)
                    {
                        return device.GetHairTriggerDown();
                    }
                    else if (pressType == ButtonPressTypes.PressUp)
                    {
                        return device.GetHairTriggerUp();
                    }
                    break;
                case ButtonTypes.Grip:
                    return IsButtonPressed(index, pressType, InteractionSourcePressType.Grasp);
                case ButtonTypes.Touchpad:
                    return IsButtonPressed(index, pressType, InteractionSourcePressType.Touchpad);
                case ButtonTypes.ButtonOne:
                    //return IsButtonPressed(index, pressType, (1ul << (int)EVRButtonId.k_EButton_A));
                    return false;
                case ButtonTypes.ButtonTwo:
                    //return IsButtonPressed(index, pressType, InteractionSourcePressType.Menu);
                    return false;
                case ButtonTypes.StartMenu:
                    return IsButtonPressed(index, pressType, InteractionSourcePressType.Menu);
            }
            return false;
        }

        public override GameObject GetControllerByIndex(uint index, bool actual = false)
        {
            SetTrackedControllerCaches();
            if (index < uint.MaxValue)
            {
                VRTK_SDKManager sdkManager = VRTK_SDKManager.instance;
                if (sdkManager != null)
                {
                    if (cachedLeftTrackedObject != null && (uint)cachedLeftTrackedObject.Index == index)
                    {
                        return (actual ? sdkManager.loadedSetup.actualLeftController : sdkManager.scriptAliasLeftController);
                    }
                    
                    if (cachedRightTrackedObject != null && (uint)cachedRightTrackedObject.Index == index)
                    {
                        return (actual ? sdkManager.loadedSetup.actualRightController : sdkManager.scriptAliasRightController);
                    }
                }

                if (cachedTrackedObjectsByIndex.ContainsKey(index) && cachedTrackedObjectsByIndex[index] != null)
                {
                    return cachedTrackedObjectsByIndex[index].gameObject;
                }
            }

            return null;
        }

        public override string GetControllerDefaultColliderPath(ControllerHand hand)
        {
            //TODO: Implement
            return "";
        }

        public override string GetControllerElementPath(ControllerElements element, ControllerHand hand, bool fullPath = false)
        {
            //TODO: Implement
            return "";
        }

        public override uint GetControllerIndex(GameObject controller)
        {
            WindowsMR_TrackedObject trackedObject = GetTrackedObject(controller);
            return (trackedObject != null ? (uint)trackedObject.Index : uint.MaxValue);
        }

        public override GameObject GetControllerLeftHand(bool actual = false)
        {
            GameObject controller = GetSDKManagerControllerLeftHand(actual);
            if (controller == null && actual)
            {
                controller = VRTK_SharedMethods.FindEvenInactiveGameObject<WindowsMR_ControllerManager>("Controller (left)");
            }
            return controller;
        }

        public override GameObject GetControllerModel(GameObject controller)
        {
            //TODO: Implement
            return null;
        }

        public override GameObject GetControllerModel(ControllerHand hand)
        {
            GameObject model = GetSDKManagerControllerModelForHand(hand);
            if (model == null)
            {
                GameObject controller = null;
                switch (hand)
                {
                    case ControllerHand.Left:
                        controller = GetControllerLeftHand(true);
                        break;
                    case ControllerHand.Right:
                        controller = GetControllerRightHand(true);
                        break;
                }

                if (controller != null)
                {
                    model = controller.transform.GetChild(0).gameObject;
                }
            }
            return model;
        }

        public override Transform GetControllerOrigin(VRTK_ControllerReference controllerReference)
        {
            //TODO: Implement
            return null;
        }

        public override GameObject GetControllerRenderModel(VRTK_ControllerReference controllerReference)
        {
            //TODO: Implement
            return null;
        }

        public override GameObject GetControllerRightHand(bool actual = false)
        {
            GameObject controller = GetSDKManagerControllerRightHand(actual);
            if (controller == null && actual)
            {
                controller = VRTK_SharedMethods.FindEvenInactiveGameObject<WindowsMR_ControllerManager>("Controller (right)");
            }
            return controller;
        }

        public override ControllerType GetCurrentControllerType()
        {
            // TODO: Implement
            return ControllerType.WindowsMR_MotionController;
        }

        public override SDK_ControllerHapticModifiers GetHapticModifiers()
        {
            // TODO: Implement
            SDK_ControllerHapticModifiers modifiers = new SDK_ControllerHapticModifiers();
            return modifiers;
        }

        public override Vector3 GetVelocity(VRTK_ControllerReference controllerReference)
        {
            // TODO: Implement
            return Vector3.zero;
        }

        public override void HapticPulse(VRTK_ControllerReference controllerReference, float strength = 0.5F)
        {
            // TODO: Implement
        }

        public override bool HapticPulse(VRTK_ControllerReference controllerReference, AudioClip clip)
        {
            // TODO: Implement
            return false;
        }

        public override bool IsControllerLeftHand(GameObject controller)
        {
            return CheckActualOrScriptAliasControllerIsLeftHand(controller);
        }

        public override bool IsControllerLeftHand(GameObject controller, bool actual)
        {
            return CheckControllerLeftHand(controller, actual);
        }

        public override bool IsControllerRightHand(GameObject controller)
        {
            return CheckActualOrScriptAliasControllerIsRightHand(controller);
        }

        public override bool IsControllerRightHand(GameObject controller, bool actual)
        {
            return CheckControllerRightHand(controller, actual);
        }

        public override bool IsTouchpadStatic(bool isTouched, Vector2 currentAxisValues, Vector2 previousAxisValues, int compareFidelity)
        {
            // TODO: Implement
            return false;
        }

        public override void ProcessFixedUpdate(VRTK_ControllerReference controllerReference, Dictionary<string, object> options)
        {
            // TODO: Implement
        }

        public override void ProcessUpdate(VRTK_ControllerReference controllerReference, Dictionary<string, object> options)
        {
            // TODO: Implement
        }

        public override void SetControllerRenderModelWheel(GameObject renderModel, bool state)
        {
            // TODO: Implement
        }

        #endregion

        protected virtual WindowsMR_TrackedObject GetTrackedObject(GameObject controller)
        {
            SetTrackedControllerCaches();

            if (IsControllerLeftHand(controller))
            {
                return cachedLeftTrackedObject;
            }
            else if (IsControllerRightHand(controller))
            {
                return cachedRightTrackedObject;
            }

            if (controller == null)
            {
                return null;
            }

            if (cachedTrackedObjectsByGameObject.ContainsKey(controller) && cachedTrackedObjectsByGameObject[controller] != null)
            {
                return cachedTrackedObjectsByGameObject[controller];
            }
            else
            {
                WindowsMR_TrackedObject trackedObject = controller.GetComponent<WindowsMR_TrackedObject>();
                if (trackedObject != null)
                {
                    cachedTrackedObjectsByGameObject.Add(controller, trackedObject);
                    cachedTrackedObjectsByIndex.Add((uint)trackedObject.Index, trackedObject);
                }
                return trackedObject;
            }
        }

        protected virtual void SetTrackedControllerCaches(bool forceRefresh = false)
        {
            if (forceRefresh)
            {
                cachedLeftTrackedObject = null;
                cachedRightTrackedObject = null;
                cachedTrackedObjectsByGameObject.Clear();
                cachedTrackedObjectsByIndex.Clear();
            }

            VRTK_SDKManager sdkManager = VRTK_SDKManager.instance;
            if (sdkManager != null)
            {
                if (cachedLeftTrackedObject == null && sdkManager.loadedSetup.actualLeftController)
                {
                    cachedLeftTrackedObject = sdkManager.loadedSetup.actualLeftController.GetComponent<WindowsMR_TrackedObject>();
                }
                if (cachedRightTrackedObject == null && sdkManager.loadedSetup.actualRightController)
                {
                    cachedRightTrackedObject = sdkManager.loadedSetup.actualRightController.GetComponent<WindowsMR_TrackedObject>();
                }
            }
        }

        protected virtual bool IsButtonPressed(uint index, ButtonPressTypes type, InteractionSourcePressType button)
        {
            bool actual = true;
            WindowsMR_TrackedObject device = GetControllerByIndex(index, actual).GetComponent<WindowsMR_TrackedObject>();

            switch (type)
            {
                case ButtonPressTypes.Press:
                    return device.GetPress(button);
                case ButtonPressTypes.PressDown:
                    return device.GetPressDown(button);
                case ButtonPressTypes.PressUp:
                    return device.GetPressUp(button);
                case ButtonPressTypes.Touch:
                    return device.GetTouch(button);
                case ButtonPressTypes.TouchDown:
                    return device.GetTouchDown(button);
                case ButtonPressTypes.TouchUp:
                    return device.GetTouchUp(button);
            }

            return false;
        }
    }
}
