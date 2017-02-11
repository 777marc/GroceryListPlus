using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using GroceryListPlus.Helpers;
using GroceryListPlus.DAL;
using System.Collections.ObjectModel;
using Telerik.Windows.Controls;
using System.Windows.Media;

namespace GroceryListPlus
{
    public partial class BuildList : PhoneApplicationPage
    {

        private GLPDB.GroceryListDataContext itemsInDB;
        private ObservableCollection<GLPDB.PopularItem> _popularItems;
        public ObservableCollection<GLPDB.PopularItem> PopularItems
        {
            get
            {
                return _popularItems;
            }
            set
            {
                if (_popularItems != value)
                {
                    _popularItems = value;
                }
            }
        }

        public BuildList()
        {
            InitializeComponent();
            BuildLocalizedApplicationBar();

            // Connect to the database and instantiate data context.
            itemsInDB = new GLPDB.GroceryListDataContext(GLPDB.GroceryListDataContext.DBConnectionString);

            // Data context and observable collection are children of the main page.
            this.DataContext = this;

            loadInitPopularItems();
            loadPopularItems();
            setThemeVisibility();

        }

        private void setThemeVisibility()
        {
            Visibility dbgisibility = (Visibility)Application.Current.Resources["PhoneDarkThemeVisibility"];
            
            if (dbgisibility != Visibility.Visible)
            {
                LayoutRoot.Background = new SolidColorBrush(Colors.LightGray);
            }
            
        }

        private void loadPopularItems()
        {
            var popItems = from GLPDB.PopularItem pi in itemsInDB.PopularItems
                           where pi.PopularItemId > 0
                           select pi;

            PopularItems = new ObservableCollection<GLPDB.PopularItem>(popItems);
            itemsListBox.ItemsSource = PopularItems;
        }
        
        private void BuildLocalizedApplicationBar()
        {
            // Set the page's ApplicationBar to a new instance of ApplicationBar.
            ApplicationBar = new ApplicationBar();

            // Create a new button and set the text value to the localized string from AppResources.
            ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/save.png", UriKind.Relative));
            appBarButton.Text = "Save";
            appBarButton.Click += new EventHandler(save_item);
            ApplicationBar.Buttons.Add(appBarButton);

            ApplicationBarIconButton appBarButton1 = new ApplicationBarIconButton(new Uri("/Assets/AppBar/back.png", UriKind.Relative));
            appBarButton1.Text = "Back";
            appBarButton1.Click += new EventHandler(go_home);
            ApplicationBar.Buttons.Add(appBarButton1);
            
        }

        private void loadInitPopularItems()
        {

            var popItems = from GLPDB.PopularItem pi in itemsInDB.PopularItems
                           where pi.PopularItemId > 0
                           select pi;

            if (popItems.Count() > 0)
            {
                return;
            }

            string[] initItems = new string[] { "eggs", "bread", "milk" };

            foreach (string itm in initItems)
            {
                addNewPopularItem(itm);
            }
        }

        private void save_item(object sender, EventArgs e)
        {

            if (txtItemName.Text == "" | txtItemName.Text == null)
                return;

            GLPDB.GroceryList newListItem = new GLPDB.GroceryList();

            if (txtCost.Text == "" | txtCost.Text == null)
                txtCost.Text = "0.00";

            double price;

            price = Convert.ToDouble(txtCost.Text);

            newListItem.ItemName = txtItemName.Text;
            newListItem.Quantity = rnumQuantity.Value;
            newListItem.Cost = price;
            newListItem.ItemDate = DateTime.Now;

            itemsInDB.GroceryListItems.InsertOnSubmit(newListItem);
            itemsInDB.SubmitChanges();


            if ((bool)chkFavs.IsChecked)
            {
                GLPDB.PopularItem newPI = new GLPDB.PopularItem();

                newPI.ItemName = newListItem.ItemName;
                newPI.Cost = newListItem.Cost;
                newPI.ItemDate = DateTime.Today;

                itemsInDB.PopularItems.InsertOnSubmit(newPI);
                itemsInDB.SubmitChanges();
                
                loadPopularItems();
            }

            MessageBox.Show("Item Saved!");

            txtCost.Text = "";
            txtItemName.Text = "";


        }

        private void addNewPopularItem(string itemName)
        {
            
            GLPDB.PopularItem newPopItem = new GLPDB.PopularItem();

            newPopItem.ItemDate = DateTime.Now;
            newPopItem.ItemName = itemName;

            itemsInDB.PopularItems.InsertOnSubmit(newPopItem);
            itemsInDB.SubmitChanges();

        }

        private void go_home(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/PivMain.xaml", UriKind.Relative));
        }

        private void delItem_Click(object sender, RoutedEventArgs e)
        {

            if (MessageBox.Show("Are you sure you want to delete this item?", "Delete Grocery item?", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                var selItem = sender as Button;

                GLPDB.PopularItem itemToDelete = selItem.DataContext as GLPDB.PopularItem;

                itemsInDB.PopularItems.DeleteOnSubmit(itemToDelete);
                itemsInDB.SubmitChanges();
                loadPopularItems();
            }

        }

        private void OnFailedToReceiveAd(object sender, GoogleAds.AdErrorEventArgs e)
        {

        }

        private void OnAdReceived(object sender, GoogleAds.AdEventArgs e)
        {

        }

        private void popPrice_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            RadInputPrompt.Show("Select a new price for this item:", closedHandler: (args) =>
            {

                var selItem = sender as TextBlock;

                string newPrice = args.Text;
                double newPriceAsDouble = 0.0;
                var selectedItm = selItem.DataContext as GLPDB.PopularItem;

                if (Double.TryParse(newPrice, out newPriceAsDouble))
                {

                    GLPDB.PopularItem updatedPopItem = itemsInDB.PopularItems.First(i => i.ItemName == selectedItm.ItemName);

                    if (updatedPopItem != null)
                    {
                        updatedPopItem.Cost = newPriceAsDouble;
                    }

                    itemsInDB.SubmitChanges();

                    loadInitPopularItems();
                    loadPopularItems();

                }
                else
                {
                    MessageBox.Show("Invalid price.");
                    return;
                }

            });
        }

        private void popName_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            GLPDB.PopularItem newListItem = new GLPDB.PopularItem();

            newListItem = (GLPDB.PopularItem)itemsListBox.SelectedValue;

            if (newListItem == null)
                return;

            GLPDB.GroceryList addedListItem = new GLPDB.GroceryList();

            addedListItem.ItemName = newListItem.ItemName;
            addedListItem.Quantity = 1;
            addedListItem.Cost = newListItem.Cost;
            addedListItem.ItemDate = DateTime.Today;

            itemsInDB.GroceryListItems.InsertOnSubmit(addedListItem);
            itemsInDB.SubmitChanges();

            MessageBox.Show("Item Added to Your List!");
        }


    }
}