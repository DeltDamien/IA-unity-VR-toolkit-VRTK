// Immersive Headset Motion Controller|SDK_ImmersiveHeadset|004
namespace VRTK
{
#if VRTK_DEFINE_IMMERSIVEHEADSET
    using UnityEngine;
    using System.Collections.Generic;
#endif

    /// <summary>
    /// The Immersive Headset Motion Controller SDK script provides a bridge to SDK methods that deal with the input devices.
    /// </summary>
    [SDK_Description(typeof(SDK_ImmersiveHeadsetSystem))]
    public class SDK_ImmersiveHeadsetController
#if VRTK_DEFINE_IMMERSIVEHEADSET
        : SDK_BaseController
#else
        : SDK_FallbackController
#endif
    {
#if VRTK_DEFINE_IMMERSIVEHEADSET
        // poop goes here
#endif
    }
}