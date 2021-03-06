﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OgmoEditor.LevelEditors.Actions.TileActions;
using System.Drawing;

namespace OgmoEditor.LevelEditors.Tools.TileTools
{
    public class TileAutoTool : TileTool
    {
        private TileDrawAction drawAction;
        private int[] _data;
        public int widthInTiles;
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
        /// Square block.
        /// </summary>
        public const int SQUARE = 4;

        private int _extraMiddleTiles = 0;


        /// <summary>
        /// Set this flag to use one of the 16-tile binary auto-tile algorithms (OFF, AUTO, or ALT).
        /// </summary>
        public int auto = 1;

        public TileAutoTool()
            : base("Auto Tile.\nRight=AUTO\nLeft=ALTERNATE\nShift+Right=AUTO-Customized\nShift+Left=ALTERNATE-Customized\n\nLeft Click+Alt=add decorations.", "autotile.png")
        {
        }

        public void autoTile(int Index)
        {
            int totalTiles = LayerEditor.Layer.TileCellsX * LayerEditor.Layer.TileCellsY;

            if (_data[Index] <= 0 ) return;
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

            _extraMiddleTiles = LayerEditor.Layer.Tileset.TilesAcross;

            if (_extraMiddleTiles >= 17 && _data[Index] == 16)
            {
                _data[Index] += ((int)(FlxCaveGenerator.random(16, _extraMiddleTiles + 1)) - 16);
            }
        }

        public void squareAutoTile(int Index)
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

            //remap to custom
            if (auto != ALT)
            {
                if (_data[Index] == 1) { _data[Index] = 23; }           // FIX
                else if (_data[Index] == 2) { _data[Index] = 31; }      // |_|
                else if (_data[Index] == 3) { _data[Index] = 22; }       // C
                else if (_data[Index] == 4) { _data[Index] = 25; }      // |_ 25 or 34
                else if (_data[Index] == 5) { _data[Index] = 15; }       // |'|
                else if (_data[Index] == 6) { _data[Index] = 8; }       // FIX
                else if (_data[Index] == 7) { _data[Index] = 2; }         // |^ 2 or 9
                else if (_data[Index] == 8) { _data[Index] = 17; }      // | --
                else if (_data[Index] == 9) { _data[Index] = 24; }       // =]
                else if (_data[Index] == 10) { _data[Index] = 29; }      // _| 29 or 36
                else if (_data[Index] == 11) { _data[Index] = 6; }      // FIX
                else if (_data[Index] == 12) { _data[Index] = 35; }    // _
                else if (_data[Index] == 13) { _data[Index] = 4; }       // ^| 4 or 13
                else if (_data[Index] == 14) { _data[Index] = 21; }     // =|
                else if (_data[Index] == 15) { _data[Index] = 3; }     // ^
                else if (_data[Index] >= 16) { _data[Index] = 19; }      // Empty
            }
            else // is ALT
            {
                if (_data[Index] == 1) { _data[Index] = 4; }            // FIX
                else if (_data[Index] == 2) { _data[Index] = 17; }      //      //BOTTOM LEFT OPEN
                else if (_data[Index] == 3) { _data[Index] = 37; }      //      //TOP LEFT OPEN
                else if (_data[Index] == 4) { _data[Index] = 129; }     // |_
                else if (_data[Index] == 5) { _data[Index] = 36; }      //      //TOP RIGHT OPEN
                else if (_data[Index] == 6) { _data[Index] = 5; }       // FIX
                else if (_data[Index] == 7) { _data[Index] = 9; }       // |^
                else if (_data[Index] == 8) { _data[Index] = 29; }      // |=
                else if (_data[Index] == 9) { _data[Index] = 16; }      //      //BOTTOM RIGHT OPEN
                else if (_data[Index] == 10) { _data[Index] = 135; }    // _|
                else if (_data[Index] == 11) { _data[Index] = 5; }      // FIX
                else if (_data[Index] == 12) { _data[Index] = 130; }    // _
                else if (_data[Index] == 13) { _data[Index] = 15; }     // ^|
                else if (_data[Index] == 14) { _data[Index] = 35; }     // =|
                else if (_data[Index] == 15) { _data[Index] = 10; }     // ^
                else if (_data[Index] >= 16) { _data[Index] = 195; }    // Empty
            }

        }


        public void customAutoTile(int Index)
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

            _extraMiddleTiles = LayerEditor.Layer.Tileset.TilesAcross;

            if (_extraMiddleTiles >= 16 && _data[Index] == 16)
            {
                _data[Index] += ((int)(FlxCaveGenerator.random(16, _extraMiddleTiles + 1)) - 16);
            }

            //remap to custom
            if (auto != ALT){
                if (_data[Index] == 1) { _data[Index] = 4;  }           // FIX
                else if (_data[Index] == 2) { _data[Index] = 4;  }      // FIX
                else if (_data[Index] == 3) { _data[Index] = 5; }       // FIX
                else if (_data[Index] == 4){ _data[Index] = 129; }      // |_
                else if (_data[Index] == 5) { _data[Index] = 4; }       // FIX
                else if (_data[Index] == 6) { _data[Index] = 5; }       // FIX
                else if (_data[Index] == 7){ _data[Index] = 9;}         // |^
                else if (_data[Index] == 8) { _data[Index] = 29; }      // |=
                else if (_data[Index] == 9) { _data[Index] = 4; }       // FIX
                else if (_data[Index] == 10){ _data[Index] = 135;}      // _|
                else if (_data[Index] == 11) { _data[Index] = 5; }      // FIX
                else if (_data[Index] == 12) { _data[Index] = 130; }    // _
                else if (_data[Index] == 13){ _data[Index] = 15;}       // ^|
                else if (_data[Index] == 14) { _data[Index] = 35; }     // =|
                else if (_data[Index] == 15) { _data[Index] = 10; }     // ^
                else if (_data[Index] >= 16){_data[Index] = 195; }      // Empty
            }
            else // is ALT
            {
                if (_data[Index] == 1) { _data[Index] = 4; }            // FIX
                else if (_data[Index] == 2) { _data[Index] = 17; }      //      //BOTTOM LEFT OPEN
                else if (_data[Index] == 3) { _data[Index] = 37; }      //      //TOP LEFT OPEN
                else if (_data[Index] == 4) { _data[Index] = 129; }     // |_
                else if (_data[Index] == 5) { _data[Index] = 36; }      //      //TOP RIGHT OPEN
                else if (_data[Index] == 6) { _data[Index] = 5; }       // FIX
                else if (_data[Index] == 7) { _data[Index] = 9; }       // |^
                else if (_data[Index] == 8) { _data[Index] = 29; }      // |=
                else if (_data[Index] == 9) { _data[Index] = 16; }      //      //BOTTOM RIGHT OPEN
                else if (_data[Index] == 10) { _data[Index] = 135; }    // _|
                else if (_data[Index] == 11) { _data[Index] = 5; }      // FIX
                else if (_data[Index] == 12) { _data[Index] = 130; }    // _
                else if (_data[Index] == 13) { _data[Index] = 15; }     // ^|
                else if (_data[Index] == 14) { _data[Index] = 35; }     // =|
                else if (_data[Index] == 15) { _data[Index] = 10; }     // ^
                else if (_data[Index] >= 16) { _data[Index] = 195; }    // Empty
            }

        }

        public void cave()
        {
            cave(0);
        }

        public void cave(int custom=0)
        {
            int sizeX = LayerEditor.Layer.TileCellsX;
            int sizeY = LayerEditor.Layer.TileCellsY;

            Console.WriteLine("Size X {0} Size Y {1} - crashes? ", sizeX, sizeY);


            FlxCaveGenerator cave = new FlxCaveGenerator(sizeX, sizeY);
            cave.genInitMatrix(sizeX, sizeY);
            int[,] level = cave.generateCaveLevel();

            for (int y = 0; y < sizeY; y++)
            {
                for (int x = 0; x < sizeX; x++)
                {
                    if (LayerEditor.Layer.Tiles[x, y] == -1)
                        level[y, x] = 0;
                    else if (LayerEditor.Layer.Tiles[x, y] <=  LayerEditor.Layer.Tileset.TilesAcross-1)
                        level[y, x] = 1;
                    // HACK : Setting this decoration tile to a negative number so doesn't affect auto-tiling
                    // HACK : Revert to original before setting tile.
                    else
                        level[y, x] = (LayerEditor.Layer.Tiles[x, y] + 1) * -1;
                }
            }

            //level = cave.smoothLevel(level);

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

                // TODO: 
                // This causes a crash in some situations. //

                while (c < widthInTiles)
                    _data[((r - 1) * widthInTiles) + c] = int.Parse(cols[c++]);
            }

            int total = 0;
            int ii = 0;
            int totalTiles = LayerEditor.Layer.TileCellsX * LayerEditor.Layer.TileCellsY;

            while (ii < totalTiles)
            {
                if (custom==1)
                {
                    customAutoTile(ii++);
                }
                else if (custom == 2)
                {
                    squareAutoTile(ii++);
                }
                else
                {
                    autoTile(ii++);
                }
            }

            total = 0;

            for (int y = 0; y < sizeY; y++)
            {
                for (int x = 0; x < sizeX; x++)
                {
                    System.Drawing.Point p = new System.Drawing.Point(x * LayerEditor.Layer.Definition.Grid.Width, y * LayerEditor.Layer.Definition.Grid.Height);

                    SetCaveTile(p, -1);

                    if (_data[total] >= 1)
                    {
                        SetCaveTile(p, _data[total] - 1);
                    }
                    else if (_data[total] == 0)
                    {
                        SetCaveTile(p, -1);
                    }
                    // HACK : Since this is a negative number, it's a decoration tile.
                    // HACK : Reverse the tile number to revert back to decoration.
                    else if (_data[total] < 0)
                    {
                        SetCaveTile(p, (_data[total] + 1)*-1 );
                    }
                    total++;
                }
            }
        }

        public void addDecorations()
        {
            int sizeX = LayerEditor.Layer.TileCellsX;
            int sizeY = LayerEditor.Layer.TileCellsY;

            float chance = Convert.ToSingle(Ogmo.CaveWindow.chanceOfDecoration.Text);

            for (int y = 0; y < sizeY; y++)
            {
                for (int x = 0; x < sizeX; x++)
                {
                    // up

                    if (chance < FlxCaveGenerator.random())
                    {

                        if (LayerEditor.Layer.Tiles[x, y] == 14 && y >= 1)
                        {
                            System.Drawing.Point p = new System.Drawing.Point(x * LayerEditor.Layer.Definition.Grid.Width, (y * LayerEditor.Layer.Definition.Grid.Height) - LayerEditor.Layer.Definition.Grid.Height);
                            SetCaveTile(p, (int)FlxCaveGenerator.random(LayerEditor.Layer.Tileset.TilesAcross, LayerEditor.Layer.Tileset.TilesAcross * 2));
                        }

                        //down
                        if (LayerEditor.Layer.Tiles[x, y] == 11 && y < LayerEditor.Layer.TileCellsY)
                        {
                            System.Drawing.Point p = new System.Drawing.Point(x * LayerEditor.Layer.Definition.Grid.Width, (y * LayerEditor.Layer.Definition.Grid.Height) + LayerEditor.Layer.Definition.Grid.Height);
                            SetCaveTile(p, (int)FlxCaveGenerator.random(LayerEditor.Layer.Tileset.TilesAcross * 2, LayerEditor.Layer.Tileset.TilesAcross * 3));
                        }

                        if ((LayerEditor.Layer.Tiles[x, y] == 7) && x >= 1)
                        {
                            System.Drawing.Point p = new System.Drawing.Point((x * LayerEditor.Layer.Definition.Grid.Width) - LayerEditor.Layer.Definition.Grid.Width, (y * LayerEditor.Layer.Definition.Grid.Height));
                            SetCaveTile(p, (int)FlxCaveGenerator.random(LayerEditor.Layer.Tileset.TilesAcross * 4, LayerEditor.Layer.Tileset.TilesAcross * 5));
                        }


                        if ((LayerEditor.Layer.Tiles[x, y] == 13) && y < LayerEditor.Layer.TileCellsX)
                        {
                            System.Drawing.Point p = new System.Drawing.Point((x * LayerEditor.Layer.Definition.Grid.Width) + LayerEditor.Layer.Definition.Grid.Width, (y * LayerEditor.Layer.Definition.Grid.Height));
                            SetCaveTile(p, (int)FlxCaveGenerator.random(LayerEditor.Layer.Tileset.TilesAcross * 3, LayerEditor.Layer.Tileset.TilesAcross * 4));
                        }
                    }


                    //





                }
            }

            for (int y = 0; y < sizeY; y++)
            {
                for (int x = 0; x < sizeX; x++)
                {
                    if ((LayerEditor.Layer.Tiles[x, y] == 6) && x >= 1)
                    {
                        System.Drawing.Point p = new System.Drawing.Point((x * LayerEditor.Layer.Definition.Grid.Width) - LayerEditor.Layer.Definition.Grid.Width, (y * LayerEditor.Layer.Definition.Grid.Height));
                        SetCaveTile(p, (int)FlxCaveGenerator.random(LayerEditor.Layer.Tileset.TilesAcross * 4, LayerEditor.Layer.Tileset.TilesAcross * 5));
                    }


                    if ((LayerEditor.Layer.Tiles[x, y] == 12) && y < LayerEditor.Layer.TileCellsX)
                    {
                        System.Drawing.Point p = new System.Drawing.Point((x * LayerEditor.Layer.Definition.Grid.Width) + LayerEditor.Layer.Definition.Grid.Width, (y * LayerEditor.Layer.Definition.Grid.Height));
                        SetCaveTile(p, (int)FlxCaveGenerator.random(LayerEditor.Layer.Tileset.TilesAcross * 3, LayerEditor.Layer.Tileset.TilesAcross * 4));
                    }
                }

            }
        }

        public override void OnMouseLeftDown(System.Drawing.Point location)
        {
            if (Util.Shift)
            {
                auto = AUTO;
                cave(1);
            }
            else if (Util.Ctrl)
            {
                auto = SQUARE;
                cave(2);

            }
            else if (Util.Alt)
            {
                Console.WriteLine("Adding decorations");
                addDecorations();

            }
            else
            {
                Console.WriteLine("-AutoTile()-");
                auto = AUTO;
                cave(0);
            }
        }

        public override void OnMouseRightDown(Point location)
        {
            if (Util.Shift)
            {
                auto = ALT;
                cave(1);
            }
            else
            {
                auto = ALT;
                cave(0);
            }
        }

        public override void OnMouseMiddleClick(Point location)
        {
            
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

    }
}
