using UnityEngine;
using System;
using System.Collections.Generic;
using Devdog.General.ThirdParty.UniLinq;
using System.Text;

namespace Devdog.InventoryPro
{
    [System.Serializable]
    public partial class CraftingBlueprintLayout
    {
        [System.Serializable]
        public partial class Row
        {
            [System.Serializable]
            public partial class Cell
            {
                public int index;
                public InventoryItemBase item;
                public int amount = 0;
            }

            public Cell this[int i]
            {
                get
                {
                    return columns[i];
                }
                set
                {
                    columns[i] = value;
                }
            }

            public int index;
            public Cell[] columns = new Cell[0]; // Named columns to avoid breaking previously serialized data (copied through issue detector)
        }

        public int ID;
        public bool enabled = true;
        public Row[] rows = new Row[0];


        public Row this[int i]
        {
            get
            {
                return rows[i];
            }
            set
            {
                rows[i] = value;
            }
        }
    }
}