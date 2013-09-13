using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OgmoEditor.LevelEditors.Actions.GridActions;
using System.Diagnostics;

namespace OgmoEditor.LevelEditors.Tools.GridTools
{
    public class GridCaveTool : GridTool
    {
        private bool drawing;
        private bool drawMode;
        private GridDrawAction drawAction;

        public GridCaveTool()
            : base("Cave\nLeft click for regular\nRight click for inverse.", "cave.png")
        {
            drawing = false;
        }

        /// <summary>
        /// Creates a cave style level.
        /// </summary>
        /// <param name="location"></param>
        public override void OnMouseLeftDown(System.Drawing.Point location)
        {
            if (!drawing)
            {
                drawing = true;
                drawMode = true;

                int sizeX = LayerEditor.Layer.GridCellsX;
                int sizeY = LayerEditor.Layer.GridCellsY;

                FlxCaveGenerator cave = new FlxCaveGenerator(LayerEditor.Layer.GridCellsX, LayerEditor.Layer.GridCellsY);
                cave.genInitMatrix(LayerEditor.Layer.GridCellsX, LayerEditor.Layer.GridCellsY);
                
                int[,] level = cave.generateCaveLevel();

                for (int i = 0; i < sizeX; i++)
                {
                    for (int j = 0; j < sizeY; j++)
                    {
                        System.Drawing.Point p = new System.Drawing.Point(i * LayerEditor.Layer.Definition.Grid.Width, j * LayerEditor.Layer.Definition.Grid.Height);
                        
                        setCell(p, false);
                        
                        if (level[j, i] == 1)
                        {
                            setCell(p, true);
                        }
                        else
                        {
                            setCell(p, false);
                        }
                    }
                }
            }
        }

        public override void OnMouseRightDown(System.Drawing.Point location)
        {
            if (!drawing)
            {
                drawing = true;
                drawMode = true;

                int sizeX = LayerEditor.Layer.GridCellsX;
                int sizeY = LayerEditor.Layer.GridCellsY;

                FlxCaveGenerator cave = new FlxCaveGenerator(LayerEditor.Layer.GridCellsX, LayerEditor.Layer.GridCellsY);
                cave.genInitMatrix(LayerEditor.Layer.GridCellsX, LayerEditor.Layer.GridCellsY);

                int[,] level = cave.generateCaveLevel();

                for (int i = 0; i < sizeX; i++)
                {
                    for (int j = 0; j < sizeY; j++)
                    {
                        System.Drawing.Point p = new System.Drawing.Point(i * LayerEditor.Layer.Definition.Grid.Width, j * LayerEditor.Layer.Definition.Grid.Height);
                       
                        setCell(p, true);

                        if (level[j, i] == 1)
                        {
                            setCell(p, false);
                        }
                        else
                        {
                            setCell(p, true);
                        }
                    }
                }
            }
        }

        public override void OnMouseLeftUp(System.Drawing.Point location)
        {
            if (drawing && drawMode)
            {
                drawing = false;
                drawAction = null;
            }
        }

        public override void OnMouseRightUp(System.Drawing.Point location)
        {
            if (drawing && drawMode)
            {
                drawing = false;
                drawAction = null;
            }
        }

        public override void OnMouseMove(System.Drawing.Point location)
        {
        //    if (drawing)
        //        setCell(location, drawMode);
        }

        private void setCell(System.Drawing.Point location, bool setTo)
        {
            location = LayerEditor.Layer.Definition.ConvertToGrid(location);

            LevelEditor.Perform(drawAction = new GridDrawAction(LayerEditor.Layer, location, setTo));

            //location = LayerEditor.Layer.Definition.ConvertToGrid(location);

            //if (!IsValidGridCell(location) || LayerEditor.Layer.Grid[location.X, location.Y] == setTo)
            //    return;

            //if (drawAction == null)
            //    LevelEditor.Perform(drawAction = new GridDrawAction(LayerEditor.Layer, location, setTo));
            //else
            //    drawAction.DoAgain(location);
        }
    }
}
