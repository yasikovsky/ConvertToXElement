using JetBrains.Application.Settings;
using JetBrains.Application.Settings.WellKnownRootKeys;

namespace ReSharperPlugin.ConvertToXElement
{
    // Settings that can persist in dotSettings files
    [SettingsKey(
        typeof(EnvironmentSettings),
//        typeof(CodeEditingSettings),
        "Settings for ConvertToXElement")]
    public class PluginSettings
    {
        [SettingsEntry(DefaultValue: false, Description: "Include comments")]
        public bool IncludeComments;
    } }
