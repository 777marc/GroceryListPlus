using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroceryListPlus.DAL
{
    public class GLPDB
    {
        public class GroceryListDataContext : DataContext
        {
            // Specify the connection string as a static, used in main page and app.xaml.
            public static string DBConnectionString = "Data Source=isostore:/GroceryList.sdf";

            // Pass the connection string to the base class.
            public GroceryListDataContext(string connectionString)
                : base(connectionString)
            { }

            // Specify a single table 
            public Table<GroceryList> GroceryListItems;

            public Table<PopularItem> PopularItems;

            public Table<SavedList> SavedLists;

        }

        [Table]
        public class GroceryList : INotifyPropertyChanged, INotifyPropertyChanging
        {
            // Define ID: private field, public property and database column.
            private int _listItemId;
            [Column(IsPrimaryKey = true, IsDbGenerated = true, DbType = "INT NOT NULL Identity", CanBeNull = false, AutoSync = AutoSync.OnInsert)]
            public int ListItemId
            {
                get
                {
                    return _listItemId;
                }
                set
                {
                    if (_listItemId != value)
                    {
                        NotifyPropertyChanging("ListItemId");
                        _listItemId = value;
                        NotifyPropertyChanged("ListItemId");
                    }
                }
            }

            private DateTime _itemDate;
            [Column]
            public DateTime ItemDate
            {
                get
                {
                    return _itemDate;
                }
                set
                {
                    if (_itemDate != value)
                    {
                        NotifyPropertyChanging("ItemDate");
                        _itemDate = value;
                        NotifyPropertyChanged("ItemDate");
                    }
                }
            }

            private double _cost;
            [Column]
            public double Cost
            {
                get
                {
                    return _cost;
                }
                set
                {
                    if (_cost != value)
                    {
                        NotifyPropertyChanging("Distance");
                        _cost = value;
                        NotifyPropertyChanged("Distance");
                    }
                }
            }

            
            private string _itemName;
            [Column]
            public string ItemName
            {
                get
                {
                    return _itemName;
                }
                set
                {
                    if (_itemName != value)
                    {
                        NotifyPropertyChanging("RunType");
                        _itemName = value;
                        NotifyPropertyChanged("RunType");
                    }
                }
            }

            private double _quantity;
            [Column]
            public double Quantity
            {
                get
                {
                    return _quantity;
                }
                set
                {
                    if (_quantity != value)
                    {
                        NotifyPropertyChanging("Temp");
                        _quantity = value;
                        NotifyPropertyChanged("Temp");
                    }
                }
            }
                        
            #region INotifyPropertyChanged Members

            public event PropertyChangedEventHandler PropertyChanged;

            // Used to notify the page that a data context property changed
            private void NotifyPropertyChanged(string propertyName)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
            }

            #endregion

            #region INotifyPropertyChanging Members

            public event PropertyChangingEventHandler PropertyChanging;

            // Used to notify the data context that a data context property is about to change
            private void NotifyPropertyChanging(string propertyName)
            {
                if (PropertyChanging != null)
                {
                    PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
                }
            }

            #endregion

        }

        [Table]
        public class PopularItem : INotifyPropertyChanged, INotifyPropertyChanging
        {
            // Define ID: private field, public property and database column.
            private int _popularItemId;
            [Column(IsPrimaryKey = true, IsDbGenerated = true, DbType = "INT NOT NULL Identity", CanBeNull = false, AutoSync = AutoSync.OnInsert)]
            public int PopularItemId
            {
                get
                {
                    return _popularItemId;
                }
                set
                {
                    if (_popularItemId != value)
                    {
                        NotifyPropertyChanging("PopularItemId");
                        _popularItemId = value;
                        NotifyPropertyChanged("PopularItemId");
                    }
                }
            }

            private DateTime _itemDate;
            [Column]
            public DateTime ItemDate
            {
                get
                {
                    return _itemDate;
                }
                set
                {
                    if (_itemDate != value)
                    {
                        NotifyPropertyChanging("ItemDate");
                        _itemDate = value;
                        NotifyPropertyChanged("ItemDate");
                    }
                }
            }

            private double _cost;
            [Column]
            public double Cost
            {
                get
                {
                    return _cost;
                }
                set
                {
                    if (_cost != value)
                    {
                        NotifyPropertyChanging("Distance");
                        _cost = value;
                        NotifyPropertyChanged("Distance");
                    }
                }
            }

            private string _itemName;
            [Column]
            public string ItemName
            {
                get
                {
                    return _itemName;
                }
                set
                {
                    if (_itemName != value)
                    {
                        NotifyPropertyChanging("RunType");
                        _itemName = value;
                        NotifyPropertyChanged("RunType");
                    }
                }
            }

            #region INotifyPropertyChanged Members

            public event PropertyChangedEventHandler PropertyChanged;

            // Used to notify the page that a data context property changed
            private void NotifyPropertyChanged(string propertyName)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
            }

            #endregion

            #region INotifyPropertyChanging Members

            public event PropertyChangingEventHandler PropertyChanging;

            // Used to notify the data context that a data context property is about to change
            private void NotifyPropertyChanging(string propertyName)
            {
                if (PropertyChanging != null)
                {
                    PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
                }
            }

            #endregion

        }

        [Table]
        public class SavedList : INotifyPropertyChanged, INotifyPropertyChanging
        {
            // Define ID: private field, public property and database column.
            private int _listItemId;
            [Column(IsPrimaryKey = true, IsDbGenerated = true, DbType = "INT NOT NULL Identity", CanBeNull = false, AutoSync = AutoSync.OnInsert)]
            public int ListItemId
            {
                get
                {
                    return _listItemId;
                }
                set
                {
                    if (_listItemId != value)
                    {
                        NotifyPropertyChanging("ListItemId");
                        _listItemId = value;
                        NotifyPropertyChanged("ListItemId");
                    }
                }
            }

            private DateTime _itemDate;
            [Column]
            public DateTime ItemDate
            {
                get
                {
                    return _itemDate;
                }
                set
                {
                    if (_itemDate != value)
                    {
                        NotifyPropertyChanging("ItemDate");
                        _itemDate = value;
                        NotifyPropertyChanged("ItemDate");
                    }
                }
            }

            private double _cost;
            [Column]
            public double Cost
            {
                get
                {
                    return _cost;
                }
                set
                {
                    if (_cost != value)
                    {
                        NotifyPropertyChanging("Distance");
                        _cost = value;
                        NotifyPropertyChanged("Distance");
                    }
                }
            }

            private string _itemName;
            [Column]
            public string ItemName
            {
                get
                {
                    return _itemName;
                }
                set
                {
                    if (_itemName != value)
                    {
                        NotifyPropertyChanging("RunType");
                        _itemName = value;
                        NotifyPropertyChanged("RunType");
                    }
                }
            }

            private string _listName;
            [Column]
            public string ListName
            {
                get
                {
                    return _listName;
                }
                set
                {
                    if (_listName != value)
                    {
                        NotifyPropertyChanging("ListName");
                        _listName = value;
                        NotifyPropertyChanged("ListName");
                    }
                }
            }
            
            private double _quantity;
            [Column]
            public double Quantitiy
            {
                get
                {
                    return _quantity;
                }
                set
                {
                    if (_quantity != value)
                    {
                        NotifyPropertyChanging("Temp");
                        _quantity = value;
                        NotifyPropertyChanged("Temp");
                    }
                }
            }

            #region INotifyPropertyChanged Members

            public event PropertyChangedEventHandler PropertyChanged;

            // Used to notify the page that a data context property changed
            private void NotifyPropertyChanged(string propertyName)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
            }

            #endregion

            #region INotifyPropertyChanging Members

            public event PropertyChangingEventHandler PropertyChanging;

            // Used to notify the data context that a data context property is about to change
            private void NotifyPropertyChanging(string propertyName)
            {
                if (PropertyChanging != null)
                {
                    PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
                }
            }

            #endregion

        }


    }
}
