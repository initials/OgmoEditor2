using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OgmoEditor.LevelEditors.Actions.TileActions;
using System.Drawing;

namespace OgmoEditor.LevelEditors.Tools.TileTools
{
    public class TileCaveTool : TileTool
    {
        /// <summary>
        /// No auto-tiling.
        /// </summary>
        public const int OFF = 0;

        /// <summary>
        /// Platformer-friendly auto-tiling.
        /// </summary>
        public const int AUTO = 1;

        /// <summary>
        /// Top-down auto-tiling.
        /// </summary>
        public const int ALT = 2;


        /// <summary>
        /// Random pick from tilesheet
        /// </summary>
        public const int RANDOM = 3;

        /// <summary>
        /// Set this flag to use one of the 16-tile binary auto-tile algorithms (OFF, AUTO, or ALT).
        /// </summary>
        public int auto = 1;

        private Point drawStart;
        private TileDrawAction drawAction;
        private int[] _data;
        public int widthInTiles;

        //private float initialRandomGen = 0.5f;

        public TileCaveTool()
            : base("Cave\nLeft click for AUTO\nRight click for ALT", "cave.png")
        {
        }

       
        public void autoTile(int Index)
        {
            int totalTiles = LayerEditor.Layer.TileCellsX * LayerEditor.Layer.TileCellsY;

            if (_data[Index] == 0) return;
            _data[Index] = 0;
            if ((Index - widthInTiles < 0) || (_data[Index - widthInTiles] > 0)) 		//UP
                _data[Index] += 1;
            if ((Index % widthInTiles >= widthInTiles - 1) || (_data[Index + 1] > 0)) 		//RIGHT
                _data[Index] += 2;
            if ((Index + widthInTiles >= totalTiles) || (_data[Index + widthInTiles] > 0)) //DOWN
                _data[Index] += 4;
            if ((Index % widthInTiles <= 0) || (_data[Index - 1] > 0)) 					//LEFT
                _data[Index] += 8;

            if ((auto == ALT) && (_data[Index] == 15))	//The alternate algo checks for interior corners
            {
                if ((Index % widthInTiles > 0) && (Index + widthInTiles < totalTiles) && (_data[Index + widthInTiles - 1] <= 0))
                    _data[Index] = 1;		//BOTTOM LEFT OPEN
                if ((Index % widthInTiles > 0) && (Index - widthInTiles >= 0) && (_data[Index - widthInTiles - 1] <= 0))
                    _data[Index] = 2;		//TOP LEFT OPEN
                if ((Index % widthInTiles < widthInTiles - 1) && (Index - widthInTiles >= 0) && (_data[Index - widthInTiles + 1] <= 0))
                    _data[Index] = 4;		//TOP RIGHT OPEN
                if ((Index % widthInTiles < widthInTiles - 1) && (Index + widthInTiles < totalTiles) && (_data[Index + widthInTiles + 1] <= 0))
                    _data[Index] = 8; 		//BOTTOM RIGHT OPEN
            }

            _data[Index] += 1;


            //extra part for if you have extra tiles.

            int _extraMiddleTiles = LayerEditor.Layer.Tileset.TilesAcross;

            if (_extraMiddleTiles >= 16 && _data[Index] == 16)
            {
                _data[Index] += ( (int)(FlxCaveGenerator.random(16, _extraMiddleTiles + 1)) - 16);
            }
        }

        public void cave()
        {
            int sizeX = LayerEditor.Layer.TileCellsX;
            int sizeY = LayerEditor.Layer.TileCellsY;

            float initWall = Convert.ToSingle( Ogmo.CaveWindow.initWalls.Text);

            //if (Util.One) initWall -= 0.1f;

            FlxCaveGenerator cave = new FlxCaveGenerator(LayerEditor.Layer.TileCellsX, LayerEditor.Layer.TileCellsY, initWall);
            cave.genInitMatrix(LayerEditor.Layer.TileCellsX, LayerEditor.Layer.TileCellsY);

            int[,] level = cave.generateCaveLevel();

            string MapData = cave.convertMultiArrayToString(level);

            int heightInTiles = 0;

            //Figure out the map dimensions based on the data string
            string[] cols;
            string[] rows = MapData.Split('\n');
            heightInTiles = rows.Length;
            int r = 0;
            int c;

            cols = rows[r].Split(',');
            _data = new int[rows.Length * cols.Length];
            while (r < heightInTiles)
            {
                cols = rows[r++].Split(',');
                if (cols.Length <= 1)
                {
                    heightInTiles = heightInTiles - 1;
                    continue;
                }
                if (widthInTiles == 0)
                    widthInTiles = cols.Length;
                c = 0;
                while (c < widthInTiles)
                    _data[((r - 1) * widthInTiles) + c] = int.Parse(cols[c++]);
            }

            int total = 0;
            int ii = 0;
            int totalTiles = LayerEditor.Layer.TileCellsX * LayerEditor.Layer.TileCellsY;

            while (ii < totalTiles)
                autoTile(ii++);

            total = 0;
            for (int y = 0; y < sizeY; y++)
            {
                for (int x = 0; x < sizeX; x++)
                {
                    System.Drawing.Point p = new System.Drawing.Point(x * LayerEditor.Layer.Definition.Grid.Width, y * LayerEditor.Layer.Definition.Grid.Height);

                    SetCaveTile(p, -1);

                    if (_data[total] >= 1)
                    {
                        if (auto == OFF) SetCaveTile(p, 35 - 1);
                        else SetCaveTile(p, _data[total] - 1);
                        
                    }
                    else
                    {
                        SetCaveTile(p, -1);
                    }
                    total++;
                }
            }
        }

        public override void OnMouseLeftDown(System.Drawing.Point location)
        {
            if (Util.Ctrl) auto = OFF;
            else auto = AUTO;
            cave();
        }

        public override void OnMouseRightDown(Point location)
        {
            if (Util.Ctrl) auto = OFF;
            else auto = ALT;
            cave();
        }

        public override void OnMouseLeftUp(Point location)
        {
        }

        public override void OnMouseRightUp(Point location)
        {
        }

        public override void OnMouseMove(Point location)
        {
        }

        private void SetCaveTile(System.Drawing.Point location, int setTo)
        {
            location = LayerEditor.Layer.Definition.ConvertToGrid(location);

            LevelEditor.Perform(drawAction = new TileDrawAction(LayerEditor.Layer, location, setTo));
        }

        private void SetTiles(Point location, Rectangle? setTo, bool start = false)
        {
            location = LayerEditor.Layer.Definition.ConvertToGrid(location);
            if (!IsValidTileCell(location))
                return;

            if (!setTo.HasValue)
            {
                if (LayerEditor.Layer[location.X, location.Y] != -1)
                {
                    if (drawAction == null)
                        LevelEditor.Perform(drawAction = new TileDrawAction(LayerEditor.Layer, location, -1));
                    else
                        drawAction.DoAgain(location, -1);
                }
            }
            else if (setTo.Value.Area() == 1)
            {
                int id = LayerEditor.Layer.Tileset.GetIDFromCell(setTo.Value.Location);
                if (LayerEditor.Layer[location.X, location.Y] != id)
                {
                    if (drawAction == null)
                        LevelEditor.Perform(drawAction = new TileDrawAction(LayerEditor.Layer, location, id));
                    else
                        drawAction.DoAgain(location, id);
                }
            }
            else
            {
                if (start)
                    drawStart = location;

                //Draw the tiles
                for (int x = 0; x < setTo.Value.Width && location.X + x < LayerEditor.Layer.TileCellsX; x++)
                {
                    for (int y = 0; y < setTo.Value.Height && location.Y + y < LayerEditor.Layer.TileCellsY; y++)
                    {
                        int id = LayerEditor.Layer.Tileset.GetIDFromSelectionRectPoint(setTo.Value, drawStart, new Point(location.X + x, location.Y + y));

                        if (LayerEditor.Layer[location.X + x, location.Y + y] != id)
                        {
                            if (drawAction == null)
                                LevelEditor.Perform(drawAction = new TileDrawAction(LayerEditor.Layer, location, id));
                            else
                                drawAction.DoAgain(new Point(location.X + x, location.Y + y), id);
                        }
                    }
                }
            }
        }
    }
}
