using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace GSMP.Content.TileEntities
{
    public class ManaStorageEntity : ModTileEntity
    {

        public int MaxMana; // MaxMana and TransferRate are set by the tile creating the entity and not modified after
        public int TransferRate; // Mana per tick from this
        public int RotationNum;

        public int StoredMana;
        public List<Vector2> ConnectionsTo = new(); // Links between these entities 
        public List<Vector2> ConnectionsFrom = new();

        public override bool IsTileValidForEntity(int x, int y)
        {
            return ManaTEutils.IsConnectionValid(Main.tile[x, y].TileType) && Main.tile[x, y].HasTile;
        }

        public override void Update() // Used to check if Tiles this tile is connected to are still active and to transfer mana to linked Entities
        {
            for (int k = 0; k < ConnectionsTo.Count; k++)
            {
                Tile tile = Main.tile[(int)ConnectionsTo[k].X, (int)ConnectionsTo[k].Y];
                if (tile == null || !tile.HasTile/* || !ManaTEutils.IsConnectionValid(tile.TileType)*/)
                    ConnectionsTo.RemoveAt(k);
            }

            for (int k = 0; k < ConnectionsFrom.Count; k++)
            {
                Tile tile = Main.tile[(int)ConnectionsFrom[k].X, (int)ConnectionsFrom[k].Y];
                if (tile == null || !tile.HasTile || !ManaTEutils.IsConnectionValid(tile.TileType))
                    ConnectionsFrom.RemoveAt(k);
            }

            if (ConnectionsTo.Count >= 1)
            {
                RotationNum = TransferMana(TransferRate > StoredMana ? StoredMana : TransferRate, ConnectionsTo, RotationNum);
                StoredMana = TransferRate > StoredMana ? 0 : StoredMana - TransferRate;
            }
        }

        public override void SaveData(TagCompound tag)
        {
            if (MaxMana != 1) tag.Add("MaxMana", MaxMana);
            if (TransferRate != 1) tag.Add("TransferRate", TransferRate);

            if (StoredMana != 0) tag.Add("StoredMana", StoredMana);
            tag.Add("ConnectionsTo", ConnectionsTo);
            tag.Add("ConnectionsFrom", ConnectionsFrom);
        }

        public override void LoadData(TagCompound tag)
        {
            MaxMana = 1;
            if (tag.ContainsKey("MaxMana"))
                MaxMana = tag.Get<int>("MaxMana");

            TransferRate = 1;
            if (tag.ContainsKey("TransferRate"))
                TransferRate = tag.Get<int>("TransferRate");



            StoredMana = 0;
            if (tag.ContainsKey("StoredMana"))
                StoredMana = tag.Get<int>("StoredMana");

            ConnectionsTo = new List<Vector2>();
            if (tag.ContainsKey("ConnectionsTo"))
                ConnectionsTo = tag.Get<List<Vector2>>("ConnectionsTo");

            ConnectionsFrom = new List<Vector2>();
            if (tag.ContainsKey("ConnectionsFrom"))
                ConnectionsFrom = tag.Get<List<Vector2>>("ConnectionsFrom");
        }

        public int TransferMana(int num, List<Vector2> connections, int RotationNum = 0)
        {
            int DistributeMana(int num1, int[] maxes, int[] previous, int[] changes, int RotationNum = 0)
            {
                int[] current = new int[previous.Length];
                for (int k = 0; k < previous.Length; k++)
                    current[k] = previous[k];

                int num = num1;

                // Default amount to try to add
                int average = (int)Math.Floor(num / (float)connections.Count);

                // Gets distributed separately to the bulk of the mana
                int remainder = num % connections.Count;

                for (int k = 0; k < current.Length; k++) // modifying the current and change values for the average values
                {
                    if (current[k] + average <= maxes[k]) // if it can fit the value just add the average / target value
                    {
                        changes[k] += average;
                        current[k] += average;
                        num -= average;
                    }
                    else // if it cant, change the current value to its max value etc.
                    {
                        changes[k] += maxes[k] - current[k];
                        num -= maxes[k] - current[k];
                        current[k] = maxes[k];
                    }
                }

                bool flag2 = true;
                //Main.NewText($"Remainder: {remainder}");
                //Main.NewText($"RotationNum: {RotationNum}");
                while (remainder > 0)
                {
                    if (current[RotationNum] < maxes[RotationNum]) // remainder will always be less than amount of things
                    {
                        changes[RotationNum]++;
                        current[RotationNum]++;
                        remainder--;
                        num--;
                    }
                    if (RotationNum < current.Length) RotationNum++;
                    else
                    {
                        if (flag2) flag2 = false;
                        else break;
                        RotationNum = 0;
                    }
                }

                if (num <= 0 || current == previous || num == num1) // num being zero indicates that mana has been distributed, current == prev indicates that it did not do anything
                {
                    //string str1 = "Previous: ";
                    //string str2 = "Changes: ";
                    //string str3 = "Final: ";

                    //for (int k = 0; k < current.Length; k++)
                    //{
                    //    str1 += $"{previous[k]} | ";
                    //    str2 += $"{changes[k]} | ";
                    //    str3 += $"{current[k]} | ";
                    //}

                    //Main.NewText("Final: ");
                    //Main.NewText(str1);
                    //Main.NewText(str2);
                    //Main.NewText(str3);

                    for (int k = 0; k < current.Length; k++)
                    {
                        // Applying all the calculated changes
                        ManaTEutils.Mana((int)connections[k].X, (int)connections[k].Y, changes[k]);
                    }
                    return RotationNum;
                }
                else
                {
                    //string str1 = "Previous: ";
                    //string str2 = "Changes: ";
                    //string str3 = "Final: ";

                    //for (int k = 0; k < current.Length; k++)
                    //{
                    //    str1 += $"{previous[k]} | ";
                    //    str2 += $"{changes[k]} | ";
                    //    str3 += $"{current[k]} | ";
                    //}

                    //Main.NewText("Cycle: ");
                    //Main.NewText(str1);
                    //Main.NewText(str2);
                    //Main.NewText(str3);

                    DistributeMana(num, maxes, current, changes, RotationNum); // Re-running the code with updated changes and mana arrays
                    return 0;
                }
            }

            int[] changes = new int[connections.Count];
            int[] maxes = new int[connections.Count];
            int[] current = new int[connections.Count];

            for (int k = 0; k < connections.Count; k++)
            {
                current[k] = ManaTEutils.Mana((int)connections[k].X, (int)connections[k].Y);
                maxes[k] = ManaTEutils.MaxMana((int)connections[k].X, (int)connections[k].Y);
            }

            if (RotationNum >= connections.Count) RotationNum = 0;


            //string str1 = "Initial: ";
            //for (int k = 0; k < ConnectionsTo.Count; k++)
            //    str1 += $"{current[k]} | ";

            //Main.NewText("Transfer: ");
            //Main.NewText(str1);

            RotationNum = DistributeMana(num, maxes, current, changes, RotationNum);
            return RotationNum;
        }
    }
}