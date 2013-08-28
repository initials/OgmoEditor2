using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OgmoEditor.LevelData.Layers;
using OgmoEditor.LevelEditors.Actions.EntityActions;
using OgmoEditor.Clipboard;

namespace OgmoEditor.LevelEditors.LayerEditors
{
    public class EntityLayerEditor : LayerEditor
    {
        public new EntityLayer Layer { get; private set; }

        public EntityLayerEditor(LevelEditor levelEditor, EntityLayer layer)
            : base(levelEditor, layer)
        {
            Layer = layer;
        }

        public override void Draw(System.Drawing.Graphics graphics, bool current, bool fullAlpha)
        {
            foreach (Entity e in Layer.Entities)
                e.Draw(graphics, current, fullAlpha);
        }

        public override void OnKeyDown(System.Windows.Forms.Keys key)
        {
            base.OnKeyDown(key);

            if (key == System.Windows.Forms.Keys.Delete)
            {
                if (Ogmo.EntitySelectionWindow.AmountSelected > 0)
                    LevelEditor.Perform(new EntityRemoveAction(Layer, Ogmo.EntitySelectionWindow.Selected));
            }
        }

        public override bool CanCopyOrCut
        {
            get
            {
                return Ogmo.EntitySelectionWindow.Selected.Count > 0;
            }
        }

        public override void Copy()
        {
            Ogmo.Clipboard = new EntityClipboardItem(Ogmo.EntitySelectionWindow.Selected);
        }

        public override void Cut()
        {
            Copy();
            LevelEditor.Perform(new EntityRemoveAction(Layer, Ogmo.EntitySelectionWindow.Selected));
        }

        public override bool CanSelectAll
        {
            get
            {
                return Layer.Entities.Count > 0;
            }
        }

        public override void SelectAll()
        {
            Ogmo.EntitySelectionWindow.SetSelection(Layer.Entities);
        }

        public override bool CanDeselect
        {
            get
            {
                return Layer.Entities.Count > 0;
            }
        }

        public override void Deselect()
        {
            Ogmo.EntitySelectionWindow.ClearSelection();
        }
    }
}
