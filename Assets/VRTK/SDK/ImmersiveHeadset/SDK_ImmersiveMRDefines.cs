namespace VRTK
{
    using System;

    /// <summary>
    /// Handles all the scripting define symbols for the Immersive Mixed Reality SDK.
    /// </summary>
    public static class SDK_ImmersiveMRDefines
    {
        /// <summary>
        /// The scripting define symbol for the Immersive Mixed Reality SDK.
        /// </summary>
        public const string ScriptingDefineSymbol = SDK_ScriptingDefineSymbolPredicateAttribute.RemovableSymbolPrefix + "SDK_IMMERSIVEMR";

        private const string BuildTargetGroupName = "Standalone";

        // TODO: Version checking (see below)

        /*
        [SDK_ScriptingDefineSymbolPredicate(ScriptingDefineSymbol, BuildTargetGroupName)]
        [SDK_ScriptingDefineSymbolPredicate(SDK_ScriptingDefineSymbolPredicateAttribute.RemovableSymbolPrefix + "OCULUS_UTILITIES_1_12_0_OR_NEWER", BuildTargetGroupName)]
        private static bool IsUtilitiesVersion1120OrNewer()
        {
            Version wrapperVersion = GetOculusWrapperVersion();
            return wrapperVersion != null && wrapperVersion >= new Version(1, 12, 0);
        }

        [SDK_ScriptingDefineSymbolPredicate(ScriptingDefineSymbol, BuildTargetGroupName)]
        [SDK_ScriptingDefineSymbolPredicate(SDK_ScriptingDefineSymbolPredicateAttribute.RemovableSymbolPrefix + "OCULUS_UTILITIES_1_11_0_OR_OLDER", BuildTargetGroupName)]
        private static bool IsUtilitiesVersion1110OrOlder()
        {
            Version wrapperVersion = GetOculusWrapperVersion();
            return wrapperVersion != null && wrapperVersion < new Version(1, 12, 0);
        }

        [SDK_ScriptingDefineSymbolPredicate(AvatarScriptingDefineSymbol, BuildTargetGroupName)]
        private static bool IsAvatarAvailable()
        {
            return (IsUtilitiesVersion1120OrNewer() || IsUtilitiesVersion1110OrOlder())
                   && VRTK_SharedMethods.GetTypeUnknownAssembly("OvrAvatar") != null;
        }

        private static Version GetOculusWrapperVersion()
        {
            Type pluginClass = VRTK_SharedMethods.GetTypeUnknownAssembly("OVRPlugin");
            if (pluginClass == null)
            {
                return null;
            }

            FieldInfo versionField = pluginClass.GetField("wrapperVersion", BindingFlags.Public | BindingFlags.Static);
            if (versionField == null)
            {
                return null;
            }

            return (Version)versionField.GetValue(null);
        }
        */
    }
}
