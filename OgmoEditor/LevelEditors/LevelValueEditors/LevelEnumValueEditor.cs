using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OgmoEditor.LevelEditors.ValueEditors;
using OgmoEditor.LevelData.Layers;
using OgmoEditor.Definitions.ValueDefinitions;
using OgmoEditor.LevelEditors.Actions.EntityActions;

namespace OgmoEditor.LevelEditors.LevelValueEditors
{
    public partial class LevelEnumValueEditor : ValueEditor
    {
        public EnumValueDefinition Definition { get; private set; }

        public LevelEnumValueEditor(Value value, int x, int y)
            : base(value, x, y)
        {
            Definition = (EnumValueDefinition)value.Definition;
            InitializeComponent();

            nameLabel.Text = Definition.Name;

            //Init the combo box
            for (int i = 0; i < Definition.Elements.Length; i++)
            {
                valueComboBox.Items.Add(Definition.Elements[i]);
                if (Value.Content == Definition.Elements[i])
                    valueComboBox.SelectedIndex = i;
            }
        }

        /*
         *  Events
         */
        private void valueComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (Definition.Elements[valueComboBox.SelectedIndex] != Value.Content)
                Ogmo.MainWindow.LevelEditors[Ogmo.CurrentLevelIndex].Perform(
                        new EntitySetValueAction(null, Value, Definition.Elements[valueComboBox.SelectedIndex])
                    );
        }
    }
}
