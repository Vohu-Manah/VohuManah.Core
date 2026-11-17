using System;

namespace ReportViewer.Common.Renderers
{
    /// <summary>
    /// Identifies the supported rendering platforms for the renderer factory.
    /// </summary>
    public enum RenderingPlatform
    {
        /// <summary>Represents the Windows platform.</summary>
        Windows,
        /// <summary>Represents the Linux platform.</summary>
        Linux,
        /// <summary>Represents the macOS platform.</summary>
        MacOs
    }

    /// <summary>
    /// Creates concrete renderer implementations based on the selected platform.
    /// </summary>
    /// <remarks>
    /// The factory currently routes all platform selections to the cross-platform
    /// Linux-compatible implementations, which provides a simple entry point while the
    /// broader rendering pipeline is being modernized.
    /// </remarks>
    public static class ReportRendererFactory
    {
        /// <summary>
        /// Creates an Excel renderer for the requested platform.
        /// </summary>
        /// <param name="platform">The platform for which to create the renderer.</param>
        /// <returns>An Excel renderer implementation.</returns>
        public static IExcelRenderer CreateExcelRenderer(RenderingPlatform platform)
        {
            return platform switch
            {
                RenderingPlatform.Linux => new LinuxExcelRenderer(),
                RenderingPlatform.MacOs => new LinuxExcelRenderer(),
                _ => new LinuxExcelRenderer()
            };
        }

        /// <summary>
        /// Creates a PDF renderer for the requested platform.
        /// </summary>
        /// <param name="platform">The platform for which to create the renderer.</param>
        /// <returns>A PDF renderer implementation.</returns>
        public static IPdfRenderer CreatePdfRenderer(RenderingPlatform platform)
        {
            return platform switch
            {
                RenderingPlatform.Linux => new LinuxPdfRenderer(),
                RenderingPlatform.MacOs => new LinuxPdfRenderer(),
                _ => new LinuxPdfRenderer()
            };
        }
    }
}
