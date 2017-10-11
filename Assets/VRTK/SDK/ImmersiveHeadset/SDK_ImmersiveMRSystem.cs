namespace VRTK
{
#if VRTK_DEFINE_SDK_IMMERSIVEMR
    using UnityEngine.XR.WSA;
#endif

    [SDK_Description("ImmersiveMR", SDK_ImmersiveMRDefines.ScriptingDefineSymbol, "WSA_XR", "Standalone")]
    public class SDK_ImmersiveMRSystem
#if VRTK_DEFINE_SDK_IMMERSIVEMR
        : SDK_BaseSystem
#else
        :SDK_FallbackSystem
#endif
    {
#if VRTK_DEFINE_SDK_IMMERSIVEMR
        /// <summary>
        /// The ForceInterleavedReprojectionOn method determines whether Interleaved Reprojection should be forced on or off.
        /// </summary>
        /// <param name="force">If true then Interleaved Reprojection will be forced on, if false it will not be forced on.</param>
        public override void ForceInterleavedReprojectionOn(bool force)
        {
            // TODO: Compare with Oculus and SteamVR
        }

        /// <summary>
        /// The IsDisplayOnDesktop method returns true if the display is extending the desktop.
        /// </summary>
        /// <returns>Returns true if the display is extending the desktop</returns>
        public override bool IsDisplayOnDesktop()
        {
            // TODO: Compare with Oculus and SteamVR
            return false;
        }

        /// <summary>
        /// The ShouldAppRenderWithLowResources method is used to determine if the Unity app should use low resource mode. Typically true when the dashboard is showing.
        /// </summary>
        /// <returns>Returns true if the Unity app should render with low resources.</returns>
        public override bool ShouldAppRenderWithLowResources()
        {
            // TODO: Compare with Oculus and SteamVR
            return false;
        }
#endif
    }
}
