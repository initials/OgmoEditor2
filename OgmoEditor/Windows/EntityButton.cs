using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OgmoEditor.Definitions;
using OgmoEditor.LevelEditors.Tools.EntityTools;

namespace OgmoEditor.Windows
{
    public partial class EntityButton : UserControl
    {
        static private readonly OgmoColor NotSelected = new OgmoColor(220, 220, 220);
        static private readonly OgmoColor Selected = new OgmoColor(255, 125, 50);
        static private readonly OgmoColor Hover = new OgmoColor(255, 220, 130);

        public EntityDefinition Definition { get; private set; }

        public EntityButton(EntityDefinition definition, int x, int y)
        {
            Definition = definition;
            InitializeComponent();
            Location = new Point(x, y);
            toolTip.SetToolTip(this, definition.Name);

            pictureBox.Paint += new PaintEventHandler(pictureBox_Paint);

            entityNameLabel.Text = definition.Name;
            entityNameLabel.BackColor = (definition == Ogmo.EntitiesWindow.CurrentEntity) ? Selected : NotSelected;

            //Events
            Ogmo.EntitiesWindow.OnEntityChanged += onEntityChanged;
        }

        public void OnRemove()
        {
            Ogmo.EntitiesWindow.OnEntityChanged -= onEntityChanged;
        }

        #region Events

        private void pictureBox_Paint(object sender, PaintEventArgs e)
        {
            Image image = Definition.ButtonBitmap;
            float scale = Math.Min(ClientSize.Width / (float)image.Width, ClientSize.Height / (float)image.Height);
            int destWidth = (int)(image.Width * scale);
            int destHeight = (int)(image.Height * scale);

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            e.Graphics.DrawImage(image,
                new Rectangle(pictureBox.ClientSize.Width / 2 - destWidth / 2, pictureBox.ClientSize.Height / 2 - destHeight / 2, destWidth, destHeight),
                new Rectangle(0, 0, image.Width, image.Height), GraphicsUnit.Pixel);
        }

        private void label_Click(object sender, EventArgs e)
        {
            Ogmo.EntitiesWindow.SetObject(Definition);
            Ogmo.ToolsWindow.SetTool(typeof(EntityPlacementTool));
            Ogmo.MainWindow.LevelEditors[Ogmo.CurrentLevelIndex].Focus();
        }

        private void onEntityChanged(EntityDefinition definition)
        {
            if (definition == Definition)
            {
                entityNameLabel.BackColor = Selected;
                Select();
            }
            else
                entityNameLabel.BackColor = NotSelected;
        }

        private void entityNameLabel_MouseEnter(object sender, EventArgs e)
        {
            if (entityNameLabel.BackColor != Selected)
                entityNameLabel.BackColor = Hover;
        }

        private void entityNameLabel_MouseLeave(object sender, EventArgs e)
        {
            if (entityNameLabel.BackColor == Hover)
                entityNameLabel.BackColor = NotSelected;
        }

        #endregion
    }
}
