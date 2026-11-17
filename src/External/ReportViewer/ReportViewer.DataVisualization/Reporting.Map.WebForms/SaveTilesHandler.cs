using System.Drawing;

namespace Reporting.Map.WebForms
{
	internal delegate void SaveTilesHandler(Layer layer, string[,] tileUrls, Image[,] tileImages);
}
