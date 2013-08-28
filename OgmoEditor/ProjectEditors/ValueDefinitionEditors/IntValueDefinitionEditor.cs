using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OgmoEditor.Definitions.ValueDefinitions;

namespace OgmoEditor.ProjectEditors.ValueDefinitionEditors
{
    public partial class IntValueDefinitionEditor : UserControl
    {
        private IntValueDefinition def;

        public IntValueDefinitionEditor(IntValueDefinition def)
        {
            this.def = def;
            InitializeComponent();
            Location = new Point(99, 53);

            defaultTextBox.Text = def.Default.ToString();
            minTextBox.Text = def.Min.ToString();
            maxTextBox.Text = def.Max.ToString();
            sliderCheckBox.Checked = def.ShowSlider;
        }

        private void defaultTextBox_Validated(object sender, EventArgs e)
        {
            OgmoParse.Parse(ref def.Default, defaultTextBox);
        }

        private void minTextBox_Validated(object sender, EventArgs e)
        {
            OgmoParse.Parse(ref def.Min, minTextBox);
        }

        private void maxTextBox_TextChanged(object sender, EventArgs e)
        {
            OgmoParse.Parse(ref def.Max, maxTextBox);
        }

        private void sliderCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            def.ShowSlider = sliderCheckBox.Checked;
        }
    }
}
