// ImmersiveHeadset Boundaries|SDK_ImmersiveHeadset|005
namespace VRTK
{
#if VRTK_DEFINE_SDK_IMMERSIVEHEADSET
    using UnityEngine;
    using UnityEngine.XR;
    using UnityEngine.Experimental.XR;
#endif

    /// <summary>
    /// The Microsoft Immersive Headset Boundaries SDK script provides a bridge to the Mixed Reality SDK play area.
    /// </summary>
    [SDK_Description(typeof(SDK_ImmersiveHeadsetSystem))]
    public class SDK_ImmersiveHeadsetBoundaries
#if VRTK_DEFINE_SDK_IMMERSIVEHEADSET
        : SDK_BaseBoundaries
#else
        : SDK_FallbackBoundaries
#endif
    {
#if VRTK_DEFINE_SDK_IMMERSIVEHEADSET

        /// <summary>
        /// The InitBoundaries method is run on start of scene and can be used to initialse anything on game start.
        /// </summary>
        public override void InitBoundaries()
        {
            if (!XRDevice.SetTrackingSpaceType(TrackingSpaceType.RoomScale))
            {
                Debug.LogError("Could not set TrackingSpace to Roomscale. Please run the Room Setup again!");
            }
        }

        /// <summary>
        /// The GetPlayArea method returns the Transform of the object that is used to represent the play area in the scene.
        /// </summary>
        /// <returns>The root transform of the scene, as the windows mixed reality API changes the origin point to correspond to the tracked space.</returns>
        public override Transform GetPlayArea()
        {
            cachedPlayArea = GetSDKManagerPlayArea();
            if (cachedPlayArea == null)
            {
                // we return the root transform, because the microsoft / unity integration means that when the play area is set up the world transform becomes the play area.
                cachedPlayArea = Camera.main.transform.root;
            }
            return cachedPlayArea;
        }

        /// <summary>
        /// The GetPlayAreaVertices method returns the points of the play area boundaries.
        /// </summary>
        /// <returns>A Vector3 array of the points in the scene that represent the play area boundaries.</returns>
        public override Vector3[] GetPlayAreaVertices()
        {
            var vertices = new System.Collections.Generic.List<Vector3>();
            if (!Boundary.TryGetGeometry(vertices, Boundary.Type.TrackedArea))
            {
                Debug.LogError("Could not get geometry, have you correctly set up your tracking space?");
                return null;
            }
            return vertices.ToArray();
        }

        /// <summary>
        /// The GetPlayAreaBorderThickness returns the thickness of the drawn border for the given play area.
        /// </summary>
        /// <returns>The thickness of the drawn border.</returns>
        public override float GetPlayAreaBorderThickness()
        {
            return 0.1f;
        }

        /// <summary>
        /// The IsPlayAreaSizeCalibrated method returns whether the given play area size has been auto calibrated by external sensors.
        /// </summary>
        /// <returns>Returns true if the play area size has been auto calibrated and set by external sensors.</returns>
        public override bool IsPlayAreaSizeCalibrated()
        {
            return XRDevice.GetTrackingSpaceType() == TrackingSpaceType.RoomScale;
        }

        /// <summary>
        /// The GetDrawAtRuntime method returns whether the given play area drawn border is being displayed.
        /// </summary>
        /// <returns>Returns true if the drawn border is being displayed.</returns>
        public override bool GetDrawAtRuntime()
        {
            return Boundary.visible;
        }

        /// <summary>
        /// The SetDrawAtRuntime method sets whether the given play area drawn border should be displayed at runtime.
        /// </summary>
        /// <param name="value">The state of whether the drawn border should be displayed or not.</param>
        public override void SetDrawAtRuntime(bool value)
        {
            Boundary.visible = value;
        }

#endif 
    }
}