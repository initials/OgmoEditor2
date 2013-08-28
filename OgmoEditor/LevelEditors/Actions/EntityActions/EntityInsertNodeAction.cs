using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OgmoEditor.LevelData.Layers;
using System.Drawing;

namespace OgmoEditor.LevelEditors.Actions.EntityActions
{
    public class EntityInsertNodeAction : EntityAction
    {
        private Entity entity;
        private Point node;
        private int index;

        public EntityInsertNodeAction(EntityLayer entityLayer, Entity entity, Point node, int index)
            : base(entityLayer)
        {
            this.entity = entity;
            this.node = node;
            this.index = index;
        }

        public override void Do()
        {
            base.Do();

            entity.Nodes.Insert(index, node);
        }

        public override void Undo()
        {
            base.Undo();

            entity.Nodes.RemoveAt(index);
        }
    }
}
