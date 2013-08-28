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
using OgmoEditor.ProjectEditors;
using OgmoEditor.LevelEditors.Actions.EntityActions;

namespace OgmoEditor.LevelEditors.ValueEditors
{
    public partial class StringValueEditor : ValueEditor
    {
        public StringValueDefinition Definition { get; private set; }

        public StringValueEditor(Value value, int x, int y)
            : base(value, x, y)
        {
            Definition = (StringValueDefinition)value.Definition;
            InitializeComponent();

            //Init the textbox
            if (Definition.MultiLine)
            {
                valueTextBox.Multiline = true;
                valueTextBox.Size = new Size(valueTextBox.Width, valueTextBox.Height * 3);
                Size = new Size(128, 96);
            }
            nameLabel.Text = Definition.Name;

            valueTextBox.Text = Value.Content;
        }

        private void handleTextBox()
        {
            string temp = Value.Content;
            OgmoParse.ParseString(ref temp, Definition.MaxChars, valueTextBox);
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
