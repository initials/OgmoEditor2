using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OgmoEditor.Definitions.ValueDefinitions;
using OgmoEditor.LevelData.Layers;
using OgmoEditor.LevelEditors.Actions.EntityActions;

namespace OgmoEditor.LevelEditors.ValueEditors
{
    public partial class ColorValueEditor : ValueEditor
    {
        public ColorValueDefinition Definition { get; private set; }

        public ColorValueEditor(Value value, int x, int y)
            : base(value, x, y)
        {
            Definition = (ColorValueDefinition)value.Definition;
            InitializeComponent();

            nameLabel.Text = Definition.Name;
            colorChooser.Color = Value.Content;
        }

        /*
         *  Events
         */
        private void colorChooser_ColorChanged(OgmoColor color)
        {
            if (colorChooser.Color.ToString() != Value.Content)
                Ogmo.MainWindow.LevelEditors[Ogmo.CurrentLevelIndex].Perform(
                        new EntitySetValueAction(null, Value, colorChooser.Color.ToString())
                    );
        }
    }
}
