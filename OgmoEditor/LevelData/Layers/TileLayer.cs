using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OgmoEditor.Definitions.LayerDefinitions;
using System.Xml;
using OgmoEditor.LevelEditors.LayerEditors;
using OgmoEditor.LevelEditors.Resizers;
using System.Drawing;
using OgmoEditor.Definitions;
using System.Diagnostics;
using System.Drawing.Drawing2D;

namespace OgmoEditor.LevelData.Layers
{
    public class TileLayer : Layer
    {
        public new TileLayerDefinition Definition { get; private set; }       
        public TileSelection Selection;
        public Tileset Tileset;

        private int[,] tiles;
        private Bitmap canvas;
        private Graphics canvasGraphics;

        public TileLayer(Level level, TileLayerDefinition definition)
            : base(level, definition)
        {
            Definition = definition;
            Tileset = Ogmo.Project.Tilesets[0];

            int tileWidth = Level.Size.Width / definition.Grid.Width + (Level.Size.Width % definition.Grid.Width != 0 ? 1 : 0);
            int tileHeight = Level.Size.Height / definition.Grid.Height + (Level.Size.Height % definition.Grid.Height != 0 ? 1 : 0);
            tiles = new int[tileWidth, tileHeight];
            Clear();

            BuildTilesEmpty();
        }

        public override XmlElement GetXML(XmlDocument doc)
        {
            XmlElement xml = doc.CreateElement(Definition.Name);

            //Save which tileset is being used for this layer
            XmlAttribute tileset = doc.CreateAttribute("tileset");
            tileset.InnerText = Tileset.Name;
            xml.Attributes.Append(tileset);

            //Save the export mode
            XmlAttribute export = doc.CreateAttribute("exportMode");
            export.InnerText = Definition.ExportMode.ToString();
            xml.Attributes.Append(export);

            if (Definition.ExportMode == TileLayerDefinition.TileExportMode.CSV || Definition.ExportMode == TileLayerDefinition.TileExportMode.TrimmedCSV)
            {
                //Convert all tile values to CSV
                string[] rows = new string[TileCellsY];
                for (int i = 0; i < TileCellsY; i++)
                {
                    string[] tileStrs = new string[TileCellsX];
                    for (int j = 0; j < tileStrs.GetLength(0); j++)
                    {
                        tileStrs[j] = tiles[j, i].ToString();
                    }
                    rows[i] = string.Join(",", tileStrs);
                }

                //Trim off trailing empties
                if (Definition.ExportMode == TileLayerDefinition.TileExportMode.TrimmedCSV)
                {
                    for (int i = 0; i < rows.Length; i++)
                    {
                        int index = rows[i].LastIndexOf(",-1");
                        while (index != -1 && index == rows[i].Length - 3)
                        {
                            rows[i] = rows[i].Substring(0, rows[i].Length - 3);
                            index = rows[i].LastIndexOf(",-1");
                        }
                        if (rows[i] == "-1")
                            rows[i] = "";
                    }
                }

                //Throw it in the xml text
                xml.InnerText = string.Join("\n", rows);
            }
            else if (Definition.ExportMode == TileLayerDefinition.TileExportMode.XML || Definition.ExportMode == TileLayerDefinition.TileExportMode.XMLCoords)
            {
                //XML Export
                XmlElement tile;
                XmlAttribute a;
                for (int i = 0; i < tiles.GetLength(0); i++)
                {
                    for (int j = 0; j < tiles.GetLength(1); j++)
                    {
                        if (tiles[i, j] != -1)
                        {
                            tile = doc.CreateElement("tile");

                            a = doc.CreateAttribute("x");
                            a.InnerText = i.ToString();
                            tile.Attributes.Append(a);

                            a = doc.CreateAttribute("y");
                            a.InnerText = j.ToString();
                            tile.Attributes.Append(a);

                            if (Definition.ExportMode == TileLayerDefinition.TileExportMode.XML)
                            {
                                a = doc.CreateAttribute("id");
                                a.InnerText = tiles[i, j].ToString();
                                tile.Attributes.Append(a);
                            }
                            else
                            {
                                Point p = Tileset.GetCellFromID(tiles[i, j]);

                                a = doc.CreateAttribute("tx");
                                a.InnerText = p.X.ToString();
                                tile.Attributes.Append(a);

                                a = doc.CreateAttribute("ty");
                                a.InnerText = p.Y.ToString();
                                tile.Attributes.Append(a);
                            }

                            xml.AppendChild(tile);
                        }
                    }
                }              
            }

            return xml;
        }

        public override bool SetXML(XmlElement xml)
        {
            Clear();

            bool cleanXML = true;

            //Load the tileset
            string tilesetName = xml.Attributes["tileset"].InnerText;
            Tileset = Ogmo.Project.Tilesets.Find(t => t.Name == tilesetName);

            //Get the export mode
            TileLayerDefinition.TileExportMode exportMode;
            if (xml.Attributes["exportMode"] != null)
                exportMode = (TileLayerDefinition.TileExportMode)Enum.Parse(typeof(TileLayerDefinition.TileExportMode), xml.Attributes["exportMode"].InnerText);
            else
                exportMode = Definition.ExportMode;

            if (exportMode == TileLayerDefinition.TileExportMode.CSV || exportMode == TileLayerDefinition.TileExportMode.TrimmedCSV)
            {
                //CSV Import
                string s = xml.InnerText;

                string[] rows = s.Split('\n');
                if (rows.Length > tiles.GetLength(1))
                {
                    Array.Resize(ref rows, tiles.GetLength(1));
                    cleanXML = false;
                }
                for (int i = 0; i < rows.Length; i++)
                {
                    string[] tileStrs = rows[i].Split(',');
                    if (tileStrs.Length > TileCellsX)
                    {
                        Array.Resize(ref tileStrs, TileCellsX);
                        cleanXML = false;
                    }
                    if (tileStrs[0] != "")
                        for (int j = 0; j < tileStrs.Length; j++)
                            tiles[j, i] = Convert.ToInt32(tileStrs[j]);
                }
            }
            else if (exportMode == TileLayerDefinition.TileExportMode.XML || exportMode == TileLayerDefinition.TileExportMode.XMLCoords)
            {
                //XML Import
                foreach (XmlElement tile in xml)
                {
                    int x = Convert.ToInt32(tile.Attributes["x"].InnerText);
                    int y = Convert.ToInt32(tile.Attributes["y"].InnerText);

                    if (x >= Tiles.GetLength(0) || y >= Tiles.GetLength(1))
                    {
                        cleanXML = false;
                        continue;
                    }

                    if (tile.Attributes["id"] != null)
                    {
                        int id = Convert.ToInt32(tile.Attributes["id"].InnerText);
                        tiles[x, y] = id;
                    }
                    else if (tile.Attributes["tx"] != null && tile.Attributes["ty"] != null)
                    {
                        int tx = Convert.ToInt32(tile.Attributes["tx"].InnerText);
                        int ty = Convert.ToInt32(tile.Attributes["ty"].InnerText);
                        tiles[x, y] = Tileset.GetIDFromCell(new Point(tx, ty));
                    }
                }
            }

            BuildTiles();
            return cleanXML;
        }

        public int this[int tx, int ty]
        {
            get
            {
                return tiles[tx, ty];
            }

            set
            {
                if (tiles[tx, ty] != value)
                {
                    tiles[tx, ty] = value;
                    UpdateTile(tx, ty);
                }
            }
        }

        public int[,] Tiles
        {
            get
            {
                return tiles;
            }

            set
            {
                tiles = value;
                BuildTiles();
            }
        }

        public Rectangle GetTilesRectangle(Point start, Point end)
        {
            Rectangle r = new Rectangle();

            //Get the rectangle
            r.X = Math.Min(start.X, end.X);
            r.Y = Math.Min(start.Y, end.Y);
            r.Width = Math.Abs(end.X - start.X) + Definition.Grid.Width;
            r.Height = Math.Abs(end.Y - start.Y) + Definition.Grid.Height;

            //Enforce Bounds
            if (r.X < 0)
            {
                r.Width += r.X;
                r.X = 0;
            }

            if (r.Y < 0)
            {
                r.Height += r.Y;
                r.Y = 0;
            }

            int width = tiles.GetLength(0) * Definition.Grid.Width;
            int height = tiles.GetLength(1) * Definition.Grid.Height;

            if (r.X + r.Width > width)
                r.Width = width - r.X;

            if (r.Y + r.Height > height)
                r.Height = height - r.Y;

            return r;
        }

        public void Clear()
        {
            for (int i = 0; i < tiles.GetLength(0); i++)
                for (int j = 0; j < tiles.GetLength(1); j++)
                    tiles[i, j] = -1;

            BuildTilesEmpty();
        }

        public override LayerEditor GetEditor(LevelEditors.LevelEditor editor)
        {
            return new TileLayerEditor(editor, this);
        }

        public int TileCellsX
        {
            get { return tiles.GetLength(0); }
        }

        public int TileCellsY
        {
            get { return tiles.GetLength(1); }
        }

        public Bitmap Bitmap
        {
            get { return canvas; }
        }

        #region Update the Cache

        private void BuildTilesEmpty()
        {
            if (canvas != null)
            {
                canvas.Dispose();
                canvasGraphics.Dispose();
            }

            canvas = new Bitmap(Level.Size.Width, Level.Size.Height);
            canvasGraphics = Graphics.FromImage(canvas);
            canvasGraphics.CompositingMode = CompositingMode.SourceCopy;
            canvasGraphics.CompositingQuality = CompositingQuality.HighSpeed;
            canvasGraphics.SmoothingMode = SmoothingMode.HighSpeed;
            canvasGraphics.InterpolationMode = InterpolationMode.NearestNeighbor;
        }

        private void BuildTiles()
        {
            BuildTilesEmpty();

            for (int i = 0; i < TileCellsX; i++)
            {
                for (int j = 0; j < TileCellsY; j++)
                {
                    if (tiles[i, j] != -1)
                    {
                        canvasGraphics.DrawImage(Tileset.Bitmap,
                            new Rectangle(i * Definition.Grid.Width, j * Definition.Grid.Height, Definition.Grid.Width, Definition.Grid.Height),
                            Tileset.GetRectFromID(tiles[i, j]),
                            GraphicsUnit.Pixel);
                    }
                }
            }
        }

        private void UpdateTile(int tx, int ty)
        {
            canvasGraphics.DrawImage(Tileset.Bitmap,
                new Rectangle(tx * Definition.Grid.Width, ty * Definition.Grid.Height, Definition.Grid.Width, Definition.Grid.Height),
                Tileset.GetRectFromID(tiles[tx, ty]),
                GraphicsUnit.Pixel);
        }

        #endregion
    }
}
