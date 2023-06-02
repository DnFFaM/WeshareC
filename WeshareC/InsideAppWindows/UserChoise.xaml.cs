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


namespace WeshareC.InsideAppWindows
{
    public partial class UserChoise : Window
    {
        private readonly string connectionString;
        private string loggedInUserName;
        public UserChoise(string userName)
        {
            InitializeComponent();
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();
            connectionString = configuration.GetConnectionString("MyConnectionString");

            loggedInUserName = userName;
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

            AddPurchaseWindow addPurchaseWindow= new AddPurchaseWindow(userName, sqlConnection);
            addPurchaseWindow.Show();
        }
        private void OtherFunction_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Other Function selected");
        }
        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            Close();
        }
    }
}