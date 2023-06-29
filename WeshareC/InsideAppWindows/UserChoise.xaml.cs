using System;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WeshareC.InsideAppWindows;
using System.ComponentModel;
using System.Runtime.CompilerServices;


namespace WeshareC.InsideAppWindows
{
    public partial class UserChoise : Window
    {
        private readonly string connectionString;
        private readonly string loggedInUserName;
        public UserChoise(string userName)
        {
            InitializeComponent();
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();
            connectionString = configuration.GetConnectionString("MyConnectionString");

            loggedInUserName = userName;
            welcomeTextBlock.Text = $"Welcome, {loggedInUserName}!";
        }
        private void CreateGroup_Click(object sender, RoutedEventArgs e)
        {
            this.Close();

            string userName = loggedInUserName;
            SqlConnection sqlConnection = new SqlConnection(connectionString);

            GroupMaking groupMakingWindow = new GroupMaking(userName, sqlConnection);
            groupMakingWindow.Show();
        }
        private void AddPurchase_Click(object sender, RoutedEventArgs e)
        {
            this.Close();

            string userName = loggedInUserName;
            SqlConnection sqlConnection = new SqlConnection(connectionString);

            AddPurchaseWindow addPurchaseWindow = new AddPurchaseWindow(userName, sqlConnection);
            addPurchaseWindow.Show();
        }
        private void DeletePurchase_Click(object sender, RoutedEventArgs e)
        {
            this.Close();

            string userName = loggedInUserName;
            SqlConnection sqlConnection = new SqlConnection(connectionString);

            DeletePurchase deletePurchase = new DeletePurchase(userName, sqlConnection);
            deletePurchase.Show();
        }
        private void DeleteGroup_Click(object sender, RoutedEventArgs e)
        {
            this.Close();

            string userName = loggedInUserName;
            SqlConnection sqlConnection = new SqlConnection(connectionString);

            DeleteGroup deleteGroup = new DeleteGroup(userName, sqlConnection);
            deleteGroup.Show();
        }
        private void OtherFunction_Click(object sender, RoutedEventArgs e)
        {
            this.Close();

            string userName = loggedInUserName;
            SqlConnection sqlConnection = new SqlConnection(connectionString);

            GroupCalculate addPurchaseWindow = new GroupCalculate(userName, sqlConnection);
            addPurchaseWindow.Show();
        }
        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            Close();
        }
    }
}