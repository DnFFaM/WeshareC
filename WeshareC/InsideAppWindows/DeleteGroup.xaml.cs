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
    public partial class DeleteGroup : Window
    {
        private readonly string connectionString;
        private readonly SqlConnection connection;
        private readonly string loggedInUserName;
        private readonly string userName;

        public DeleteGroup(string userName, SqlConnection sqlConnection)
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
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading groups: " + ex.Message);
            }
        }


        private void BtnDeleteGroup_Click(object sender, RoutedEventArgs e)
        {
            string selectedGroup = cmbGroups.SelectedItem as string;

            if (string.IsNullOrEmpty(selectedGroup))
            {
                MessageBox.Show("Please select a group to delete.");
                return;
            }

            if (!chkDeleteWholeGroup.IsChecked.HasValue || !chkDeleteWholeGroup.IsChecked.Value)
            {
                MessageBox.Show("Please check the 'Delete whole group' checkbox to confirm group deletion.");
                return;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Verify if the user is a member of the selected group
                    string verifyMembershipQuery = "SELECT COUNT(*) FROM GroupData WHERE GroupName = @GroupName AND UserName = @UserName";
                    using (SqlCommand verifyMembershipCommand = new SqlCommand(verifyMembershipQuery, connection))
                    {
                        verifyMembershipCommand.Parameters.AddWithValue("@GroupName", selectedGroup);
                        verifyMembershipCommand.Parameters.AddWithValue("@UserName", loggedInUserName);

                        int count = (int)verifyMembershipCommand.ExecuteScalar();

                        if (count == 0)
                        {
                            MessageBox.Show("You are not a member of the selected group. You cannot delete it.");
                            return;
                        }
                    }

                    // Delete purchases related to the group
                    string deletePurchasesQuery = "DELETE FROM Purchases WHERE GroupName = @GroupName";
                    using (SqlCommand deletePurchasesCommand = new SqlCommand(deletePurchasesQuery, connection))
                    {
                        deletePurchasesCommand.Parameters.AddWithValue("@GroupName", selectedGroup);
                        deletePurchasesCommand.ExecuteNonQuery();
                    }

                    // Delete group from Groups table
                    string deleteGroupQuery = "DELETE FROM Groups WHERE GroupName = @GroupName";
                    using (SqlCommand deleteGroupCommand = new SqlCommand(deleteGroupQuery, connection))
                    {
                        deleteGroupCommand.Parameters.AddWithValue("@GroupName", selectedGroup);
                        deleteGroupCommand.ExecuteNonQuery();
                    }

                    MessageBox.Show("Group deleted successfully.");
                    cmbGroups.Items.Remove(selectedGroup);
                    cmbGroups.SelectedIndex = -1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting group: " + ex.Message);
            }
        }
        private void GoBack_Click(object sender, RoutedEventArgs e)
        {
            string userName = loggedInUserName;
            UserChoise userChoice = new UserChoise(userName);
            userChoice.Show();
            Close();
        }
    }
}
