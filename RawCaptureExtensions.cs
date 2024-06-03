using System.Reflection;
using PacketDotNet;
using SharpPcap;

namespace CapturingAndParsingPackets;

    public static class RawCaptureExtensions
    {
        private static readonly MethodInfo GetLinkLayerType;

        static RawCaptureExtensions()
        {
            var propertyInfo = typeof(RawCapture).GetProperty("LinkLayerType", BindingFlags.Public | BindingFlags.Instance);
            GetLinkLayerType = propertyInfo?.GetMethod;
        }

        public static LinkLayers GetLinkLayers(this RawCapture rawCapture)
        {
            return (LinkLayers) (GetLinkLayerType?.Invoke(rawCapture, null) ?? 0);
        }
    }