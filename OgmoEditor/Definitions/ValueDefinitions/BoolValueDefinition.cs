using System.Xml.Serialization;
using OgmoEditor.LevelData.Layers;
using OgmoEditor.LevelEditors.LevelValueEditors;
using OgmoEditor.LevelEditors.ValueEditors;
using OgmoEditor.ProjectEditors.ValueDefinitionEditors;

namespace OgmoEditor.Definitions.ValueDefinitions
{
    public class BoolValueDefinition : ValueDefinition
    {
        [XmlAttribute]
        public bool Default;

        public BoolValueDefinition()
            : base()
        {
            Default = false;
        }

        public override System.Windows.Forms.UserControl GetEditor()
        {
            return new BoolValueDefinitionEditor(this);
        }

        public override ValueEditor GetInstanceEditor(Value instance, int x, int y)
        {
            return new BoolValueEditor(instance, x, y);
        }

        public override ValueEditor GetInstanceLevelEditor(Value instance, int x, int y)
        {
            return new LevelBoolValueEditor(instance, x, y);
        }

        public override ValueDefinition Clone()
        {
            BoolValueDefinition def = new BoolValueDefinition();
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
            return Name + " (bool)";
        }
    }
}
