using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OgmoEditor.LevelEditors.Actions
{
    public class ActionBatch : OgmoAction
    {
        private List<OgmoAction> actions;

        public ActionBatch()
        {
            actions = new List<OgmoAction>();
        }

        public void Add(OgmoAction action)
        {
            actions.Add(action);
        }

        public override void Do()
        {
            base.Do();

            for (int i = 0; i < actions.Count; i++)
                actions[i].Do();
        }

        public override void Undo()
        {
            base.Undo();

            for (int i = actions.Count - 1; i >= 0; i--)
                actions[i].Undo();
        }
    }
}
