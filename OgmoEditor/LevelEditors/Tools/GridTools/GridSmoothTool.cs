using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OgmoEditor.LevelEditors.Actions.GridActions;
using System.Diagnostics;

namespace OgmoEditor.LevelEditors.Tools.GridTools
{
    public class GridSmoothTool : GridTool
    {
        private GridDrawAction drawAction;

        public GridSmoothTool()
            : base("Smooth", "smooth.png")
        {
        }

        public override void OnMouseLeftDown(System.Drawing.Point location)
        {
         
            int sizeX = LayerEditor.Layer.GridCellsX;
            int sizeY = LayerEditor.Layer.GridCellsY;

            FlxCaveGenerator cave = new FlxCaveGenerator(LayerEditor.Layer.GridCellsX, LayerEditor.Layer.GridCellsY);
            cave.genInitMatrix(LayerEditor.Layer.GridCellsX, LayerEditor.Layer.GridCellsY);
            int[,] level = cave.generateCaveLevel();

            for (int y = 0; y < sizeY; y++)
            {
                for (int x = 0; x < sizeX; x++)
                {
                    level[y,x] = Convert.ToInt32(LayerEditor.Layer.Grid[x, y]);
                }
            }

            level = cave.smoothLevel(level);

            for (int y = 0; y < sizeY; y++)
            {
                for (int x = 0; x < sizeX; x++)
                {
                    System.Drawing.Point p = new System.Drawing.Point(x * LayerEditor.Layer.Definition.Grid.Width, y * LayerEditor.Layer.Definition.Grid.Height);

                    setCell(p, false);

                    if (level[y,x] == 1)
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

        public override void OnMouseRightDown(System.Drawing.Point location)
        {

        }

        public override void OnMouseLeftUp(System.Drawing.Point location)
        {
        }

        public override void OnMouseRightUp(System.Drawing.Point location)
        {
        }

        public override void OnMouseMove(System.Drawing.Point location)
        {
        }

        private void setCell(System.Drawing.Point location, bool setTo)
        {
            location = LayerEditor.Layer.Definition.ConvertToGrid(location);

            LevelEditor.Perform(drawAction = new GridDrawAction(LayerEditor.Layer, location, setTo));

        }
    }
}
