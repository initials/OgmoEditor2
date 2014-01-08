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
        public TextBox initWalls;

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
            ClientSize = new Size(120, project.LayerDefinitions.Count * 24);

            initWalls = new TextBox();
            initWalls.Text = "0.5";
            Controls.Add(initWalls);

        }
    }
}
