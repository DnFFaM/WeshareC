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
    public partial class DeletePurchase : Window
    {
        private readonly string connectionString;
        private readonly SqlConnection connection;
        private readonly string loggedInUserName;
        private readonly string userName;

        public DeletePurchase(string userName, SqlConnection sqlConnection)
        {
            InitializeComponent();
            connection = sqlConnection;
            this.userName = userName;
            loggedInUserName = userName; // Assign the value to loggedInUserName

            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();
            connectionString = configuration.GetConnectionString("MyConnectionString");

            ReadAllGroups();
        }
        private void ReadAllGroups()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT DISTINCT Groups.GroupName " +
                                   "FROM Groups " +
                                   "INNER JOIN GroupData ON Groups.GroupName = GroupData.GroupName " +
                                   "WHERE GroupData.UserName = @UserName";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@UserName", userName);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string groupName = reader.GetString(0);
                                cmbGroups.Items.Add(groupName);
                            }
                        }
                    }

                    // Check user group connection
                    string selectedGroup = cmbGroups.SelectedItem as string;
                    if (!string.IsNullOrEmpty(selectedGroup))
                    {
                        bool isConnected = CheckUserGroupConnection(userName, selectedGroup);
                        if (!isConnected)
                        {
                            MessageBox.Show("You are not connected to this group.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading groups: " + ex.Message);
            }
        }
        private bool CheckUserGroupConnection(string userName, string groupName)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT COUNT(*) FROM GroupData WHERE UserName = @UserName AND GroupName = @GroupName";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@UserName", userName);
                        command.Parameters.AddWithValue("@GroupName", groupName);

                        int count = (int)command.ExecuteScalar();
                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error checking user group connection: " + ex.Message);
                return false;
            }
        }
        private void CmbGroups_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cmbPurchases.Items.Clear();

            string selectedGroup = cmbGroups.SelectedItem as string;

            if (string.IsNullOrEmpty(selectedGroup))
                return;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Retrieve purchases made by the logged-in user
                    string query = "SELECT Item FROM Purchases WHERE UserName = @UserName AND GroupName = @GroupName";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@UserName", userName);
                        command.Parameters.AddWithValue("@GroupName", selectedGroup);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string itemName = reader.GetString(0);
                                cmbPurchases.Items.Add(itemName);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading purchases: " + ex.Message);
            }
        }
        private void BtnDeletePurchases_Click(object sender, RoutedEventArgs e)
        {
            List<string> selectedItems = new List<string>();
            for (int i = 0; i < cmbPurchases.Items.Count; i++)
            {
                if (cmbPurchases.SelectedIndex == i)
                {
                    selectedItems.Add(cmbPurchases.Items[i].ToString());
                }
            }

            if (selectedItems.Count == 0)
            {
                MessageBox.Show("Please select purchases to delete.");
                return;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    foreach (string selectedItem in selectedItems)
                    {
                        string query = "DELETE FROM Purchases WHERE Item = @Item";
                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@Item", selectedItem);
                            command.ExecuteNonQuery();
                        }

                        cmbPurchases.Items.Remove(selectedItem);
                    }

                    MessageBox.Show("Selected purchases deleted successfully.");
                    cmbPurchases.SelectedIndex = -1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting purchases: " + ex.Message);
            }
        }
        private void GoBack_Click(object sender, RoutedEventArgs e)
        {
            string userName = loggedInUserName;
            UserChoise userChoise = new UserChoise(userName);
            userChoise.Show();
            Close();
        }
    }
}