using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OgmoEditor.LevelData.Layers;

namespace OgmoEditor.LevelEditors.Actions.EntityActions
{
    public abstract class EntityAction : OgmoAction
    {
        public EntityLayer EntityLayer { get; private set; }

        public EntityAction(EntityLayer entityLayer)
        {
            EntityLayer = entityLayer;
        }
    }
}
