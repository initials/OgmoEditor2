using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OgmoEditor.Definitions.LayerDefinitions;
using System.IO;
using System.Diagnostics;

namespace OgmoEditor.Windows
{
    public partial class LayerButton : UserControl
    {
        static private readonly OgmoColor NotSelected = new OgmoColor(220, 220, 220);
        static private readonly OgmoColor Selected = new OgmoColor(255, 125, 50);
        static private readonly OgmoColor Hover = new OgmoColor(255, 220, 130);

        public LayerDefinition LayerDefinition { get; private set; }
        private bool selected;

        public LayerButton(LayerDefinition definition, int y)
        {
            LayerDefinition = definition;
            InitializeComponent();
            Location = new Point(0, y);
            pictureBox.Image = Image.FromFile(Path.Combine(Ogmo.ProgramDirectory, @"Content\layers", LayerDefinition.Image));
            layerNameLabel.Text = definition.Name;

            //Init state
            selected = Ogmo.LayersWindow.CurrentLayerIndex != -1 && Ogmo.Project.LayerDefinitions[Ogmo.LayersWindow.CurrentLayerIndex] == LayerDefinition;
            layerNameLabel.BackColor = selected ? Selected : NotSelected;
            visibleCheckBox.Checked = LayerDefinition.Visible;

            //Add events
            Ogmo.LayersWindow.OnLayerChanged += onLayerChanged;          
        }

        public void OnRemove()
        {
            //Clean up events
            Ogmo.LayersWindow.OnLayerChanged -= onLayerChanged;
        }

        private void onLayerChanged(LayerDefinition layer, int index)
        {
            selected = layer == LayerDefinition;
            layerNameLabel.BackColor = selected ? Selected : NotSelected;
        }

        private void visibleCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            LayerDefinition.Visible = visibleCheckBox.Checked;
        }

        private void layerNameLabel_MouseEnter(object sender, EventArgs e)
        {
            if (!selected)
                layerNameLabel.BackColor = Hover;
        }

        private void layerNameLabel_MouseLeave(object sender, EventArgs e)
        {
            if (!selected)
                layerNameLabel.BackColor = NotSelected;
        }

        private void layerNameLabel_Click(object sender, EventArgs e)
        {
            Ogmo.LayersWindow.SetLayer(Ogmo.Project.LayerDefinitions.IndexOf(LayerDefinition));
        }
    }
}
