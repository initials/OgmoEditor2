﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OgmoEditor.Definitions.LayerDefinitions;
using System.Diagnostics;

namespace OgmoEditor.ProjectEditors.LayerDefinitionEditors
{
    public partial class GridLayerDefinitionEditor : UserControl
    {
        private GridLayerDefinition def;

        public GridLayerDefinitionEditor(GridLayerDefinition def)
        {
            this.def = def;
            InitializeComponent();
            Location = new Point(206, 128);

            colorChooser.Color = def.Color;
            exportModeComboBox.SelectedIndex = (int)def.ExportMode;
        }

        private void colorChooser_ColorChanged(OgmoColor color)
        {
            def.Color = color;
        }

        private void exportModeComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            def.ExportMode = (GridLayerDefinition.ExportModes)exportModeComboBox.SelectedIndex;
        }

        private void exportModeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
