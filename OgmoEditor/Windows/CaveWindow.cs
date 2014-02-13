using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OgmoEditor.LevelData.Layers;
using System.Diagnostics;

namespace OgmoEditor.Windows
{
    public partial class CaveWindow : OgmoWindow
    {
        public int CurrentLayerIndex { get; private set; }

        public Label initWallsLabel;
        public TextBox initWalls;

        public Label chanceOfDecorationLabel;
        public TextBox chanceOfDecoration;

        public CaveWindow()
            : base(HorizontalSnap.Right, VerticalSnap.Bottom)
        {
            Name = "CaveWindow";
            Text = "Cave";
            CurrentLayerIndex = -1;

            //Events
            Ogmo.OnProjectStart += initFromProject;
            Ogmo.OnProjectEdited += initFromProject;
        }

        public override bool ShouldBeVisible()
        {
            return Ogmo.Project.LayerDefinitions.Count > 1;
        }

        private void initFromProject(Project project)
        {
            ClientSize = new Size(160, project.LayerDefinitions.Count * 25);

            initWallsLabel = new Label();
            initWallsLabel.Text = "Ratio";
            initWallsLabel.Bounds = new Rectangle(0, 0, 150, 20);
            Controls.Add(initWallsLabel);

            initWalls = new TextBox();
            initWalls.Text = "0.5";
            initWalls.Bounds = new Rectangle(0, 20, 150, 20);
            Controls.Add(initWalls);



            chanceOfDecorationLabel = new Label();
            chanceOfDecorationLabel.Text = "Decoration Chance";
            chanceOfDecorationLabel.Bounds = new Rectangle(0, 40, 150, 20);
            Controls.Add(chanceOfDecorationLabel);

            chanceOfDecoration = new TextBox();
            chanceOfDecoration.Text = "0.6";
            chanceOfDecoration.Bounds = new Rectangle(0, 60, 150, 20);
            Controls.Add(chanceOfDecoration);



        }
    }
}
