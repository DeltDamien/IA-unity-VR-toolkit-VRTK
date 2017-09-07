// ImmersiveHeadset System|SDK_ImmersiveHeadset|002
namespace VRTK
{
    /// <summary>
    /// The Microsoft Immersive Headset System SDK script provides a bridge to the Microsoft Holographic SDK.
    /// </summary>
    [SDK_Description("ImmersiveHeadset", SDK_ImmersiveHeadsetDefines.ScriptingDefineSymbol, "ImmersiveHeadset", "Standalone")]
    public class SDK_ImmersiveHeadsetSystem
#if VRTK_DEFINE_SDK_IMMERSIVEHEADSET
        : SDK_BaseSystem
#else
        : SDK_FallbackSystem
#endif
    {
#if VRTK_DEFINE_SDK_IMMERSIVEHEADSET
        /// <summary>
        /// The IsDisplayOnDesktop method returns true if the display is extending the desktop.
        /// </summary>
        /// <returns>Returns true if the display is extending the desktop</returns>
        public override bool IsDisplayOnDesktop()
        {
            return true;
        }

        /// <summary>
        /// The ShouldAppRenderWithLowResources method is used to determine if the Unity app should use low resource mode. Typically true when the dashboard is showing.
        /// </summary>
        /// <returns>Returns true if the Unity app should render with low resources.</returns>
        public override bool ShouldAppRenderWithLowResources()
        {
            return false;
        }

        /// <summary>
        /// The ForceInterleavedReprojectionOn method determines whether Interleaved Reprojection should be forced on or off.
        /// </summary>
        /// <param name="force">If true then Interleaved Reprojection will be forced on, if false it will not be forced on.</param>
        public override void ForceInterleavedReprojectionOn(bool force)
        {
        }
#endif
    }
}

