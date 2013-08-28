using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OgmoEditor.LevelData.Layers;

namespace OgmoEditor.LevelEditors.Actions
{
    public abstract class OgmoAction
    {
        public bool LevelWasChanged = true;
        public bool Performed { get; private set; }

        public virtual void Do() { Performed = true; }
        public virtual void Undo() { Performed = false; }
    }
}
