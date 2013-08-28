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

        //private FlxCaveGenerator cave;

        public GridCaveTool()
            : base("Cave", "cave.png")
        {
            //Console.WriteLine(LayerEditor.Layer.Definition.Grid.Width + "  " + LayerEditor.Layer.Definition.Grid.Height);
            //cave = new FlxCaveGenerator(50,50);
            drawing = false;
        }

        public override void OnMouseLeftDown(System.Drawing.Point location)
        {
            if (!drawing)
            {
                drawing = true;
                drawMode = true;
                //setCell(location, true);

                int sizeX = LayerEditor.Layer.GridCellsX;
                int sizeY = LayerEditor.Layer.GridCellsY;

                Console.WriteLine(sizeX + " " + sizeY);

                FlxCaveGenerator cave = new FlxCaveGenerator(LayerEditor.Layer.GridCellsX, LayerEditor.Layer.GridCellsY);
                int[,] level = cave.generateCaveLevel();

                for (int i = 0; i < sizeX; i++)
                {
                    for (int j = 0; j < sizeY; j++)
                    {
                        System.Drawing.Point p = new System.Drawing.Point(i * LayerEditor.Layer.Definition.Grid.Width, j * LayerEditor.Layer.Definition.Grid.Height);

                        if (level[j, i] == 1)
                        {
                            //Console.WriteLine("In loop" + j + " " + i);
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
                drawMode = false;
                setCell(location, false);
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
            if (drawing && !drawMode)
            {
                drawing = false;
                drawAction = null;
            }
        }

        public override void OnMouseMove(System.Drawing.Point location)
        {
            if (drawing)
                setCell(location, drawMode);
        }

        private void setCell(System.Drawing.Point location, bool setTo)
        {
            location = LayerEditor.Layer.Definition.ConvertToGrid(location);

            if (!IsValidGridCell(location) || LayerEditor.Layer.Grid[location.X, location.Y] == setTo)
                return;

            if (drawAction == null)
                LevelEditor.Perform(drawAction = new GridDrawAction(LayerEditor.Layer, location, setTo));
            else
                drawAction.DoAgain(location);
        }
    }
}
