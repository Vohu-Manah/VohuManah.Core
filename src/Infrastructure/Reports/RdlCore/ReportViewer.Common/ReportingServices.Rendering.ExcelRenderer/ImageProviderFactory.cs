using System.Runtime.InteropServices;

namespace ReportingServices.Rendering.ExcelRenderer
{
	/// <summary>
	/// Factory for creating platform-specific image providers.
	/// </summary>
	internal static class ImageProviderFactory
	{
		/// <summary>
		/// Create appropriate image provider for current platform.
		/// </summary>
		public static IImageProvider CreateProvider()
		{
			// Check if running on Windows
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				return new Windows.WindowsImageProvider();
			}

			// Use cross-platform provider on non-Windows
			return new CrossPlatformImageProvider();
		}
	}
}
