using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OgmoEditor.LevelData.Layers;
using OgmoEditor.LevelEditors.Resizers;
using OgmoEditor.LevelEditors.Actions.TileActions;
using System.Drawing;
using OgmoEditor.Clipboard;
using System.Drawing.Imaging;
using System.Diagnostics;

namespace OgmoEditor.LevelEditors.LayerEditors
{
    public class TileLayerEditor : LayerEditor
    {
        public new TileLayer Layer { get; private set; }

        public TileLayerEditor(LevelEditor levelEditor, TileLayer layer)
            : base(levelEditor, layer)
        {
            Layer = layer;
        }

        public override void Draw(Graphics graphics, bool current, bool fullAlpha)
        {
            graphics.DrawImage(Layer.Bitmap, new Rectangle(0, 0, Layer.Bitmap.Width, Layer.Bitmap.Height), 0, 0, Layer.Bitmap.Width, Layer.Bitmap.Height, GraphicsUnit.Pixel, fullAlpha ? Util.FullAlphaAttributes : Util.HalfAlphaAttributes);

            //Draw the selection box
            if (current && Layer.Selection != null)
                graphics.DrawSelectionRectangle(new Rectangle(
                    Layer.Selection.Area.X * Layer.Definition.Grid.Width,
                    Layer.Selection.Area.Y * Layer.Definition.Grid.Height,
                    Layer.Selection.Area.Width * Layer.Definition.Grid.Width,
                    Layer.Selection.Area.Height * Layer.Definition.Grid.Height));
        }

        public override Resizer GetResizer()
        {
            return new TileResizer(this);
        }

        public override bool CanSelectAll
        {
            get
            {
                return true;
            }
        }

        public override void SelectAll()
        {
            LevelEditor.Perform(new TileSelectAction(Layer, new Rectangle(0, 0, Layer.TileCellsX, Layer.TileCellsY)));
        }

        public override bool CanDeselect
        {
            get
            {
                return Layer.Selection != null;
            }
        }

        public override void Deselect()
        {
            LevelEditor.Perform(new TileClearSelectionAction(Layer));
        }

        public override bool CanCopyOrCut
        {
            get
            {
                return Layer.Selection != null;
            }
        }

        public override void Copy()
        {
            Ogmo.Clipboard = new TileClipboardItem(Layer.Selection.Area, Layer);
        }

        public override void Cut()
        {
            Copy();
            LevelEditor.Perform(new TileDeleteSelectionAction(Layer));
        }
    }
}
