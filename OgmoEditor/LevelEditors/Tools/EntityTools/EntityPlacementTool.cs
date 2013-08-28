using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OgmoEditor.LevelData.Layers;
using OgmoEditor.LevelEditors.Actions.EntityActions;

namespace OgmoEditor.LevelEditors.Tools.EntityTools
{
    public class EntityPlacementTool : EntityTool
    {
        public EntityPlacementTool()
            : base("Create", "pencil.png")
        {

        }

        public override void Draw(System.Drawing.Graphics graphics)
        {
            if (Ogmo.EntitiesWindow.CurrentEntity != null && LevelEditor.Focused)
                Ogmo.EntitiesWindow.CurrentEntity.Draw(graphics, Util.Ctrl ? LevelEditor.MousePosition : LayerEditor.MouseSnapPosition, 0, DrawUtil.AlphaMode.Half);
        }

        public override void OnMouseLeftClick(System.Drawing.Point location)
        {
            if (Ogmo.EntitiesWindow.CurrentEntity != null)
                LevelEditor.Perform(new EntityAddAction(LayerEditor.Layer, new Entity(LayerEditor.Layer, Ogmo.EntitiesWindow.CurrentEntity, Util.Ctrl ? LevelEditor.MousePosition : LayerEditor.MouseSnapPosition)));
        }
    }
}
