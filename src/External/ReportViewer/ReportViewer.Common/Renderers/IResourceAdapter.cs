using System.IO;
using System.Resources;

namespace ReportViewer.Common.Renderers
{
    /// <summary>
    /// Defines a contract for writing resource payloads to a stream.
    /// </summary>
    public interface IResourceAdapter
    {
        /// <summary>
        /// Writes a resource payload to the supplied stream.
        /// </summary>
        /// <param name="resource">The resource payload to write.</param>
        /// <param name="destinationStream">The destination stream.</param>
        void WriteResource(object resource, Stream destinationStream);

        /// <summary>
        /// Writes an embedded resource identified by a resource manager to the supplied stream.
        /// </summary>
        /// <param name="resourceManager">The resource manager that owns the resource.</param>
        /// <param name="resourceName">The name of the resource to write.</param>
        /// <param name="destinationStream">The destination stream.</param>
        void WriteEmbeddedImage(ResourceManager resourceManager, string resourceName, Stream destinationStream);
    }
}
