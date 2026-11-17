using System.Collections.Generic;

namespace ReportingServices.Rendering.RichText
{
	internal class TextRunItemizedData
	{
		internal List<int> SplitIndexes;

		internal List<TexRunShapeData> GlyphData;

		internal TextRunItemizedData(List<int> splitIndexes, List<TexRunShapeData> textRunsShapeData)
		{
			SplitIndexes = splitIndexes;
			GlyphData = textRunsShapeData;
		}
	}
}
