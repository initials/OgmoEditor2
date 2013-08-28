using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OgmoEditor.LevelData.Layers;
using System.Windows.Forms;
using OgmoEditor.LevelEditors.LayerEditors;
using System.Drawing;

namespace OgmoEditor.LevelEditors.Tools.GridTools
{
    public abstract class GridTool : Tool
    {
        public GridTool(string name, string image)
            : base(name, image)
        {

        }

        public GridLayerEditor LayerEditor
        {
            get { return (GridLayerEditor)LevelEditor.LayerEditors[Ogmo.LayersWindow.CurrentLayerIndex]; }
        }

        public bool IsValidGridCell(Point cell)
        {
            return cell.X >= 0 && cell.Y >= 0 && cell.X < LayerEditor.Layer.Grid.GetLength(0) && cell.Y < LayerEditor.Layer.Grid.GetLength(1);
        }
    }
}
