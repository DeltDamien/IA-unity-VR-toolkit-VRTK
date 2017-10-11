using System;
using UnityEngine;

namespace VRTK
{
#if VRTK_DEFINE_SDK_WINDOWSMR
    using UnityEngine;
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
            // TODO: Implement
            return null;
        }

        public override float GetPlayAreaBorderThickness()
        {
            // TODO: Implement
            return 0;
        }

        public override Vector3[] GetPlayAreaVertices()
        {
            // TODO: Implement
            return null;
        }

        public override void InitBoundaries()
        {
            // TODO: Implement
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
