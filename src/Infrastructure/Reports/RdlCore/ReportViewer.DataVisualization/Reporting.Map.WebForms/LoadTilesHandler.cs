using System.Drawing;

namespace Reporting.Map.WebForms
{
	internal delegate Image[,] LoadTilesHandler(Layer layer, string[,] tileUrls);
}
