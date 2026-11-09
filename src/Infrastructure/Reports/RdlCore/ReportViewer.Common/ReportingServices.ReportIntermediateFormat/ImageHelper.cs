using ReportingServices.OnDemandProcessing;
using ReportingServices.ReportProcessing;
using System;
using System.IO;

namespace ReportingServices.ReportIntermediateFormat
{
	internal static class ImageHelper
	{
		internal static string StoreImageDataInChunk(ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes chunkType, byte[] imageData, string mimeType, OnDemandMetadata odpMetadata, IChunkFactory chunkFactory)
		{
			string text = GenerateImageStreamName();
			_ = odpMetadata.ReportSnapshot;
			using (Stream stream = chunkFactory.CreateChunk(text, chunkType, mimeType))
			{
				stream.Write(imageData, 0, imageData.Length);
				return text;
			}
		}

		internal static string GenerateImageStreamName()
		{
			return Guid.NewGuid().ToString("N");
		}
	}
}
