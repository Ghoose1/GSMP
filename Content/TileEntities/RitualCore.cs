using System;
using System.Collections.Generic;
using Terraria.ModLoader;
using Terraria;
using GSMP.DataStructures;
using Terraria.ModLoader.IO;
using Microsoft.Xna.Framework;
using Terraria.ObjectData;

namespace GSMP.Content.TileEntities
{
    public class RitualCoreTE : ModTileEntity
    {
        public int radius;
        public List<KeyValuePair<RitualObject, int>> objects; // Ritual object, amount

        public bool active;

        internal int timer;

        public override bool IsTileValidForEntity(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            return ManaTEutils.RitualCores.Contains(tile.TileType) && tile.HasTile;
        }

        public override void Update()
        {
            // Checking if things are active, but only once every second (for lag)
            if (timer != 300)
            {
                timer++;
                return;
            }
            timer = 0;

            Main.NewText("Searching Tiles...");
            Main.NewText($"Base X: {Position.X}, Base Y: {Position.Y}");

            List<KeyValuePair<RitualObject, int>> temp = objects;
            List<Point> foundTiles = new List<Point>();

            foundTiles.Add(new Point(Position.X, Position.Y)); // So we dont search this tile

            bool failflag= false;

            foreach (KeyValuePair<RitualObject, int> pair in temp)
            {
                RitualObject obj = pair.Key;

                int num = pair.Value;

                Main.NewText($"Looking for object... " +
                    $"\nKind: {obj.kind}" +
                    $"\nType: {obj.type}" +
                    $"\nMax: {obj.maxMana}" +
                    $"\nMana: {obj.storedMana}" +
                    $"\nNum: {num}");
                for (int x = Position.X - radius; x <= Position.X + radius && num != 0; x++)
                    for (int y = Position.Y - radius; y <= Position.Y + radius && num != 0; y++)
                    {
                        Tile tile = Main.tile[x, y];
                        int X2 = x;
                        int Y2 = y;

                        TileObjectData data = TileObjectData.GetTileData(tile.TileType, 0);
                        if (data != null && data.Width != 1 && data.Height != 1) // If the tile is a mutlityle, have the checks use the origin/ top left tile
                        {
                            X2 = x - (tile.TileFrameX / 18);
                            Y2 = y - (tile.TileFrameY / 18);
                            tile = Main.tile[X2, Y2];
                        }

                        if (!tile.HasTile || foundTiles.Contains(new Point(X2, Y2))) continue;

                        switch (obj.kind) 
                        {
                            case 0: // null, i.e. should not turn up here
                                {
                                    Main.NewText("error: null ritual object detected and ignored");
                                    break; 
                                }
                            case 1: break; // Mana storage tile
                            case 2: // Candles (with specific buff)
                                {
                                    if (num != 0 && tile.TileType == ModContent.TileType<Tiles.PotionBurner>() && Tiles.PotionBurner.GetBuff(X2, Y2) == obj.type)
                                    {
                                        Main.NewText($"Found tile at {X2}, {Y2}");
                                        foundTiles.Add(new Point(X2, Y2));
                                        num--;
                                    }
                                    break;
                                }
                            case 3: // Just plain tiles
                                {
                                    if (tile.TileType == obj.type && num != 0)
                                    {
                                        Main.NewText($"Found tile at {X2}, {Y2}");
                                        foundTiles.Add(new Point(X2, Y2));
                                        num--;
                                    }
                                    else Main.NewText($"Wrong tile. Type: {tile.TileType}, X: {X2}, Y: {Y2}, needed: {obj.type}");
                                    break;
                                }
                            default: break;
                        }
                    }

                if (num != 0)
                {
                    Main.NewText("Tile not found");
                    failflag = true;
                    break;
                }
            }

            active = false;
            if (!failflag)
            {
                Main.NewText("Tile found, effect active");
                active = true;
            }
        }

        public override void SaveData(TagCompound tag)
        {
            tag.Add("radius", radius);
            tag.Add("objects", objects);
        }

        public override void LoadData(TagCompound tag)
        {
            if (tag.ContainsKey("radius")) radius = tag.Get<int>("radius");
            if (tag.ContainsKey("objects")) objects = tag.Get<List<KeyValuePair<RitualObject, int>>>("objects");
        }

        // TODO: This TE finds other ritual TEs in a radius around it
        //       If it finds the required ones for a certin ritual it will trigger that ritual and apply the appropriate effects to the tiles.
        //       This can range from giving a special buff and depleting mana to things like summoning bosses and large changes to the world.
    }
}