using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OgmoEditor.LevelData.Layers;
using System.Drawing;

namespace OgmoEditor.LevelEditors.Actions.TileActions
{
    public class TileSelectAction : TileAction
    {
        private TileSelection oldSelection;
        private Rectangle selectArea;

        public TileSelectAction(TileLayer tileLayer, Rectangle selectArea)
            : base(tileLayer)
        {
            this.selectArea = selectArea;
        }

        public override void Do()
        {
            base.Do();

            oldSelection = TileLayer.Selection;
            TileLayer.Selection = new TileSelection(TileLayer, selectArea);
        }

        public override void Undo()
        {
            base.Undo();

            TileLayer.Selection = oldSelection;
        }
    }
}
