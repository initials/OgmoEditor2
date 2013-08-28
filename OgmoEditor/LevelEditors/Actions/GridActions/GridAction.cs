using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OgmoEditor.LevelData.Layers;

namespace OgmoEditor.LevelEditors.Actions.GridActions
{
    public abstract class GridAction : OgmoAction
    {
        public GridLayer GridLayer { get; private set; }

        public GridAction(GridLayer gridLayer)
        {
            GridLayer = gridLayer;
        }
    }
}
