using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OgmoEditor.LevelData.Layers;
using OgmoEditor.LevelEditors;

namespace OgmoEditor.Clipboard
{
    public abstract class ClipboardItem
    {
        public abstract bool CanPaste(Layer layer);
        public abstract void Paste(LevelEditor editor, Layer layer);
    }
}
