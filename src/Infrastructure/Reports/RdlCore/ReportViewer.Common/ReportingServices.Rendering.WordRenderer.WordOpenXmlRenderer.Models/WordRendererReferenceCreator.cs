using ReportingServices.OnDemandProcessing.Scalability;
using ReportingServices.ReportIntermediateFormat.Persistence;

namespace ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Models
{
	internal sealed class WordRendererReferenceCreator : IReferenceCreator
	{
		private static WordRendererReferenceCreator _instance = new WordRendererReferenceCreator();

		internal static WordRendererReferenceCreator Instance => _instance;

		private WordRendererReferenceCreator()
		{
		}

		public bool TryCreateReference(IStorable refTarget, out BaseReference reference)
		{
			reference = null;
			return false;
		}

		public bool TryCreateReference(ObjectType referenceObjectType, out BaseReference reference)
		{
			reference = null;
			return false;
		}
	}
}
