using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OgmoEditor.LevelData;

namespace OgmoEditor.LevelEditors.Actions.LevelActions
{
    public abstract class LevelAction : OgmoAction
    {
        public Level Level { get; private set; }

        public LevelAction(Level level)
        {
            Level = level;
        }
    }
}
