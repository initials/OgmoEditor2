using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OgmoEditor.LevelData.Layers;
using System.Windows.Forms;
using OgmoEditor.LevelEditors.LayerEditors;

namespace OgmoEditor.LevelEditors.Tools.EntityTools
{
    public abstract class EntityTool : Tool
    {
        public EntityTool(string name, string image)
            : base(name, image)
        {

        }

        public EntityLayerEditor LayerEditor
        {
            get { return (EntityLayerEditor)LevelEditor.LayerEditors[Ogmo.LayersWindow.CurrentLayerIndex]; }
        }
    }
}
