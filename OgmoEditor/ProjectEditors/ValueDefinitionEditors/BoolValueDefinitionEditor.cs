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
    public partial class BoolValueDefinitionEditor : UserControl
    {
        private BoolValueDefinition def;

        public BoolValueDefinitionEditor(BoolValueDefinition def)
        {
            this.def = def;
            InitializeComponent();
            Location = new Point(99, 53);

            defaultCheckBox.Checked = def.Default;
        }

        private void defaultCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            def.Default = defaultCheckBox.Checked;
        }
    }
}
