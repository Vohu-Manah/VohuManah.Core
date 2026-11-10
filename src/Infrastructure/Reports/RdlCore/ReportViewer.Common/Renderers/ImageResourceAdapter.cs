using System;
using System.IO;
using System.Resources;

namespace ReportViewer.Common.Renderers
{
    /// <summary>
    /// Provides a small adapter for writing embedded resources to a stream.
    /// </summary>
    /// <remarks>
    /// The adapter is intentionally lightweight and focuses on the common resource
    /// payload shapes used by the rendering pipeline, including streams, strings,
    /// and byte arrays.
    /// </remarks>
    public class ImageResourceAdapter : IResourceAdapter
    {
        /// <summary>
        /// Writes a generic resource payload to the supplied stream.
        /// </summary>
        /// <param name="resource">The resource payload to write.</param>
        /// <param name="destinationStream">The destination stream that receives the payload.</param>
        /// <exception cref="ArgumentNullException">Thrown when a required argument is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the resource payload is not supported.</exception>
        public virtual void WriteResource(object resource, Stream destinationStream)
        {
            if (destinationStream == null) throw new ArgumentNullException(nameof(destinationStream));

            if (resource is Stream stream)
            {
                stream.CopyTo(destinationStream);
                return;
            }

            if (resource is string text)
            {
                using var writer = new StreamWriter(destinationStream, leaveOpen: true);
                writer.Write(text);
                writer.Flush();
                return;
            }

            if (resource is byte[] bytes)
            {
                destinationStream.Write(bytes, 0, bytes.Length);
                return;
            }

            if (resource == null)
            {
                throw new InvalidOperationException("Cannot write a null resource payload.");
            }

            throw new InvalidOperationException($"Unsupported resource payload type '{resource.GetType().FullName}'.");
        }

        /// <summary>
        /// Writes an embedded resource to the supplied stream.
        /// </summary>
        /// <param name="resourceManager">The resource manager that owns the embedded resource.</param>
        /// <param name="resourceName">The name of the resource to read.</param>
        /// <param name="destinationStream">The stream that receives the resource content.</param>
        /// <exception cref="ArgumentNullException">Thrown when a required argument is null.</exception>
        /// <exception cref="ArgumentException">Thrown when the resource name is empty or whitespace.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the resource payload is not supported.</exception>
        public virtual void WriteEmbeddedImage(ResourceManager resourceManager, string resourceName, Stream destinationStream)
        {
            if (resourceManager == null) throw new ArgumentNullException(nameof(resourceManager));
            if (destinationStream == null) throw new ArgumentNullException(nameof(destinationStream));
            if (string.IsNullOrWhiteSpace(resourceName)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(resourceName));

            object resource = null;
            try
            {
                resource = resourceManager.GetObject(resourceName);
            }
            catch (MissingManifestResourceException)
            {
                // Fall back to the stream API for resources that are stored as streams.
            }

            if (resource != null)
            {
                WriteResource(resource, destinationStream);
                return;
            }

            try
            {
                using Stream resourceStream = resourceManager.GetStream(resourceName);
                resourceStream.CopyTo(destinationStream);
                return;
            }
            catch (MissingManifestResourceException)
            {
                // Fall back to object-based resources for non-stream resources.
            }

            throw new InvalidOperationException($"Unsupported embedded resource '{resourceName}'.");
        }
    }
}
