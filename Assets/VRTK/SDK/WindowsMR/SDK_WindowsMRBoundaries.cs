using System;
using UnityEngine;

namespace VRTK
{
    
#if VRTK_DEFINE_SDK_WINDOWSMR
    using UnityEngine;
    using System.Collections.Generic;
    using UnityEngine.Experimental.XR;
    using UnityEngine.XR.WSA;
    using UnityEngine.XR;
#endif

    /// <summary>
    /// The Oculus Boundaries SDK script provides a bridge to the Oculus SDK play area.
    /// </summary>
    [SDK_Description(typeof(SDK_WindowsMR))]
    public class SDK_WindowsMRBoundaries : SDK_BaseBoundaries
    {
        public override bool GetDrawAtRuntime()
        {
            // TODO: Implement
            return false;
        }

        public override Transform GetPlayArea()
        {
            if(cachedPlayArea == null)
            {
                Transform headsetCamera = VRTK_DeviceFinder.HeadsetCamera();
                cachedPlayArea = headsetCamera.transform;
            }

            if (cachedPlayArea.parent)
            {
                cachedPlayArea = cachedPlayArea.parent;
            }

            return cachedPlayArea;
        }

        public override float GetPlayAreaBorderThickness()
        {
            // TODO: Implement
            return 0.1f;
        }

        public override Vector3[] GetPlayAreaVertices()
        {
            Debug.Log("GetPlayAreaVertices");
            List<Vector3> boundaryGeometry = new List<Vector3>(0);

            if (Boundary.TryGetGeometry(boundaryGeometry))
            {
                if(boundaryGeometry.Count > 0)
                {
                    foreach(Vector3 point in boundaryGeometry)
                    {
                        Debug.Log("Point: " + point);
                        return boundaryGeometry.ToArray();
                    }
                }
                else
                {
                    Debug.Log("Boundary has no points");
                }
            }

            return null;
        }

        public override void InitBoundaries()
        {
            Debug.Log("InitBoundaries");
            if (HolographicSettings.IsDisplayOpaque)
            {
                // Defaulting coordinate system to RoomScale in immersive headsets.
                // This puts the origin 0,0,0 on the floor if a floor has been established during RunSetup via MixedRealityPortal
                XRDevice.SetTrackingSpaceType(TrackingSpaceType.RoomScale);
            }
            else
            {
                // Defaulting coordinate system to Stationary for HoloLens.
                // This puts the origin 0,0,0 at the first place where the user started the application.
                XRDevice.SetTrackingSpaceType(TrackingSpaceType.Stationary);
            }

            Transform headsetCamera = VRTK_DeviceFinder.HeadsetCamera();

            cachedPlayArea = headsetCamera.transform;

            Debug.Log("Camera? " + headsetCamera);
        }

        public override bool IsPlayAreaSizeCalibrated()
        {
            // TODO: Implement
            return false;
        }

        public override void SetDrawAtRuntime(bool value)
        {
            // TODO: Implement
        }
    }
}
