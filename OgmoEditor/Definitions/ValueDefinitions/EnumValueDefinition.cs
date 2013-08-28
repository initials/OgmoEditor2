using OgmoEditor.LevelData.Layers;
using OgmoEditor.LevelEditors.LevelValueEditors;
using OgmoEditor.LevelEditors.ValueEditors;
using OgmoEditor.ProjectEditors.ValueDefinitionEditors;

namespace OgmoEditor.Definitions.ValueDefinitions
{
    public class EnumValueDefinition : ValueDefinition
    {
        public string[] Elements;

        public EnumValueDefinition()
            : base()
        {
            Elements = new string[] { "default" };
        }

        public override System.Windows.Forms.UserControl GetEditor()
        {
            return new EnumValueDefinitionEditor(this);
        }

        public override ValueEditor GetInstanceEditor(Value instance, int x, int y)
        {
            return new EnumValueEditor(instance, x, y);
        }

        public override ValueEditor GetInstanceLevelEditor(Value instance, int x, int y)
        {
            return new LevelEnumValueEditor(instance, x, y);
        }

        public override ValueDefinition Clone()
        {
            EnumValueDefinition def = new EnumValueDefinition();
            def.Name = Name;
            def.Elements = (string[])Elements.Clone();
            return def;
        }

        public override string GetDefault()
        {
            return Elements[0];
        }

        public override string ToString()
        {
            return Name + " (enum)";
        }
    }
}
