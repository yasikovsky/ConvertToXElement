using JetBrains.Application.BuildScript.Application.Zones;

namespace ReSharperPlugin.ConvertToXElement
{
    [ZoneMarker]
    public class ZoneMarker : IRequire<IConvertToXElementZone>
    {
    }
}