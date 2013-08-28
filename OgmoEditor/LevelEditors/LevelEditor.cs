using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OgmoEditor.LevelEditors;
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using OgmoEditor.LevelData;
using OgmoEditor.LevelEditors.LayerEditors;
using OgmoEditor.LevelEditors.Actions;
using OgmoEditor.LevelEditors.Tools;
using System.Drawing.Imaging;

namespace OgmoEditor.LevelEditors
{
    public class LevelEditor : Control
    {
        static private readonly Brush NoFocusBrush = new SolidBrush(Color.FromArgb(80, 220, 220, 220));
        static private readonly Brush ShadowBrush = new SolidBrush(Color.FromArgb(120, 0, 0, 0));
        static private readonly Pen GridBorderPen = new Pen(Color.Black, 2);
        private const float MAX_IMAGE_SIZE = 4096;

        private enum MouseMode { Normal, Pan, Camera };

        private MouseMode mouseMode = MouseMode.Normal;
        private bool mousePanMode;
        private Point lastMousePoint;

        public Level Level { get; private set; }
        public LevelView LevelView { get; private set; }
        public List<LayerEditor> LayerEditors { get; private set; }
        public new Point MousePosition { get; private set; }        

        public LinkedList<OgmoAction> UndoStack { get; private set; }
        public LinkedList<OgmoAction> RedoStack { get; private set; }

        private ActionBatch batch;
        private Brush levelBGBrush;
        private Pen gridPen;

        public LevelEditor(Level level)
            : base()
        {
            Level = level;
            Dock = System.Windows.Forms.DockStyle.Fill;
            SetAutoSizeMode(AutoSizeMode.GrowAndShrink);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            LevelView = new LevelView(this);

            //Create the undo/redo stacks
            UndoStack = new LinkedList<OgmoAction>();
            RedoStack = new LinkedList<OgmoAction>();

            //Create the layer editors
            LayerEditors = new List<LayerEditor>();
            foreach (var l in level.Layers)
                LayerEditors.Add(l.GetEditor(this));

            //Init the level BG brush
            BackgroundImage = DrawUtil.ImgBG;
            levelBGBrush = new SolidBrush(Ogmo.Project.BackgroundColor);
            gridPen = new Pen(Color.FromArgb(130, Ogmo.Project.GridColor));

            //Events
            Application.Idle += Application_Idle;
            this.Paint += Draw;
            this.Resize += onResize;
            this.MouseClick += onMouseClick;
            this.MouseDown += onMouseDown;
            this.MouseUp += onMouseUp;
            this.MouseMove += onMouseMove;
            this.MouseWheel += onMouseWheel;
            this.KeyDown += onKeyDown;
            this.KeyUp += onKeyUp;
        }

        void Application_Idle(object sender, EventArgs e)
        {
            Invalidate();
            LevelView.Update();
        }

        public void OnRemove()
        {
            //Remove external events
            Application.Idle -= Application_Idle;
        }

        #region Rendering

        private void Draw(object sender, PaintEventArgs e)
        {
            e.Graphics.CompositingQuality = CompositingQuality.HighSpeed;
            e.Graphics.SmoothingMode = SmoothingMode.HighSpeed;
            e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;

            //Draw the background logo
            e.Graphics.Transform = LevelView.Identity;
            e.Graphics.DrawImage(DrawUtil.ImgLogo, new Rectangle(Width / 2 - DrawUtil.ImgLogo.Width / 4, Height / 2 - DrawUtil.ImgLogo.Height / 4, DrawUtil.ImgLogo.Width / 2, DrawUtil.ImgLogo.Height / 2));
            if (!Focused)
                e.Graphics.FillRectangle(NoFocusBrush, new Rectangle(0, 0, Width, Height));

            //Draw the level bg
            e.Graphics.Transform = LevelView.Matrix;
            e.Graphics.FillRectangle(ShadowBrush, new Rectangle(10, 10, Level.Size.Width, Level.Size.Height));
            e.Graphics.FillRectangle(levelBGBrush, new Rectangle(0, 0, Level.Size.Width, Level.Size.Height));

            //Layers below the current one
            int i;
            for (i = 0; i < Ogmo.LayersWindow.CurrentLayerIndex; i++)
            {
                if (Ogmo.Project.LayerDefinitions[i].Visible)
                    LayerEditors[i].DrawHandler(e.Graphics, false, true);
            }

            //Current layer
            LayerEditors[Ogmo.LayersWindow.CurrentLayerIndex].DrawHandler(e.Graphics, true, true);

            //Layers above the current one
            for (; i < LayerEditors.Count; i++)
            {
                if (i < Ogmo.Project.LayerDefinitions.Count && Ogmo.Project.LayerDefinitions[i].Visible)
                    LayerEditors[i].DrawHandler(e.Graphics, false, false);
            }

            //Draw the grid
            if (Ogmo.MainWindow.EditingGridVisible)
            {
                e.Graphics.Transform = LevelView.Identity;

                PointF inc = new PointF(Ogmo.LayersWindow.CurrentLayer.Definition.Grid.Width * LevelView.Zoom, Ogmo.LayersWindow.CurrentLayer.Definition.Grid.Height * LevelView.Zoom);
                while (inc.X <= 5)
                    inc.X *= 2;
                while (inc.Y <= 5)
                    inc.Y *= 2;

                float width = Ogmo.CurrentLevel.Size.Width * LevelView.Zoom;
                float height = Ogmo.CurrentLevel.Size.Height * LevelView.Zoom;

                PointF offset = LevelView.EditorToScreen(LayerEditors[Ogmo.LayersWindow.CurrentLayerIndex].DrawOffset);

                for (float xx = inc.X; xx < width; xx += inc.X)
                    e.Graphics.DrawLine(gridPen, offset.X + xx, offset.Y, offset.X + xx, offset.Y + height);

                for (float yy = inc.Y; yy < height; yy += inc.Y)
                    e.Graphics.DrawLine(gridPen, offset.X, offset.Y + yy, offset.X + width, offset.Y + yy);

                e.Graphics.DrawRectangle(GridBorderPen, new Rectangle((int)offset.X, (int)offset.Y, (int)width + 1, (int)height));
            }

            //Draw the camera
            if (Ogmo.Project.CameraEnabled)
            {
                e.Graphics.Transform = LevelView.Matrix;
                e.Graphics.DrawRectangle(DrawUtil.CameraRectPen, new Rectangle(Level.CameraPosition.X, Level.CameraPosition.Y, Ogmo.Project.CameraSize.Width, Ogmo.Project.CameraSize.Height));
            }
        }

        public void SaveAsImage()
        {
            SaveLevelToImage(Level.Bounds);
        }

        public void SaveCameraAsImage()
        {
            SaveLevelToImage(new Rectangle(Level.CameraPosition.X, Level.CameraPosition.Y, Ogmo.Project.CameraSize.Width, Ogmo.Project.CameraSize.Height));
        }

        private void SaveLevelToImage(Rectangle area)
        {
            //Get the path!
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Title = "Save Level as Image...";
            dialog.Filter = "PNG Image File|*.png";

            string file = Level.SaveName;
            int num = file.LastIndexOf(".oel");
            if (num != -1)
                file = file.Remove(num);
            file = file + ".png";
            dialog.FileName = file;
            dialog.InitialDirectory = Ogmo.Project.SavedDirectory;
            DialogResult result = dialog.ShowDialog();
            if (result == DialogResult.Cancel)
                return;

            //Actual rendering
            Bitmap bmp;
            {
                //Init the size vars
                float scale = Math.Min(Math.Min(MAX_IMAGE_SIZE / area.Width, 1), Math.Min(MAX_IMAGE_SIZE / area.Height, 1));
                int width = (int)(scale * area.Width);
                int height = (int)(scale * area.Height);

                //Init the bitmap
                bmp = new Bitmap(width, height);
                Graphics g = Graphics.FromImage(bmp);
                g.SmoothingMode = SmoothingMode.HighSpeed;
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                g.TranslateTransform(-area.X, -area.Y);
                g.ScaleTransform(scale, scale);

                //Draw the layers!
                for (int i = 0; i < LayerEditors.Count; i++)
                    if (Ogmo.Project.LayerDefinitions[i].Visible)
                        LayerEditors[i].DrawHandler(g, false, true);
            }

            //Save it then dispose it
            Stream stream = dialog.OpenFile();
            bmp.Save(stream, ImageFormat.Png);
            stream.Close();
            bmp.Dispose();
        }

        #endregion

        #region Actions

        public void Perform(OgmoAction action)
        {
            if (action == null)
                return;

            //If a batch is in progress, stop it!
            EndBatch();

            //If you're over the undo limit, chop off an action
            while (UndoStack.Count >= Properties.Settings.Default.UndoLimit)
                UndoStack.RemoveFirst();

            //If the level is so-far unchanged, change it and store that fact
            if (!Level.Changed)
            {
                action.LevelWasChanged = false;
                Level.Changed = true;
            }

            //Add the action to the undo stack and then do it!
            UndoStack.AddLast(action);  
            action.Do();

            //Clear the redo stack
            RedoStack.Clear();
        }

        public void StartBatch()
        {
            batch = new ActionBatch();

            if (!Level.Changed)
            {
                batch.LevelWasChanged = false;
                Level.Changed = true;
            }

            while (UndoStack.Count >= Properties.Settings.Default.UndoLimit)
                UndoStack.RemoveFirst();
            UndoStack.AddLast(batch);
            RedoStack.Clear();
        }

        public void BatchPerform(OgmoAction action)
        {
            if (action == null)
                return;

            batch.Add(action);
            action.Do();
        }

        public void EndBatch()
        {
            batch = null;
        }

        public void Undo()
        {
            if (UndoStack.Count > 0)
            {
                //Remove it
                OgmoAction action = UndoStack.Last.Value;
                UndoStack.RemoveLast();

                //Undo it
                action.Undo();

                //Roll back level changed flag
                Level.Changed = action.LevelWasChanged;

                //Add it to the redo stack
                RedoStack.AddLast(action);
            }
        }

        public void Redo()
        {
            if (RedoStack.Count > 0)
            {
                //Remove it
                OgmoAction action = RedoStack.Last.Value;
                RedoStack.RemoveLast();

                //Redo it
                action.Do();

                //Mark level as changed
                Level.Changed = true;

                //Add it to the undo stack
                UndoStack.AddLast(action);
            }
        }

        public bool CanUndo
        {
            get { return UndoStack.Count > 0; }
        }

        public bool CanRedo
        {
            get { return RedoStack.Count > 0; }
        }

        #endregion

        #region Events

        public void SwitchTo()
        {
            Focus();
            Ogmo.MainWindow.ZoomLabel.Text = LevelView.ZoomString;
        }

        private void onResize(object sender, EventArgs e)
        {
            LevelView.OnParentResized();
        }

        private void onKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Home)
            {
                LevelView.Center();
            }
            else if (e.KeyCode == Keys.Oemplus)
            {
                LevelView.ZoomIn(LevelView.EditorToScreen(MousePosition));
            }
            else if (e.KeyCode == Keys.OemMinus)
            {
                LevelView.ZoomOut(LevelView.EditorToScreen(MousePosition));
            }
            else if (mouseMode == MouseMode.Normal && e.KeyCode == Keys.Space)
            {
                mouseMode = MouseMode.Pan;
            }
            else if (mouseMode == MouseMode.Normal && e.KeyCode == Keys.C)
            {
                mouseMode = MouseMode.Camera;
            }
            else
            {
                //Call the layer event
                LayerEditors[Ogmo.LayersWindow.CurrentLayerIndex].OnKeyDown(e.KeyCode);
            }
        }

        private void onKeyUp(object sender, KeyEventArgs e)
        {
            if (mouseMode == MouseMode.Pan && e.KeyCode == Keys.Space)
            {
                mouseMode = MouseMode.Normal;
                mousePanMode = false;
            }
            else if (mouseMode == MouseMode.Camera && e.KeyCode == Keys.C)
            {
                mouseMode = MouseMode.Normal;
                mousePanMode = false;
                if (!Util.Ctrl)
                    SnapCamera();
            }
            else
            {
                //Call the layer event
                LayerEditors[Ogmo.LayersWindow.CurrentLayerIndex].OnKeyUp(e.KeyCode);
            }
        }

        private void onMouseClick(object sender, MouseEventArgs e)
        {
            Focus();
            if (mouseMode != MouseMode.Normal)
                return;

            //Call the layer event
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                LayerEditors[Ogmo.LayersWindow.CurrentLayerIndex].OnMouseLeftClick(LevelView.ScreenToEditor(e.Location));
            else if (e.Button == System.Windows.Forms.MouseButtons.Right)
                LayerEditors[Ogmo.LayersWindow.CurrentLayerIndex].OnMouseRightClick(LevelView.ScreenToEditor(e.Location));
        }

        private void onMouseDown(object sender, MouseEventArgs e)
        {
            if (mouseMode != MouseMode.Normal)
            {
                //Enter mouse move mode
                mousePanMode = true;
                lastMousePoint = e.Location;

                if (mouseMode == MouseMode.Camera)
                {
                    //Update the camera position
                    Level.CameraPosition = LevelView.ScreenToEditor(e.Location);
                    foreach (var ed in LayerEditors)
                        ed.UpdateDrawOffset(Level.CameraPosition);
                }
            }
            else
            {
                //Call the layer event
                if (e.Button == System.Windows.Forms.MouseButtons.Left)
                    LayerEditors[Ogmo.LayersWindow.CurrentLayerIndex].OnMouseLeftDown(LevelView.ScreenToEditor(e.Location));
                else if (e.Button == System.Windows.Forms.MouseButtons.Right)
                    LayerEditors[Ogmo.LayersWindow.CurrentLayerIndex].OnMouseRightDown(LevelView.ScreenToEditor(e.Location));
                else if (e.Button == System.Windows.Forms.MouseButtons.Middle)
                {
                    //Enter mouse move mode
                    mousePanMode = true;
                    lastMousePoint = e.Location;
                }
            }
        }

        private void onMouseUp(object sender, MouseEventArgs e)
        {
            if (mouseMode != MouseMode.Normal)
            {
                if (mouseMode == MouseMode.Camera && !Util.Ctrl)
                    SnapCamera();

                //Exit mouse move mode
                mousePanMode = false;
            }
            else
            {
                //Call the layer event
                if (e.Button == System.Windows.Forms.MouseButtons.Left)
                    LayerEditors[Ogmo.LayersWindow.CurrentLayerIndex].OnMouseLeftUp(LevelView.ScreenToEditor(e.Location));
                else if (e.Button == System.Windows.Forms.MouseButtons.Right)
                    LayerEditors[Ogmo.LayersWindow.CurrentLayerIndex].OnMouseRightUp(LevelView.ScreenToEditor(e.Location));
                else if (e.Button == System.Windows.Forms.MouseButtons.Middle)
                {
                    //Exit mouse move mode
                    mousePanMode = false;
                }
            }
        }

        private void onMouseMove(object sender, MouseEventArgs e)
        {
            //Pan the camera if in move mode
            if (mousePanMode)
            {
                if (mouseMode == MouseMode.Camera)
                {
                    //Update the camera position
                    Level.CameraPosition = LevelView.ScreenToEditor(e.Location);
                    foreach (var ed in LayerEditors)
                        ed.UpdateDrawOffset(Level.CameraPosition);
                }
                else
                {
                    LevelView.Pan(new Point(e.Location.X - lastMousePoint.X, e.Location.Y - lastMousePoint.Y));
                    lastMousePoint = e.Location;
                }
            }

            //Update the mouse coord display
            MousePosition = LevelView.ScreenToEditor(e.Location);
            Point mouseDraw = Ogmo.Project.LayerDefinitions[Ogmo.LayersWindow.CurrentLayerIndex].SnapToGrid(MousePosition);
            Point gridPos = Ogmo.Project.LayerDefinitions[Ogmo.LayersWindow.CurrentLayerIndex].ConvertToGrid(MousePosition);
            Ogmo.MainWindow.MouseCoordinatesLabel.Text = "Mouse: ( " + mouseDraw.X.ToString() + ", " + mouseDraw.Y.ToString() + " )";
            Ogmo.MainWindow.GridCoordinatesLabel.Text = "Grid: ( " + gridPos.X.ToString() + ", " + gridPos.Y.ToString() + " )";

            //Call the layer event
            LayerEditors[Ogmo.LayersWindow.CurrentLayerIndex].OnMouseMove(MousePosition);
        }

        private void onMouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0)
                LevelView.ZoomIn(e.Location);
            else
                LevelView.ZoomOut(e.Location);
        }

        #endregion

        #region Utilities

        private void SnapCamera()
        {
            Level.CameraPosition = Ogmo.LayersWindow.CurrentLayer.Definition.SnapToGrid(Level.CameraPosition);
            foreach (var ed in LayerEditors)
                ed.UpdateDrawOffset(Level.CameraPosition);
        }

        #endregion
    }
}
