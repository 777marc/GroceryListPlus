using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Collections.ObjectModel;
using GroceryListPlus.Helpers;
using GroceryListPlus.DAL;
using Microsoft.Phone.Tasks;
using Telerik.Windows.Controls;
using System.Windows.Media;
using Windows.ApplicationModel.Store;

namespace GroceryListPlus
{
    public partial class PivMain : PhoneApplicationPage
    {

        private GLPDB.GroceryListDataContext itemsInDB;
        private ObservableCollection<GLPDB.GroceryList> _listItems;
        public ObservableCollection<GLPDB.GroceryList> ListItems
        {
            get
            {
                return _listItems;
            }
            set
            {
                if (_listItems != value)
                {
                    _listItems = value;
                }
            }
        }

        private ObservableCollection<GLPDB.GroceryList> _savedLists;
        public ObservableCollection<GLPDB.GroceryList> SavedLists
        {
            get
            {
                return _savedLists;
            }
            set
            {
                if (_savedLists != value)
                {
                    _savedLists = value;
                }
            }
        }

        public PivMain()
        {
            InitializeComponent();

            //BuildLocalizedApplicationBar();

            // Connect to the database and instantiate data context.
            itemsInDB = new GLPDB.GroceryListDataContext(GLPDB.GroceryListDataContext.DBConnectionString);

            // Data context and observable collection are children of the main page.
            this.DataContext = this;

            //initializeList();
            setThemeVisibility();
            
            RadRateApplicationReminder radRAReminder = new RadRateApplicationReminder();

            radRAReminder.RecurrencePerUsageCount = 3;
            radRAReminder.SkipFurtherRemindersOnYesPressed = true;
            radRAReminder.Notify();

            checkRemoveAdsLicesnse();

        }

        private void checkRemoveAdsLicesnse()
        {
            var licenseInformation = CurrentApp.LicenseInformation;

            if (licenseInformation.ProductLicenses["GLP_NoAds"].IsActive)
            {
                App.SHOWADS = false;
            }
            else
            {
                App.SHOWADS = true;
            }
        }

        private void setThemeVisibility()
        {
            Visibility dbgisibility = (Visibility)Application.Current.Resources["PhoneDarkThemeVisibility"];

            if (dbgisibility != Visibility.Visible)
            {
                LayoutRoot.Background = new SolidColorBrush(Colors.White);
            }
            
        }

        private void initializeList()
        {
            lisGrocery.DataContext = getGroceryList();
            lisSavedLists.DataContext = getSavedLists();
        }

        private List<string> getSavedLists()
        {
            List<string> savedLists;

            var lists = itemsInDB.SavedLists.Select(l => l.ListName).Distinct();

            savedLists = new List<string>(lists);

            if (savedLists.Count() > 0)
            {
                txtNoDataSavedList.Visibility = System.Windows.Visibility.Collapsed;
                txtblkSavedTip.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                txtNoDataSavedList.Visibility = System.Windows.Visibility.Visible;
                txtblkSavedTip.Visibility = System.Windows.Visibility.Collapsed;

            }
                

           return savedLists;

        }

        private ObservableCollection<GLPDB.GroceryList> getGroceryList()
        {

            double tally = 0.0;

            ObservableCollection<GLPDB.GroceryList> myList = new ObservableCollection<GLPDB.GroceryList>();

            var listItems = from GLPDB.GroceryList li in itemsInDB.GroceryListItems
                           where li.ListItemId > 0
                           select li;

            if (listItems.Count() > 0)
            {
                txtNoData.Visibility = System.Windows.Visibility.Collapsed;
                
                foreach(var itm in listItems)
                {
                    tally = tally + (itm.Cost * itm.Quantity);
                }

                App.TALLY = tally; //listItems.Sum(i => i.Cost);
               
                
                txtTally.Text = String.Format("Total: {0:C}", App.TALLY);
            }
            else
            {
                txtNoData.Visibility = System.Windows.Visibility.Visible;
            }

            myList = new ObservableCollection<GLPDB.GroceryList>(listItems);
            
            return myList;

        }

        private ObservableCollection<GLPDB.GroceryList> getGroceryList(string listName)
        {
            ObservableCollection<GLPDB.GroceryList> groceryListCollection;

            var listItems = from  li in itemsInDB.SavedLists
                                where li.ListName == listName
                                select li;


            foreach (GLPDB.SavedList itm in listItems)
            {
                GLPDB.GroceryList gli = new GLPDB.GroceryList();

                gli.ItemName = itm.ItemName;
                gli.Cost = itm.Cost;
                gli.Quantity = itm.Quantitiy;
                gli.ItemDate = DateTime.Today;

                itemsInDB.GroceryListItems.InsertOnSubmit(gli);

            }

            itemsInDB.SubmitChanges();

            var listItemsNew = from GLPDB.GroceryList li in itemsInDB.GroceryListItems
                               where li.ListItemId > 0
                               select li;

            if (listItems.Count() > 0)
            {
                txtNoData.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                txtNoData.Visibility = System.Windows.Visibility.Visible;
            }

            App.TALLY = listItems.Sum(i => i.Cost);
            txtTally.Text = String.Format("Total: {0:C}", App.TALLY);

            groceryListCollection = new ObservableCollection<GLPDB.GroceryList>(listItemsNew);

            return groceryListCollection;

        }

        private void go_donate(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Donate.xaml", UriKind.Relative));
        }

        private void restore_list(object sender, EventArgs e)
        {
            App.DoneOnListItems.Clear();

            lisGrocery.DataContext = getGroceryList();
        }

        private void delete_list(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete this list?", "Delete Grocery List?", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {

                var toDelete = itemsInDB.GroceryListItems.Where(l => l.ListItemId >= 0);
                itemsInDB.GroceryListItems.DeleteAllOnSubmit(toDelete);
                itemsInDB.SubmitChanges();

                MessageBox.Show("The list has been deleted");
                initializeList();
                App.TALLY = 0;
                txtTally.Text = String.Format("Total: {0:C}", App.TALLY);

            }
        }

        private void save_list(object sender, EventArgs e)
        {

            ListItems = getGroceryList();

            if (ListItems.Count() == 0)
            {
                MessageBox.Show("There are no items on your list to save.");
                return;
            }

            RadInputPrompt.Show("Select a name for the list:", closedHandler: (args) =>
            {
                string listName = args.Text;

                if (listName.Length == 0)
                {
                    return;
                }

                ListItems = getGroceryList();

                foreach (GLPDB.GroceryList itm in ListItems)
                {
                    GLPDB.SavedList newListToSave = new GLPDB.SavedList();

                    newListToSave.ItemName = itm.ItemName;
                    newListToSave.ItemDate = DateTime.Today;
                    newListToSave.Cost = itm.Cost;
                    newListToSave.ListName = listName;

                    itemsInDB.SavedLists.InsertOnSubmit(newListToSave);
                }

                itemsInDB.SubmitChanges();

                MessageBox.Show("Your list has been saved!");

                lisSavedLists.DataContext = getSavedLists();

            });

        }

        private void new_list(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/BuildList.xaml" , UriKind.Relative));
        }

        private void share_list(object sender, EventArgs e)
        {
            SmsComposeTask smsComposeTask = new SmsComposeTask();

            string smsList = "";

            ListItems = getGroceryList();

            if (ListItems.Count == 0)
            {
                MessageBox.Show("nonthing on list");
                return;
            }

            foreach (GLPDB.GroceryList itm in ListItems)
            {
                smsList += itm.ItemName + "\r";
            }

            smsComposeTask.Body = smsList;

            smsComposeTask.Show();
        }

        private void chkDone_Click(object sender, RoutedEventArgs e)
        {
            var check = sender as Button;

            GLPDB.GroceryList glitm = check.DataContext as GLPDB.GroceryList;

            App.DoneOnListItems.Add(glitm);

            ListItems = getGroceryList();


            foreach (GLPDB.GroceryList itm in App.DoneOnListItems)
            {
                ListItems.Remove(itm);
            }

            lisGrocery.DataContext = ListItems;


        }
            
        private void chkDel_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete this item?", "Delete list item?", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                var check = sender as Button;
                GLPDB.GroceryList glitm = check.DataContext as GLPDB.GroceryList;
                itemsInDB.GroceryListItems.DeleteOnSubmit(glitm);
                itemsInDB.SubmitChanges();

                lisGrocery.DataContext = getGroceryList();
            }
        }

        private void chkFav_Click(object sender, RoutedEventArgs e)
        {
            var check = sender as Button;
            
            GLPDB.GroceryList glitm = check.DataContext as GLPDB.GroceryList;

            GLPDB.PopularItem newPopItem = new GLPDB.PopularItem();

            newPopItem.ItemName = glitm.ItemName;
            newPopItem.Cost = glitm.Cost;
            newPopItem.ItemDate = DateTime.Today;

            itemsInDB.PopularItems.InsertOnSubmit(newPopItem);

            itemsInDB.SubmitChanges();

            MessageBox.Show("Item has been added to your favorites!");

        }

        private void lisSavedLists_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (lisSavedLists.SelectedValue == null)
                return;

            string listName = lisSavedLists.SelectedValue.ToString();

            RadMessageBox.Show("Do you want to overwrite the current list?", MessageBoxButtons.YesNo , closedHandler: (args) =>
            {
                DialogResult result = args.Result;

                int buttonIndex = args.ButtonIndex;

                if(buttonIndex == 0)
                {
                    var toDelete = itemsInDB.GroceryListItems.Where(l => l.ListItemId >= 0);
                    itemsInDB.GroceryListItems.DeleteAllOnSubmit(toDelete);
                    itemsInDB.SubmitChanges();

                    lisGrocery.DataContext = getGroceryList(listName);
                    mainPivControl.SelectedIndex = 0;
                }
                else
                {
                    lisGrocery.DataContext = getGroceryList(listName);
                    mainPivControl.SelectedIndex = 0;
                }

                Button clickedButton = args.ClickedButton;
            });
            


        }

        private void delSavedList(object sender, RoutedEventArgs e)
        {

            if (MessageBox.Show("Are you sure you want to delete this list?", "Delete Grocery List?", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                var listToDelete = sender as Button;

                string listName = listToDelete.DataContext as string;

                if (listName == null)
                    return;

                List<GLPDB.SavedList> sl = itemsInDB.SavedLists.Where(l => l.ListName == listName).ToList();

                itemsInDB.SavedLists.DeleteAllOnSubmit(sl);
                itemsInDB.SubmitChanges();
                MessageBox.Show("The list has been deleted");
                initializeList();

            }

        }

        private void TextBlock_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            RadInputPrompt.Show("Select a new price for this item:", closedHandler: (args) =>
            {

                var selItem = sender as TextBlock;

                string newPrice = args.Text;
                double newPriceAsDouble = 0.0;
                var selectedItm = selItem.DataContext as GLPDB.GroceryList;

                if (Double.TryParse(newPrice, out newPriceAsDouble))
                {

                    GLPDB.GroceryList updatedItem = itemsInDB.GroceryListItems.First(i => i.ListItemId == selectedItm.ListItemId);
                    updatedItem.Cost = newPriceAsDouble;

                    try
                    {
                        GLPDB.PopularItem updatedPopItem = itemsInDB.PopularItems.First(i => i.ItemName == selectedItm.ItemName);

                        if (updatedPopItem != null)
                        {
                            updatedPopItem.Cost = newPriceAsDouble;
                        }

                    }
                    catch (Exception)
                    {
                        // continue
                    }

                    itemsInDB.SubmitChanges();

                    lisGrocery.DataContext = getGroceryList();

                }
                else
                {
                    MessageBox.Show("Invalid price.");
                    return;
                }

            });
        }

        private void btnEmailDev_Click(object sender, RoutedEventArgs e)
        {
            string emailacct = "support@alminasoftware.com";

            EmailComposeTask emailComposeTask = new EmailComposeTask();

            emailComposeTask.Subject = "Message From Grocery List Plus App";
            emailComposeTask.Body = "insert your message here";
            emailComposeTask.To = emailacct;

            emailComposeTask.Show();
        }

        private void btnRate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var rateTask = new MarketplaceReviewTask();
                rateTask.Show();
            }
            catch
            {
            }
        }

        private void quant_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            RadInputPrompt.Show("Select a new quantity for this item:", closedHandler: (args) =>
            {

                var selItem = sender as TextBlock;

                string newQuantity = args.Text;
                double newQuantityAsDouble = 0.0;
                var selectedItm = selItem.DataContext as GLPDB.GroceryList;

                if (Double.TryParse(newQuantity, out newQuantityAsDouble))
                {

                    GLPDB.GroceryList updatedItem = itemsInDB.GroceryListItems.First(i => i.ListItemId == selectedItm.ListItemId);
                    updatedItem.Quantity = (int)newQuantityAsDouble;

                    itemsInDB.SubmitChanges();

                    lisGrocery.DataContext = getGroceryList();

                }
                else
                {
                    MessageBox.Show("Invalid quantity.");
                    return;
                }

            });
        }

        private void icoNewList_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            mainPivControl.SelectedIndex = 1;
        }

        private void txtNewList_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            mainPivControl.SelectedIndex = 1;
        }

        private void icoSaved_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            mainPivControl.SelectedIndex = 2;
        }

        private void txtSavedLists_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            mainPivControl.SelectedIndex = 2;
        }

        private void txtAppInfo_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            mainPivControl.SelectedIndex = 3;
        }

        private void icoInfo_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            mainPivControl.SelectedIndex = 3;
        }

        private void mainPivControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(mainPivControl.SelectedIndex == 1)
            {

                //load any existing lists
                initializeList();

                // Set the page's ApplicationBar to a new instance of ApplicationBar.
                ApplicationBar = new ApplicationBar();

                // Create a new button and set the text value to the localized string from AppResources.
                ApplicationBarIconButton newListButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/add.png", UriKind.Relative));
                newListButton.Text = "New Item";
                newListButton.Click += new EventHandler(new_list);
                ApplicationBar.Buttons.Add(newListButton);

                ApplicationBarIconButton shareListButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/share.png", UriKind.Relative));
                shareListButton.Text = "Share List";
                shareListButton.Click += new EventHandler(share_list);
                ApplicationBar.Buttons.Add(shareListButton);

                ApplicationBarIconButton saveListButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/save.png", UriKind.Relative));
                saveListButton.Text = "Save List";
                saveListButton.Click += new EventHandler(save_list);
                ApplicationBar.Buttons.Add(saveListButton);

                ApplicationBarIconButton deleteListButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/delete.png", UriKind.Relative));
                deleteListButton.Text = "Delete List";
                deleteListButton.Click += new EventHandler(delete_list);
                ApplicationBar.Buttons.Add(deleteListButton);

                ApplicationBarMenuItem restoreList = new ApplicationBarMenuItem();
                restoreList.Text = "Restore List";
                restoreList.Click += new EventHandler(restore_list);
                ApplicationBar.MenuItems.Add(restoreList);

                ApplicationBarMenuItem mnuDonate = new ApplicationBarMenuItem();
                mnuDonate.Text = "Remove Ads";
                mnuDonate.Click += new EventHandler(go_donate);
                ApplicationBar.MenuItems.Add(mnuDonate);

            }
            
            if(mainPivControl.SelectedIndex != 1)
            {
                ApplicationBar = new ApplicationBar();

                ApplicationBarIconButton newListButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/feature.settings.png", UriKind.Relative));
                newListButton.Text = "Remove Ads";
                newListButton.Click += new EventHandler(remov_ads);
                ApplicationBar.Buttons.Add(newListButton);
            }

        }

        private void remov_ads(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Donate.xaml", UriKind.Relative));
        }

        private void adsHome_Loaded(object sender, RoutedEventArgs e)
        {
            if (App.SHOWADS)
            {
                adsHome.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                adsHome.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void adsSaved_Loaded(object sender, RoutedEventArgs e)
        {
            if (App.SHOWADS)
            {
                adsSaved.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                adsSaved.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void adsSettings_Loaded(object sender, RoutedEventArgs e)
        {
            if (App.SHOWADS)
            {
                adsSettings.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                adsSettings.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            initializeList();
        }
    }
}