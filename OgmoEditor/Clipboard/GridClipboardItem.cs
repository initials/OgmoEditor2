using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OgmoEditor.LevelData.Layers;
using OgmoEditor.LevelEditors;
using OgmoEditor.LevelEditors.Actions.GridActions;
using System.Drawing;

namespace OgmoEditor.Clipboard
{
    public class GridClipboardItem : ClipboardItem
    {
        public Rectangle Area;
        public bool[,] Bits;

        public GridClipboardItem(Rectangle area, GridLayer layer)
            : base()
        {
            Area = area;

            Bits = new bool[Area.Width, Area.Height];
            for (int i = 0; i < Area.Width; i++)
                for (int j = 0; j < Area.Height; j++)
                    Bits[i, j] = layer.Grid[i + Area.X, j + Area.Y];
        }

        public override bool CanPaste(Layer layer)
        {
            return layer is GridLayer;
        }

        public override void Paste(LevelEditor editor, Layer layer)
        {
            editor.Perform(new GridPasteSelectionAction(layer as GridLayer, Area, Bits));
        }
    }
}
