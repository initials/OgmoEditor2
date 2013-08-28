using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OgmoEditor.LevelEditors.Actions.GridActions;

namespace OgmoEditor.LevelEditors.Tools.GridTools
{
    public class GridFloodTool : GridTool
    {
        public GridFloodTool()
            : base("Flood Fill", "flood.png")
        {
            
        }

        public override void OnMouseLeftClick(System.Drawing.Point location)
        {
            location = LayerEditor.Layer.Definition.ConvertToGrid(location);
            if (IsValidGridCell(location) && !LayerEditor.Layer.Grid[location.X, location.Y])
                LevelEditor.Perform(new GridFloodAction(LayerEditor.Layer, location.X, location.Y, true)); 
        }

        public override void OnMouseRightClick(System.Drawing.Point location)
        {
            location = LayerEditor.Layer.Definition.ConvertToGrid(location);
            if (IsValidGridCell(location) && LayerEditor.Layer.Grid[location.X, location.Y])
                LevelEditor.Perform(new GridFloodAction(LayerEditor.Layer, location.X, location.Y, false));
        }
    }
}
