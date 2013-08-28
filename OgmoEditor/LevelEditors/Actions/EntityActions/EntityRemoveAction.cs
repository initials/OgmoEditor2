using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OgmoEditor.LevelData.Layers;

namespace OgmoEditor.LevelEditors.Actions.EntityActions
{
    public class EntityRemoveAction : EntityAction
    {
        private List<Entity> toRemove;

        public EntityRemoveAction(EntityLayer entityLayer, List<Entity> entities)
            : base(entityLayer)
        {
            toRemove = entities;
        }

        public override void Do()
        {
            base.Do();

            foreach (var e in toRemove)
                EntityLayer.Entities.Remove(e);

            Ogmo.EntitySelectionWindow.RemoveFromSelection(toRemove);
        }

        public override void Undo()
        {
            base.Undo();

            foreach (var e in toRemove)
                EntityLayer.Entities.Add(e);
        }
    }
}
