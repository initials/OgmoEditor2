using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using OgmoEditor.LevelEditors.Tools;
using OgmoEditor.Definitions.LayerDefinitions;
using OgmoEditor.LevelEditors.Tools.GridTools;
using OgmoEditor.LevelEditors.Tools.EntityTools;
using OgmoEditor.LevelEditors.Tools.TileTools;

namespace OgmoEditor.Windows
{
    public class ToolsWindow : OgmoWindow
    {
        public Tool CurrentTool { get; private set; }
        public event Ogmo.ToolCallback OnToolChanged;

        private Dictionary<Type, Tool[]> toolsForLayerTypes;
        private Tool[] tools;

        public ToolsWindow()
            : base(HorizontalSnap.Right, VerticalSnap.Top)
        {
            Name = "ToolsWindow";
            Text = "Tools";
            AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            AutoSize = true;

            CurrentTool = null;

            //Initialize the tool lists
            toolsForLayerTypes = new Dictionary<Type, Tool[]>();
            toolsForLayerTypes.Add(typeof(GridLayerDefinition), new Tool[] { new GridPencilTool(), new GridFloodTool(), new GridRectangleTool(), new GridLineTool(), new GridSelectionTool(), new GridMoveSelectionTool() });
            toolsForLayerTypes.Add(typeof(TileLayerDefinition), new Tool[] { new TilePencilTool(), new TileEyedropperTool(), new TileFloodTool(), new TileRectangleTool(), new TileLineTool(), new TileSelectionTool(), new TileMoveSelectionTool() });
            toolsForLayerTypes.Add(typeof(EntityLayerDefinition), new Tool[] { new EntityPlacementTool(), new EntityEraseTool(), new EntitySelectionTool(), new EntityMoveTool(), new EntityResizeTool(), new EntityAddNodeTool(), new EntityInsertNodeTool() });

            //Events
            Ogmo.LayersWindow.OnLayerChanged += onLayerChanged;
            Ogmo.OnLevelAdded += onLevelAdded;
        }

        public void SetTool(Tool tool)
        {
            if (CurrentTool == tool)
                return;

            //Set it!
            CurrentTool = tool;
            if (tool != null)
                tool.SwitchTo();

            //Call the event
            if (OnToolChanged != null)
                OnToolChanged(tool);

            Ogmo.MainWindow.FocusEditor();
        }

        public void SetTool(Type toolType)
        {
            if (tools != null)
            {
                for (int i = 0; i < tools.Length; i++)
                {
                    if (tools[i].GetType() == toolType)
                    {
                        SetTool(tools[i]);
                        return;
                    }
                }
            }
        }

        public void ClearTool()
        {
            tools = null;
            SetTool((Tool)null);
        }

        protected override void handleKeyDown(KeyEventArgs e)
        {
            if (!e.Control && e.KeyCode >= Keys.D1 && e.KeyCode <= Keys.D9)
            {
                int i = (int)e.KeyCode - (int)Keys.D1;
                if (i < tools.Length)
                    SetTool(tools[i]);
            }
        }

        /*
         *  Events
         */
        private void onLevelAdded(int index)
        {
            EditorVisible = true;
        }

        private void onLayerChanged(LayerDefinition def, int index)
        {
            Controls.Clear();

            if (def != null)
            {
                tools = toolsForLayerTypes[def.GetType()];

                for (int i = 0; i < tools.Length; i++)
                    Controls.Add(new ToolButton(tools[i], (i % 2) * 24, (i / 2) * 24, i));

                if (tools.Length > 0)
                    SetTool(tools[0]); 
                else
                    ClearTool();
            }
        }
    }
}
