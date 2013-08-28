using System;
using System.Collections.Generic;
using System.Deployment.Application;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using OgmoEditor.LevelData;
using OgmoEditor.LevelEditors;
using OgmoEditor.Windows;
using OgmoEditor.Windows.Utilities;

namespace OgmoEditor
{
    public partial class MainWindow : Form
    {
        private const int EDIT_BOUNDS_PADDING = 10;

        public bool EditingGridVisible { get; private set; }
        public List<LevelEditor> LevelEditors { get; private set; }

        private ImageList imageList;
        private int rightClicked = -1;      //After a right-click context menu on a tab is closed, switch to this level

        public MainWindow()
        {
            InitializeComponent();

            //Start maximized?
            if (Properties.Settings.Default.StartMaximized)
                WindowState = FormWindowState.Maximized;

            EditingGridVisible = true;
            LevelEditors = new List<LevelEditor>();

            imageList = new ImageList();
            imageList.Images.Add(Image.FromFile(Path.Combine(Ogmo.ProgramDirectory, @"Content\icons", "icon32.png")));
            imageList.Images.Add(Image.FromFile(Path.Combine(Ogmo.ProgramDirectory, @"Content\icons", "lvl32.png")));
            MasterTabControl.ImageList = imageList;

            AddStartPage();

            Ogmo.OnProjectStart += onProjectStart;
            Ogmo.OnProjectClose += onProjectClose;
            Ogmo.OnLevelAdded += onLevelAdded;
            Ogmo.OnLevelClosed += onLevelClosed;
            Ogmo.OnLevelChanged += onLevelChanged;
        }

        public void AddStartPage()
        {
            TabPage start = new TabPage("Start Page");
            start.Name = "startPage";
            start.Controls.Add(new StartPage());
            start.ImageIndex = 0;
            MasterTabControl.TabPages.Add(start);
        }

        public void RemoveStartPage()
        {
            foreach (TabPage p in MasterTabControl.TabPages)
            {
                if (p.Name == "startPage")
                {
                    MasterTabControl.TabPages.Remove(p);
                    return;
                }
            }
        }

        public void FocusEditor()
        {
            if (LevelEditors.Count > 0)
                LevelEditors[Ogmo.CurrentLevelIndex].Focus();
            else
                Focus();
        }

        public void EnableEditing()
        {
            Enabled = true;
            foreach (var f in OwnedForms)
                f.Enabled = true;
        }

        public void DisableEditing()
        {
            Enabled = false;
            foreach (var f in OwnedForms)
                f.Enabled = false;
        }

        private int SelectedLevelIndex
        {
            get { return MasterTabControl.SelectedIndex; }
            set {  MasterTabControl.SelectedIndex = value; }
        }

        public Rectangle EditBounds
        {
            get
            {
                if (WindowState == FormWindowState.Maximized)
                {
                    int titleBar = Size.Height - ClientSize.Height;
                    return new Rectangle(Location.X + 8 + MasterTabControl.Location.X + EDIT_BOUNDS_PADDING,
                        Location.Y + titleBar + MasterTabControl.Location.Y + 10 + EDIT_BOUNDS_PADDING, 
                        MasterTabControl.Width - (EDIT_BOUNDS_PADDING * 2),
                        MasterTabControl.Height - 20 - (EDIT_BOUNDS_PADDING * 2));
                }
                else
                {
                    int border = (Size.Width - ClientSize.Width) / 2;
                    int titleBar = Size.Height - ClientSize.Height - border;
                    return new Rectangle(
                        Location.X + border + MasterTabControl.Location.X + EDIT_BOUNDS_PADDING,
                        Location.Y + 20 + titleBar + MasterTabControl.Location.Y + EDIT_BOUNDS_PADDING,
                        MasterTabControl.Width - (EDIT_BOUNDS_PADDING * 2),
                        MasterTabControl.Height - 20 - (EDIT_BOUNDS_PADDING * 2));
                }
            }
        }

        public string StatusText
        {
            set
            {
                editorStatusLabel.Text = value;
            }
        }

        #region Ogmo Event Callbacks

        private void onProjectStart(Project project)
        {
            //Enable menu items
            newProjectToolStripMenuItem.Enabled = false;
            openProjectToolStripMenuItem.Enabled = false;
            closeProjectToolStripMenuItem.Enabled = true;
            editProjectToolStripMenuItem.Enabled = true;

            levelToolStripMenuItem.Enabled = true;
            viewToolStripMenuItem.Enabled = true;
            utilitiesToolStripMenuItem.Enabled = true;
        }

        private void onProjectClose(Project project)
        {
            //Disable menu items
            newProjectToolStripMenuItem.Enabled = true;
            openProjectToolStripMenuItem.Enabled = true;
            closeProjectToolStripMenuItem.Enabled = false;
            editProjectToolStripMenuItem.Enabled = false;          

            levelToolStripMenuItem.Enabled = false;
            viewToolStripMenuItem.Enabled = false;
            utilitiesToolStripMenuItem.Enabled = false;

            //Clear mouse/grid readouts
            MouseCoordinatesLabel.Text = GridCoordinatesLabel.Text = "";
        }

        private void onLevelAdded(int index)
        {
            TabPage t = new TabPage(Ogmo.Levels[index].Name);
            t.ImageIndex = 1;
            LevelEditor e = new LevelEditor(Ogmo.Levels[index]);
            LevelEditors.Add(e);
            t.Controls.Add(e);
            MasterTabControl.TabPages.Add(t);
        }

        private void onLevelClosed(int index)
        {
            MasterTabControl.TabPages.RemoveAt(index);
            LevelEditors[index].OnRemove();
            LevelEditors.RemoveAt(index);

            //Clear zoom/mouse/grid readouts
            if (Ogmo.Levels.Count == 0)
                ZoomLabel.Text = MouseCoordinatesLabel.Text = GridCoordinatesLabel.Text = "";
        }

        private void onLevelChanged(int index)
        {
            SelectedLevelIndex = index;
            
            //Switch to the editor
            if (index != -1)
                LevelEditors[index].SwitchTo();

            //Refresh stuff
            editToolStripMenuItem.Enabled =
                levelPropertiesToolStripMenuItem.Enabled =
                saveLevelToolStripMenuItem.Enabled =
                saveLevelAsToolStripMenuItem.Enabled =
                closeLevelToolStripMenuItem.Enabled =
                duplicateLevelToolStripMenuItem.Enabled =
                closeOtherLevelsToolStripMenuItem.Enabled =
                saveAsImageToolStripMenuItem.Enabled = index != -1;

            saveCameraAsImageToolStripMenuItem.Enabled = index != -1 && Ogmo.Project.CameraEnabled;
        }

        #endregion

        #region Tab Control Events

        private void MasterTabControl_Selecting(object sender, TabControlCancelEventArgs e)
        {
            Ogmo.SetLevel(e.TabPageIndex);
        }

        private void MasterTabControl_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                int old = MasterTabControl.SelectedIndex;
                int num = -1;
                for (int i = 0; i < MasterTabControl.TabCount; i++)
                {
                    if (MasterTabControl.GetTabRect(i).Contains(e.Location))
                        num = i;
                }

                if (num != -1 && MasterTabControl.TabPages[num].Name != "startPage")
                {
                    rightClicked = num;
                    tabPageContextMenuStrip.Show(this, e.Location);
                }
            }
            else
                FocusEditor();
        }

        #endregion

        #region Ogmo Menu Events

        private void preferencesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PreferencesWindow pref = new PreferencesWindow();
            DisableEditing();
            pref.Show(this);
        }

        private void checkForUpdatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ApplicationDeployment.IsNetworkDeployed)
                ApplicationDeployment.CurrentDeployment.CheckForUpdate();
            else
                MessageBox.Show(this, "You cannot check for updates while debugging Ogmo Editor!", "Nope!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutWindow about = new AboutWindow();
            DisableEditing();
            about.Show(this);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        #endregion

        #region Project Menu Events

        private void newProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Ogmo.NewProject();
        }

        private void closeProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Ogmo.CloseProject();
        }

        private void editProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Ogmo.EditProject(Ogmo.ProjectEditMode.NormalEdit);
        }

        private void openProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Ogmo.LoadProject();
        }

        #endregion

        #region Level Menu Events

        private int getTargetLevel()
        {
            if (rightClicked == -1)
                return SelectedLevelIndex;
            else
            {
                int num = rightClicked;
                rightClicked = -1;
                return num;
            }
        }

        private void levelPropertiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Ogmo.CurrentLevel.EditProperties();
        }

        private void newLevelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Ogmo.Project == null)
                return;

            Ogmo.NewLevel();
        }

        private void openLevelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Ogmo.Project == null)
                return;

            Ogmo.OpenLevel();
        }

        private void saveLevelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Ogmo.Project == null)
                return;

            Ogmo.Levels[getTargetLevel()].Save();
        }

        private void saveLevelAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Ogmo.Project == null)
                return;

            Ogmo.Levels[getTargetLevel()].SaveAs();
        }

        private void closeLevelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Ogmo.Project == null)
                return;

            Ogmo.CloseLevel(Ogmo.Levels[getTargetLevel()], true);
        }

        private void duplicateLevelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Ogmo.Project == null)
                return;

            Ogmo.AddLevel(new Level(Ogmo.Levels[getTargetLevel()]));
        }

        private void closeOtherLevelsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Ogmo.CloseOtherLevels(Ogmo.Levels[getTargetLevel()]);
        }

        private void saveAsImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LevelEditors[Ogmo.CurrentLevelIndex].SaveAsImage();
        }

        private void saveCameraAsImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LevelEditors[Ogmo.CurrentLevelIndex].SaveCameraAsImage();
        }

        #endregion

        #region Edit Menu Events

        private void editToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            undoToolStripMenuItem.Enabled = LevelEditors[Ogmo.CurrentLevelIndex].CanUndo;
            redoToolStripMenuItem.Enabled = LevelEditors[Ogmo.CurrentLevelIndex].CanRedo;

            cutToolStripMenuItem.Enabled = copyToolStripMenuItem.Enabled = LevelEditors[Ogmo.CurrentLevelIndex].LayerEditors[Ogmo.LayersWindow.CurrentLayerIndex].CanCopyOrCut;
            if (Ogmo.Clipboard == null)
                pasteToolStripMenuItem.Enabled = false;
            else
                pasteToolStripMenuItem.Enabled = Ogmo.Clipboard.CanPaste(Ogmo.LayersWindow.CurrentLayer);
            selectAllToolStripMenuItem.Enabled = LevelEditors[Ogmo.CurrentLevelIndex].LayerEditors[Ogmo.LayersWindow.CurrentLayerIndex].CanSelectAll;
            deselectToolStripMenuItem.Enabled = LevelEditors[Ogmo.CurrentLevelIndex].LayerEditors[Ogmo.LayersWindow.CurrentLayerIndex].CanDeselect;
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LevelEditors[Ogmo.CurrentLevelIndex].Undo();
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LevelEditors[Ogmo.CurrentLevelIndex].Redo();
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (LevelEditors[Ogmo.CurrentLevelIndex].LayerEditors[Ogmo.LayersWindow.CurrentLayerIndex].CanCopyOrCut)
                LevelEditors[Ogmo.CurrentLevelIndex].LayerEditors[Ogmo.LayersWindow.CurrentLayerIndex].Cut();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (LevelEditors[Ogmo.CurrentLevelIndex].LayerEditors[Ogmo.LayersWindow.CurrentLayerIndex].CanCopyOrCut)
                LevelEditors[Ogmo.CurrentLevelIndex].LayerEditors[Ogmo.LayersWindow.CurrentLayerIndex].Copy();
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Ogmo.Clipboard != null && Ogmo.Clipboard.CanPaste(Ogmo.LayersWindow.CurrentLayer))
                Ogmo.Clipboard.Paste(LevelEditors[Ogmo.CurrentLevelIndex], Ogmo.LayersWindow.CurrentLayer);
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (selectAllToolStripMenuItem.Enabled = LevelEditors[Ogmo.CurrentLevelIndex].LayerEditors[Ogmo.LayersWindow.CurrentLayerIndex].CanSelectAll)
                LevelEditors[Ogmo.CurrentLevelIndex].LayerEditors[Ogmo.LayersWindow.CurrentLayerIndex].SelectAll();
        }

        private void deselectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (LevelEditors[Ogmo.CurrentLevelIndex].LayerEditors[Ogmo.LayersWindow.CurrentLayerIndex].CanDeselect)
                LevelEditors[Ogmo.CurrentLevelIndex].LayerEditors[Ogmo.LayersWindow.CurrentLayerIndex].Deselect();
        }

        #endregion

        #region View Menu Events

        private void viewToolStripMenuItem_DropDownOpened(object sender, EventArgs e)
        {
            editingGridToolStripMenuItem.Checked = EditingGridVisible;

            layersToolStripMenuItem.Enabled = Ogmo.LayersWindow.EditorVisible;
            layersToolStripMenuItem.Checked = Ogmo.LayersWindow.UserVisible;

            toolsToolStripMenuItem.Enabled = Ogmo.ToolsWindow.EditorVisible;
            toolsToolStripMenuItem.Checked = Ogmo.ToolsWindow.UserVisible;

            entitiesToolStripMenuItem.Enabled = Ogmo.EntitiesWindow.EditorVisible;
            entitiesToolStripMenuItem.Checked = Ogmo.EntitiesWindow.UserVisible;

            entitySelectionToolStripMenuItem.Enabled = Ogmo.EntitySelectionWindow.EditorVisible;
            entitySelectionToolStripMenuItem.Checked = Ogmo.EntitySelectionWindow.UserVisible;

            tilePaletteToolStripMenuItem.Enabled = Ogmo.TilePaletteWindow.EditorVisible;
            tilePaletteToolStripMenuItem.Checked = Ogmo.TilePaletteWindow.UserVisible;
        }

        private void editingGridToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EditingGridVisible = !EditingGridVisible;
        }

        private void centerViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LevelEditors[Ogmo.CurrentLevelIndex].LevelView.Center();
        }

        private void zoomOutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LevelEditors[Ogmo.CurrentLevelIndex].LevelView.ZoomOut();
        }

        private void zoomInToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LevelEditors[Ogmo.CurrentLevelIndex].LevelView.ZoomIn();
        }

        private void layersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Ogmo.LayersWindow.EditorVisible)
            {
                Ogmo.LayersWindow.UserVisible = !Ogmo.LayersWindow.UserVisible;
                FocusEditor();
            }
        }

        private void toolsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Ogmo.ToolsWindow.EditorVisible)
            {
                Ogmo.ToolsWindow.UserVisible = !Ogmo.ToolsWindow.UserVisible;
                FocusEditor();
            }
        }

        private void tilePaletteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Ogmo.TilePaletteWindow.EditorVisible)
            {
                Ogmo.TilePaletteWindow.UserVisible = !Ogmo.TilePaletteWindow.UserVisible;
                FocusEditor();
            }
        }

        private void entitiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Ogmo.EntitiesWindow.EditorVisible)
            {
                Ogmo.EntitiesWindow.UserVisible = !Ogmo.EntitiesWindow.UserVisible;
                FocusEditor();
            }
        }

        private void entitySelectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Ogmo.EntitySelectionWindow.EditorVisible)
            {
                Ogmo.EntitySelectionWindow.UserVisible = !Ogmo.EntitySelectionWindow.UserVisible;
                FocusEditor();
            }
        }

        #endregion

        #region Utilities Menu Events

        private void resaveLevelsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form window = null;
            foreach (Form form in OwnedForms)
            {
                if (form is ResaveLevelsWindow)
                {
                    window = form;
                    break;
                }
            }

            if (window == null)
            {
                window = new ResaveLevelsWindow();
                window.Show(this);
            }

            window.Focus();
        }

        private void shiftRenamerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form window = null;
            foreach (Form form in OwnedForms)
            {
                if (form is ShiftRenameLevelsWindow)
                {
                    window = form;
                    break;
                }
            }

            if (window == null)
            {
                window = new ShiftRenameLevelsWindow();
                window.Show(this);
            }

            window.Focus();
        }

        private void swapRenamerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form window = null;
            foreach (Form form in OwnedForms)
            {
                if (form is SwapRenameLevelsWindow)
                {
                    window = form;
                    break;
                }
            }

            if (window == null)
            {
                window = new SwapRenameLevelsWindow();
                window.Show(this);
            }

            window.Focus();
        }

        #endregion

        #region Drag and Drop Events

        private void MainWindow_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        }

        private void MainWindow_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            if (Ogmo.Project == null)
            {
                if (files.Length == 1 && Path.GetExtension(files[0]) == ".oep")
                    Ogmo.LoadProject(files[0]);
            }
            else
            {
                foreach (string file in files)
                    Ogmo.AddLevel(new Level(Ogmo.Project, file));
            }
        }

        #endregion
    }
}
