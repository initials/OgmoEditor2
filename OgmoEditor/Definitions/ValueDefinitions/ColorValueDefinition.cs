using OgmoEditor.LevelData.Layers;
using OgmoEditor.LevelEditors.LevelValueEditors;
using OgmoEditor.LevelEditors.ValueEditors;
using OgmoEditor.ProjectEditors.ValueDefinitionEditors;

namespace OgmoEditor.Definitions.ValueDefinitions
{
    public class ColorValueDefinition : ValueDefinition
    {
        public OgmoColor Default;

        public ColorValueDefinition()
            : base()
        {
            Default = new OgmoColor(255, 255, 255);
        }

        public override System.Windows.Forms.UserControl GetEditor()
        {
            return new ColorValueDefinitionEditor(this);
        }

        public override ValueEditor GetInstanceEditor(Value instance, int x, int y)
        {
            return new ColorValueEditor(instance, x, y);
        }

        public override ValueEditor GetInstanceLevelEditor(Value instance, int x, int y)
        {
            return new LevelColorValueEditor(instance, x, y);
        }

        public override ValueDefinition Clone()
        {
            ColorValueDefinition def = new ColorValueDefinition();
            def.Name = Name;
            def.Default = Default;
            return def;
        }

        public override string GetDefault()
        {
            return Default.ToString();
        }

        public override string ToString()
        {
            return Name + " (color)";
        }
    }
}
