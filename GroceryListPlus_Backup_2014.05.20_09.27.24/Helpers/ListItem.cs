using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Windows.Controls.DataForm;

namespace GroceryListPlus.Helpers
{
    class ListItem
    {
        [FieldInfo(FieldHeader = "Item Name")]
        private string _itemName;
        public string ItemName
        {
            get
            {
                return _itemName;
            }
            set
            {
                _itemName = value;
            }
        }

        private int _quantity;
        public int Quantity
        {
            get
            {
                return _quantity;
            }
            set
            {
                _quantity = value;
            }
        }

        private double _cost;
        public double Cost
        {
            get
            {
                return _cost;
            }
            set
            {
                _cost = value;
            }
        }

        private bool _favorite;
        public bool Favorite
        {
            get
            {
                return _favorite;
            }
            set
            {
                _favorite = value;
            }
        }
    }
}
