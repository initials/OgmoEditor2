using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OgmoEditor.LevelData.Layers;
using System.Windows.Forms;
using OgmoEditor.LevelEditors.LayerEditors;
using System.Drawing;

namespace OgmoEditor.LevelEditors.Tools.TileTools
{
    public abstract class TileTool : Tool
    {
        public TileTool(string name, string image)
            : base(name, image)
        {

        }

        public TileLayerEditor LayerEditor
        {
            get { return (TileLayerEditor)LevelEditor.LayerEditors[Ogmo.LayersWindow.CurrentLayerIndex]; }
        }

        public bool IsValidTileCell(Point cell)
        {
            return cell.X >= 0 && cell.Y >= 0 && cell.X < LayerEditor.Layer.TileCellsX && cell.Y < LayerEditor.Layer.TileCellsY;
        } 
    }
}
