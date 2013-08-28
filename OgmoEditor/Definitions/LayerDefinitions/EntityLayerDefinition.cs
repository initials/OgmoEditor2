using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OgmoEditor.LevelData.Layers;
using OgmoEditor.LevelData;
using System.Windows.Forms;

namespace OgmoEditor.Definitions.LayerDefinitions
{
    public class EntityLayerDefinition : LayerDefinition
    {
        public EntityLayerDefinition()
            : base()
        {
            Image = "entity.png";
        }

        public override UserControl GetEditor()
        {
            return null;
        }

        public override Layer GetInstance(Level level)
        {
            return new EntityLayer(level, this);
        }

        public override LayerDefinition Clone()
        {
            EntityLayerDefinition def = new EntityLayerDefinition();
            def.Name = Name;
            def.Grid = Grid;
            def.ScrollFactor = ScrollFactor;
            return def;
        }
    }
}
