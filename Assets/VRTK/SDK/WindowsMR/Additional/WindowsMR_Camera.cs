namespace VRTK
{
    using UnityEngine;
    using UnityEngine.XR;

    /// <summary>
    /// Camera script for the main camera for Immersive Mixed Reality. 
    /// </summary>
    [RequireComponent (typeof(Camera))]
    public class WindowsMR_Camera : MonoBehaviour
    {
        /// <summary>
        /// Name of the Windows Mixed Reality Device as listed in XRSettings.
        /// </summary>
        private const string DEVICE_NAME = "WindowsMR";
        
        void Start()
        {
            if (CheckForMixedRealitySupport())
            {
                SetupMRCamera();
            }
        }

        /// <summary>
        /// Check if the Mixed (Virtual) Reality Settings are properly set.
        /// </summary>
        /// <returns>Are the settings set.</returns>
        private bool CheckForMixedRealitySupport()
        {
            if(XRSettings.enabled == false)
            {
                Debug.Log("XRSettings: " + XRSettings.enabled);
                Debug.LogError("XRSettings are not enabled. Enable in PlayerSettings. Do not forget to add Windows Mixed Reality to Virtual Reality SDKs.");
                return false;
            }
            else
            {
                foreach(string device in XRSettings.supportedDevices)
                {
                    if(device.Equals("WindowsMR"))
                    {
                        return true;
                    }
                }
                Debug.LogError("Windows Mixed Reality is not supported in XRSettings, add in PlayerSettings.");
            }

            return false;
        }

        /// <summary>
        /// Setup the MR camera properly.
        /// </summary>
        private void SetupMRCamera()
        {
            Camera camera = GetComponent<Camera>();
            if (camera.tag != "MainCamera")
            {
                camera.tag = "MainCamera";
            }

            if(camera.stereoTargetEye != StereoTargetEyeMask.Both)
            {
                Debug.LogError("Target eye of main camera is not set to both. Are you sure you want to render only one eye?");
            }
        }
    }
}