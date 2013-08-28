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
    public partial class FloatValueDefinitionEditor : UserControl
    {
        private FloatValueDefinition def;

        public FloatValueDefinitionEditor(FloatValueDefinition def)
        {
            this.def = def;
            InitializeComponent();
            Location = new Point(99, 53);

            defaultTextBox.Text = def.Default.ToString();
            roundTextBox.Text = def.Round.ToString();
            minTextBox.Text = def.Min.ToString();
            maxTextBox.Text = def.Max.ToString();
        }

        private void defaultTextBox_Validated(object sender, EventArgs e)
        {
            OgmoParse.Parse(ref def.Default, defaultTextBox);
        }

        private void roundTextBox_Validated(object sender, EventArgs e)
        {
            OgmoParse.Parse(ref def.Round, roundTextBox);
        }

        private void minTextBox_Validated(object sender, EventArgs e)
        {
            OgmoParse.Parse(ref def.Min, minTextBox);
        }

        private void maxTextBox_Validated(object sender, EventArgs e)
        {
            OgmoParse.Parse(ref def.Max, maxTextBox);
        }
    }
}
