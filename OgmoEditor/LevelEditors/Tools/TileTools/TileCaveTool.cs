using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OgmoEditor.LevelEditors.Actions.TileActions;
using System.Drawing;

namespace OgmoEditor.LevelEditors.Tools.TileTools
{
    public class TileCaveTool : TileTool
    {
        private bool drawing;
        private bool drawMode;
        private Point drawStart;
        private TileDrawAction drawAction;
        private int[] _data;
        public int widthInTiles;

        public TileCaveTool()
            : base("Cave", "cave.png")
        {
            drawing = false;
        }

       
        protected void autoTile(int Index)
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

            //if ((auto == ALT) && (_data[Index] == 15))	//The alternate algo checks for interior corners
            //{
            //    if ((Index % widthInTiles > 0) && (Index + widthInTiles < totalTiles) && (_data[Index + widthInTiles - 1] <= 0))
            //        _data[Index] = 1;		//BOTTOM LEFT OPEN
            //    if ((Index % widthInTiles > 0) && (Index - widthInTiles >= 0) && (_data[Index - widthInTiles - 1] <= 0))
            //        _data[Index] = 2;		//TOP LEFT OPEN
            //    if ((Index % widthInTiles < widthInTiles - 1) && (Index - widthInTiles >= 0) && (_data[Index - widthInTiles + 1] <= 0))
            //        _data[Index] = 4;		//TOP RIGHT OPEN
            //    if ((Index % widthInTiles < widthInTiles - 1) && (Index + widthInTiles < totalTiles) && (_data[Index + widthInTiles + 1] <= 0))
            //        _data[Index] = 8; 		//BOTTOM RIGHT OPEN
            //}

            _data[Index] += 1;

            //if (_extraMiddleTiles >= 1 && _data[Index] == 16)
            //{
            //    _data[Index] += (int)(FlxU.random() * (_extraMiddleTiles + 1));
            //}
        }
        /*
        public void createDataSet(int[,] MapData)
        {
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
                    _data[((r - 1) * widthInTiles) + c] = int.Parse(cols[c++]); //.push(uint(cols[c++]));
            }

            //now that height and width have been determined, find how many extra 
            //"filler tiles" are at the end of your map.

            int standardLength = 16 * TileWidth;
            int graphicWidth = _tileBitmap.Width;
            _extraMiddleTiles = (graphicWidth - standardLength) / TileWidth;

            //Pre-process the map data if it's auto-tiled
            int i;
            totalTiles = widthInTiles * heightInTiles;
            if (auto > OFF)
            {
                collideIndex = startingIndex = drawIndex = 1;
                i = 0;
                while (i < totalTiles)
                    autoTile(i++);
            }
            if (auto == RANDOM)
            {
                collideIndex = startingIndex = drawIndex = 1;
                i = 0;
                while (i < totalTiles)
                    randomTile(i++);
            }
        }
        */


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

                int sizeX = LayerEditor.Layer.TileCellsX;
                int sizeY = LayerEditor.Layer.TileCellsY;

                FlxCaveGenerator cave = new FlxCaveGenerator(LayerEditor.Layer.TileCellsX, LayerEditor.Layer.TileCellsY);
                cave.genInitMatrix(LayerEditor.Layer.TileCellsX, LayerEditor.Layer.TileCellsY);

                int[,] level = cave.generateCaveLevel();

                string MapData = cave.convertMultiArrayToString(level);

                Console.WriteLine(MapData.ToString());

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
                        _data[((r - 1) * widthInTiles) + c] = int.Parse(cols[c++]); //.push(uint(cols[c++]));
                }

                int total = 0;
                int ii = 0;
                int totalTiles = LayerEditor.Layer.TileCellsX * LayerEditor.Layer.TileCellsY;

                while (ii < totalTiles)
                    autoTile(ii++);

                total = 0;
                for (int i = 0; i < sizeY; i++)
                {
                    for (int j = 0; j < sizeX; j++)
                    {
                        System.Drawing.Point p = new System.Drawing.Point(j * LayerEditor.Layer.Definition.Grid.Width, i * LayerEditor.Layer.Definition.Grid.Height);

                        SetCaveTile(p,  -1);

                        if (_data[total] >= 1)
                        {
                            SetCaveTile(p,  _data[total] - 1  );
                        }
                        else
                        {
                            SetCaveTile(p,  -1);
                        }
                        total++;
                    }
                }
            }
        }

        public override void OnMouseRightDown(Point location)
        {
            if (!drawing)
            {
                drawing = true;
                drawMode = false;

                SetTiles(location, null);
            }
        }

        public override void OnMouseLeftUp(Point location)
        {
            if (drawing && drawMode)
            {
                drawing = false;
                drawAction = null;
            }
        }

        public override void OnMouseRightUp(Point location)
        {
            if (drawing && !drawMode)
            {
                drawing = false;
                drawAction = null;
            }
        }

        public override void OnMouseMove(Point location)
        {
            if (drawing)
                SetTiles(location, drawMode ? Ogmo.TilePaletteWindow.Tiles : null);
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
