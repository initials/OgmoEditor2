using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using OgmoEditor.ProjectEditors;

namespace OgmoEditor
{
    public partial class ColorChooser : UserControl
    {
        private OgmoColor color;
        public delegate void ColorCallback(OgmoColor color);
        public event ColorCallback ColorChanged;

        public ColorChooser()
        {
            InitializeComponent();
            color = new OgmoColor(255, 255, 255);
            hexTextBox.Text = color.ToString();
            colorButton.BackColor = color;
        }

        public OgmoColor Color
        {
            get { return color; }
            set
            {
                if (color != value)
                {
                    color = value;
                    if (ColorChanged != null)
                        ColorChanged(color);

                    hexTextBox.Text = color.ToString();
                    colorButton.BackColor = color;
                }
            }
        }

        private void colorButton_Click(object sender, EventArgs e)
        {
            //Show the dialog
            ColorDialog dialog = new ColorDialog();
            dialog.Color = color;
            dialog.AllowFullOpen = true;

            //Handle cancel
            if (dialog.ShowDialog() == DialogResult.Cancel)
                return;

            //Get the new color
            Color = (OgmoColor)dialog.Color;
        }

        private void hexTextBox_Validated(object sender, EventArgs e)
        {
            OgmoColor col = color;
            OgmoParse.Parse(ref col, hexTextBox);
            Color = col;
        }

        private void hexTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                hexTextBox_Validated(sender, e);
        }
    }
}
