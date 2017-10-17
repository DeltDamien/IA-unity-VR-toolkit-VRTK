using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.WSA.Input;

namespace VRTK
{
    public class WindowsMR_TrackedObject : MonoBehaviour
    {

        private struct ButtonState
        {
            //
            // Summary:
            //     ///
            //     Normalized amount ([0, 1]) representing how much select is pressed.
            //     ///
            public float SelectPressedAmount { get; set; }
            //
            // Summary:
            //     ///
            //     Depending on the InteractionSourceType of the interaction source, this returning
            //     true could represent a number of equivalent things: main button on a blicker,
            //     air-tap on a hand, and the trigger on a motion controller.
            //     ///
            public bool SelectPressed { get; set; }
            //
            // Summary:
            //     ///
            //     Whether or not the menu button is pressed.
            //     ///
            public bool MenuPressed { get; set; }
            //
            // Summary:
            //     ///
            //     Whether the controller is grasped.
            //     ///
            public bool Grasped { get; set; }
            //
            // Summary:
            //     ///
            //     Whether or not the touchpad is touched.
            //     ///
            public bool TouchpadTouched { get; set; }
            //
            // Summary:
            //     ///
            //     Whether or not the touchpad is pressed, as if a button.
            //     ///
            public bool TouchpadPressed { get; set; }
            //
            // Summary:
            //     ///
            //     Normalized coordinates for the position of a touchpad interaction.
            //     ///
            public Vector2 TouchpadPosition { get; set; }
            //
            // Summary:
            //     ///
            //     Normalized coordinates for the position of a thumbstick.
            //     ///
            public Vector2 ThumbstickPosition { get; set; }
            //
            // Summary:
            //     ///
            //     Whether or not the thumbstick is pressed.
            //     ///
            public bool ThumbstickPressed { get; set; }
        }

        [SerializeField]
        private InteractionSourceHandedness handedness;

        private uint index;
        public uint Index { get { return index; } }

        private ButtonState currentButtonState;
        private ButtonState prevButtonState;

        private Vector3 angularVelocity;
        public Vector3 AngularVelocity { get { return angularVelocity; } }

        private float hairTriggerDelta = 0.1f; // amount trigger must be pulled or released to change state
        private float hairTriggerLimit;
        private bool hairTriggerState;
        private bool hairTriggerPrevState;

        private bool isDetected;

        private void Start()
        {
            Debug.Log("Start controller " + handedness);
            InteractionManager.InteractionSourceDetected += InteractionManager_InteractionSourceDetected;
            // InteractionManager.InteractionSourceUpdated updates too often and messes up Down and Up.
            // Only used for controller position.
            // Might be better in future releases.
            //InteractionManager.InteractionSourceUpdated += InteractionManager_InteractionSourceUpdated;
            InteractionManager.InteractionSourceLost += InteractionManager_InteractionSourceLost;
        }

        private void Update()
        {
            if (isDetected)
            {
                UpdateStates();
                UpdateHairTrigger();
            }
        }

        #region Getter functions
        public bool GetPress(InteractionSourcePressType button)
        {
            switch(button)
            {
                case InteractionSourcePressType.Select:
                    return currentButtonState.SelectPressed;
                case InteractionSourcePressType.Grasp:
                    return currentButtonState.Grasped;
                case InteractionSourcePressType.Menu:
                    return prevButtonState.MenuPressed;
            }
            return false;
        }

        public bool GetPressDown(InteractionSourcePressType button)
        {
            switch (button)
            {
                case InteractionSourcePressType.Select:
                    return prevButtonState.SelectPressed == false && currentButtonState.SelectPressed == true;
                case InteractionSourcePressType.Grasp:
                    return prevButtonState.Grasped == false && currentButtonState.Grasped == true;
                case InteractionSourcePressType.Menu:
                    return prevButtonState.MenuPressed == false && currentButtonState.MenuPressed == true;
            }
            return false;
        }

        public bool GetPressUp(InteractionSourcePressType button)
        {
            switch (button)
            {
                case InteractionSourcePressType.Select:
                    return prevButtonState.SelectPressed == true && currentButtonState.SelectPressed == false;
                case InteractionSourcePressType.Grasp:
                    return prevButtonState.Grasped == true && currentButtonState.Grasped == false;
                case InteractionSourcePressType.Menu:
                    return prevButtonState.MenuPressed == true && currentButtonState.MenuPressed == false;
            }
            return false;
        }

        public bool GetTouch(InteractionSourcePressType button)
        {
            switch (button)
            {
                case InteractionSourcePressType.Touchpad:
                    return currentButtonState.TouchpadTouched;
            }
            return false;
        }

        public bool GetTouchDown(InteractionSourcePressType button)
        {
            switch (button)
            {
                case InteractionSourcePressType.Touchpad:
                    return prevButtonState.TouchpadTouched == false && currentButtonState.TouchpadTouched == true;
            }
            return false;
        }

        public bool GetTouchUp(InteractionSourcePressType button)
        {
            switch (button)
            {
                case InteractionSourcePressType.Touchpad:
                    return prevButtonState.TouchpadTouched == true && currentButtonState.TouchpadTouched == false;
            }
            return false;
        }

        public Vector2 GetAxis(InteractionSourcePressType button)
        {
            switch(button)
            {
                case InteractionSourcePressType.Touchpad:
                    return currentButtonState.TouchpadPosition;

                case InteractionSourcePressType.Thumbstick:
                    return currentButtonState.ThumbstickPosition;
            }
            return Vector2.zero;
        }

        public bool GetHairTrigger()
        {
            //Update();
            return hairTriggerState;
        }

        public bool GetHairTriggerDown()
        {
            //Update();
            return hairTriggerState && !hairTriggerPrevState;
        }
        public bool GetHairTriggerUp()
        {
            //Update();
            return !hairTriggerState && hairTriggerPrevState;
        }
        #endregion

        #region Event callbacks
        private void InteractionManager_InteractionSourceDetected(InteractionSourceDetectedEventArgs args)
        {
            InteractionSourceState state = args.state;
            InteractionSource source = state.source;

            if (source.kind == InteractionSourceKind.Controller && source.handedness == handedness)
            {
                index = source.id;
                currentButtonState = new ButtonState();
                isDetected = true;
                Debug.Log("New controller detected " + source.handedness);
            }
        }

        private void InteractionManager_InteractionSourceLost(InteractionSourceLostEventArgs args)
        {
            InteractionSourceState state = args.state;
            InteractionSource source = state.source;

            if (source.kind == InteractionSourceKind.Controller && source.handedness == handedness)
            {
                index = uint.MaxValue;
                currentButtonState = new ButtonState();
                isDetected = false;
                Debug.Log("Controller lost " + source.handedness);
            }
        }

        private void InteractionManager_InteractionSourceUpdated(InteractionSourceUpdatedEventArgs args)
        {
            InteractionSourceState state = args.state;
            InteractionSource source = state.source;

            if (source.kind == InteractionSourceKind.Controller && source.handedness == handedness)
            {
                //UpdateInteractionState(state);
                UpdatePose(state);
            }
        }
        #endregion

        #region Update functions
        private void UpdateStates()
        {
            InteractionSourceState[] states = InteractionManager.GetCurrentReading();

            foreach(InteractionSourceState state in states)
            {
                UpdateInteractionState(state);
            }
        }

        private void UpdatePose(InteractionSourceState state)
        {
            UpdateAngularVelocity(state.sourcePose);
            UpdateControllerPose(state.sourcePose);
        }

        private void UpdateInteractionState(InteractionSourceState state)
        {
            InteractionSource source = state.source;

            if (source.handedness == handedness && source.id == index)
            {
                prevButtonState = currentButtonState;

                currentButtonState.SelectPressed = state.selectPressed;
                currentButtonState.SelectPressedAmount = state.selectPressedAmount;

                if (source.supportsGrasp)
                {
                    currentButtonState.Grasped = state.grasped;
                }

                if (source.supportsMenu)
                {
                    currentButtonState.MenuPressed = state.menuPressed;
                }

                if (source.supportsThumbstick)
                {
                    currentButtonState.ThumbstickPosition = state.thumbstickPosition;
                    currentButtonState.ThumbstickPressed = state.thumbstickPressed;
                }

                if (source.supportsTouchpad)
                {
                    currentButtonState.TouchpadPosition = state.touchpadPosition;
                    currentButtonState.TouchpadPressed = state.touchpadPressed;
                    currentButtonState.TouchpadTouched = state.touchpadTouched;
                }
            }
        }

        private void UpdateControllerPose(InteractionSourcePose pose)
        {
            Quaternion newRotation;
            if (pose.TryGetRotation(out newRotation, InteractionSourceNode.Grip))
            {
                transform.localRotation = newRotation;
            }

            Vector3 newPosition;
            if (pose.TryGetPosition(out newPosition, InteractionSourceNode.Grip))
            {
                transform.localPosition = newPosition;
            }
        }

        private void UpdateAngularVelocity(InteractionSourcePose pose)
        {
            Vector3 newAngularVelocity;
            if(pose.TryGetAngularVelocity(out newAngularVelocity))
            {
                angularVelocity = newAngularVelocity;
            }
        }

        private void UpdateHairTrigger()
        {
            hairTriggerPrevState = hairTriggerState;
            float value = currentButtonState.SelectPressedAmount;

            if (hairTriggerState)
            {
                if (value < hairTriggerLimit - hairTriggerDelta || value <= 0.0f)
                    hairTriggerState = false;
            }
            else
            {
                if (value > hairTriggerLimit + hairTriggerDelta || value >= 1.0f)
                    hairTriggerState = true;
            }

            hairTriggerLimit = hairTriggerState ? Mathf.Max(hairTriggerLimit, value) : Mathf.Min(hairTriggerLimit, value);
        }
        #endregion
    }
}