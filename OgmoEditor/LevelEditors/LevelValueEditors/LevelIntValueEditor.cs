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
    public partial class LevelIntValueEditor : ValueEditor
    {   
        public IntValueDefinition Definition { get; private set; }

        public LevelIntValueEditor(Value value, int x, int y)
            : base(value, x, y)
        {
            Definition = (IntValueDefinition)value.Definition;
            InitializeComponent();

            nameLabel.Text = Definition.Name;
            valueTextBox.Text = Value.Content;

            //Deal with the slider
            if (Definition.ShowSlider)
            {
                valueTrackBar.Minimum = Definition.Min;
                valueTrackBar.Maximum = Definition.Max;
                valueTrackBar.Value = Convert.ToInt32(Value.Content);
                valueTrackBar.TickFrequency = (Definition.Max - Definition.Min) / 10;
            }
            else
            {
                Controls.Remove(valueTrackBar);
                valueTrackBar = null;
            }
        }

        private void handleTextBox()
        {
            string temp = Value.Content;
            OgmoParse.ParseIntToString(ref temp, Definition.Min, Definition.Max, valueTextBox);
            if (temp != Value.Content)
            {
                if (valueTrackBar != null)
                    valueTrackBar.Value = Convert.ToInt32(temp);
                Ogmo.MainWindow.LevelEditors[Ogmo.CurrentLevelIndex].Perform(
                        new EntitySetValueAction(null, Value, temp)
                    );
            }
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

        private void valueTrackBar_Scroll(object sender, EventArgs e)
        {
            if (valueTrackBar.Value.ToString() != Value.Content)
            {
                valueTextBox.Text = valueTrackBar.Value.ToString();
                Ogmo.MainWindow.LevelEditors[Ogmo.CurrentLevelIndex].Perform(
                        new EntitySetValueAction(null, Value, valueTrackBar.Value.ToString())
                    );
            }
        }
    }
}
