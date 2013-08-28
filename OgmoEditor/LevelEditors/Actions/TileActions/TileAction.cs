using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OgmoEditor.LevelData.Layers;

namespace OgmoEditor.LevelEditors.Actions.TileActions
{
    public abstract class TileAction : OgmoAction
    {
        public TileLayer TileLayer { get; private set; }

        public TileAction(TileLayer tileLayer)
        {
            TileLayer = tileLayer;
        }
    }
}
