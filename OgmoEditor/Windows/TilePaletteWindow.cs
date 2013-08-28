using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using OgmoEditor.Definitions.LayerDefinitions;
using OgmoEditor.LevelData.Layers;
using System.Windows.Forms;
using OgmoEditor.Definitions;
using System.Diagnostics;
using OgmoEditor.LevelEditors.Actions.TileActions;

namespace OgmoEditor.Windows
{
    public class TilePaletteWindow : OgmoWindow
    {
        private ComboBox tilesetsComboBox;
        private TileSelector tileSelector;

        public TilePaletteWindow()
            : base(HorizontalSnap.Left, VerticalSnap.Bottom)
        {
            Name = "TilePaletteWindow";
            Text = "Tile Palette";
            ClientSize = new Size(160, 192);

            FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;

            tilesetsComboBox = new ComboBox();
            tilesetsComboBox.Location = new Point(48, 4);
            tilesetsComboBox.Width = 104;
            tilesetsComboBox.Enabled = false;
            tilesetsComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            tilesetsComboBox.SelectionChangeCommitted += new EventHandler(tilesetsComboBox_SelectionChangeCommitted);
            Controls.Add(tilesetsComboBox);

            Label tilesetsLabel = new Label();
            tilesetsLabel.Text = "Tileset:";
            tilesetsLabel.Location = new Point(4, 7);
            tilesetsLabel.Size = new Size(44, 20);
            Controls.Add(tilesetsLabel);

            tileSelector = new TileSelector();
            tileSelector.Location = new Point(4, 27);
            tileSelector.Size = new Size(ClientSize.Width - 8, ClientSize.Height - 31);
            tileSelector.Dock = DockStyle.Bottom;
            Controls.Add(tileSelector);

            Resize += new EventHandler(TilePaletteWindow_ResizeEnd);
            Ogmo.LayersWindow.OnLayerChanged += onLayerChanged;
            Ogmo.OnProjectStart += initFromProject;
            Ogmo.OnProjectEdited += initFromProject;
        }

        public override bool ShouldBeVisible()
        {
            return Ogmo.LayersWindow.CurrentLayer is TileLayer;
        }

        public void initFromProject(Project project)
        {
            tilesetsComboBox.Items.Clear();
            foreach (Tileset t in Ogmo.Project.Tilesets)
                tilesetsComboBox.Items.Add(t);
            tilesetsComboBox.SelectedIndex = (Ogmo.Project.Tilesets.Count > 0) ? 0 : -1;
            tilesetsComboBox.Enabled = (Ogmo.Project.Tilesets.Count > 1);

            if (project.Tilesets.Count > 0)
                SetTileset(project.Tilesets[0]);
        }

        public Rectangle? Tiles
        {
            get { return tileSelector.Selection; }
        }

        public Point? TilesStart
        {
            get
            {
                if (tileSelector.Selection.HasValue)
                    return new Point(tileSelector.Selection.Value.X, tileSelector.Selection.Value.Y);
                else
                    return null;
            }
        }

        public int TilesStartID
        {
            get
            {
                Point? start = TilesStart;
                if (start.HasValue)
                    return tileSelector.Tileset.GetIDFromCell(start.Value);
                else
                    return -1;
            }

            set
            {
                tileSelector.SetSelectionID(value);
            }
        }

        public void SetTileset(Tileset to)
        {
            tilesetsComboBox.SelectedItem = to;
            tileSelector.Tileset = to;
        }

        /*
         *  Events
         */
        protected override void handleKeyDown(KeyEventArgs e)
        {
            if (EditorVisible)
            {
                if (e.KeyCode == Keys.A)
                    tileSelector.MoveSelectionLeft();
                else if (e.KeyCode == Keys.D)
                    tileSelector.MoveSelectionRight();
                else if (e.KeyCode == Keys.W)
                    tileSelector.MoveSelectionUp();
                else if (e.KeyCode == Keys.S)
                    tileSelector.MoveSelectionDown();
            }
        }

        private void onLayerChanged(LayerDefinition layerDefinition, int index)
        {
            EditorVisible = layerDefinition is TileLayerDefinition;
            if (EditorVisible && Ogmo.LayersWindow.CurrentLayer != null)
            {
                tilesetsComboBox.SelectedIndex = Ogmo.Project.Tilesets.IndexOf((Ogmo.LayersWindow.CurrentLayer as TileLayer).Tileset);
                tileSelector.Tileset = (Ogmo.LayersWindow.CurrentLayer as TileLayer).Tileset;
            }
        }

        private void TilePaletteWindow_ResizeEnd(object sender, EventArgs e)
        {
            tileSelector.Size = new Size(ClientSize.Width - 8, ClientSize.Height - 34);
        }

        private void  tilesetsComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            SetTileset(tilesetsComboBox.SelectedItem as Tileset);
            Ogmo.MainWindow.LevelEditors[Ogmo.CurrentLevelIndex].Perform(new TileSetTilesetAction(Ogmo.LayersWindow.CurrentLayer as TileLayer, tilesetsComboBox.SelectedItem as Tileset));

            Ogmo.MainWindow.FocusEditor();
        }
    }
}
