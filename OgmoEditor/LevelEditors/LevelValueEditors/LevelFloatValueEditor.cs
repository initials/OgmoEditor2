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
    public partial class LevelFloatValueEditor : ValueEditor
    {
        public FloatValueDefinition Definition { get; private set; }

        public LevelFloatValueEditor(Value value, int x, int y)
            : base(value, x, y)
        {
            Definition = (FloatValueDefinition)value.Definition;
            InitializeComponent();

            nameLabel.Text = Definition.Name;
            valueTextBox.Text = Value.Content;
        }

        private void handleTextBox()
        {
            string temp = Value.Content;
            OgmoParse.ParseFloatToString(ref temp, Definition.Min, Definition.Max, Definition.Round, valueTextBox);
            if (temp != Value.Content)
                Ogmo.MainWindow.LevelEditors[Ogmo.CurrentLevelIndex].Perform(
                        new EntitySetValueAction(null, Value, temp)
                    );
        }

        /*
         *  Events
         */
        private void valueTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                handleTextBox();
        }

        private void valueTextBox_Leave(object sender, EventArgs e)
        {
            handleTextBox();
        }
    }
}
