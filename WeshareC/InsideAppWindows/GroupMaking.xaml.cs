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

namespace WeshareC.InsideAppWindows
{
    public partial class GroupMaking : Window
    {
        private readonly string connectionString;
        private string loggedInUserName;
        private SqlConnection connection;
        private string username;

        public GroupMaking(string userName, SqlConnection sqlConnection)
        {
            InitializeComponent();
            username = userName;
            connection = sqlConnection;

            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();
            connectionString = configuration.GetConnectionString("MyConnectionString");

            loggedInUserName = userName;
        }

        private void btnCreateGroup_Click(object sender, RoutedEventArgs e)
        {
            string groupName = txtGroupName.Text;

            if (string.IsNullOrEmpty(groupName))
            {
                MessageBox.Show("Please enter a group name.");
                return;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Check if the group name already exists
                    string checkQuery = "SELECT COUNT(*) FROM GroupData WHERE GroupName = @GroupName";
                    using (SqlCommand checkCommand = new SqlCommand(checkQuery, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@GroupName", groupName);
                        int count = Convert.ToInt32(checkCommand.ExecuteScalar());

                        if (count > 0)
                        {
                            MessageBox.Show("A group with the same name already exists.");
                            return;
                        }
                    }

                    // Create the group
                    string query = "INSERT INTO GroupData (GroupName, UserName) VALUES (@GroupName, @UserName)";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@GroupName", groupName);
                        command.Parameters.AddWithValue("@UserName", username);

                        command.ExecuteNonQuery();
                        MessageBox.Show("Group created successfully.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error creating group: " + ex.Message);
            }
        }

        private void btnAddToGroup_Click(object sender, RoutedEventArgs e)
        {
            string groupName = txtGroupName.Text;
            string userName = txtPersonName.Text;

            if (string.IsNullOrEmpty(groupName) || string.IsNullOrEmpty(userName))
            {
                MessageBox.Show("Please enter a group name and a person's name.");
                return;
            }

            if (!UserExists(userName))
            {
                MessageBox.Show("User not found.");
                return;
            }

            if (UserExistsInGroup(userName, groupName))
            {
                MessageBox.Show("User already exists in the group.");
                return;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string insertQuery = "INSERT INTO GroupData (UserName, GroupName) VALUES (@Name, @GroupName)";

                    using (SqlCommand command = new SqlCommand(insertQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Name", userName);
                        command.Parameters.AddWithValue("@GroupName", groupName);
                        command.ExecuteNonQuery();
                        MessageBox.Show("Name added to the group successfully.");

                        // Check if group name is not null before calling ShowGroupMembers
                        if (!string.IsNullOrEmpty(groupName))
                        {
                            ShowGroupMembers(groupName);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding name to the group: " + ex.Message);
            }
        }
        private bool UserExists(string userName)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string userExistsQuery = $"SELECT COUNT(*) FROM UserInfo WHERE UserName = @Name";

                    using (SqlCommand command = new SqlCommand(userExistsQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Name", userName);
                        int count = (int)command.ExecuteScalar();

                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error checking user existence: " + ex.Message);
                return false;
            }
        }
        private bool UserExistsInGroup(string userName, string groupName)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string userExistsQuery = $"SELECT COUNT(*) FROM GroupData WHERE UserName = @Name AND GroupName = @GroupName";

                    using (SqlCommand command = new SqlCommand(userExistsQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Name", userName);
                        command.Parameters.AddWithValue("@GroupName", groupName);
                        int count = (int)command.ExecuteScalar();

                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error checking user existence in the group: " + ex.Message);
                return false;
            }
        }
        private void ShowGroupMembers(string groupName)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string selectQuery = "SELECT UserName FROM GroupData WHERE GroupName = @GroupName";

                    using (SqlCommand command = new SqlCommand(selectQuery, connection))
                    {
                        command.Parameters.AddWithValue("@GroupName", groupName);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            List<string> groupMembers = new List<string>();

                            while (reader.Read())
                            {
                                string memberName = reader.GetString(0);
                                groupMembers.Add(memberName);
                            }

                            if (groupMembers.Count > 0)
                            {
                                lbGroupMembers.Items.Clear();

                                foreach (string member in groupMembers)
                                {
                                    lbGroupMembers.Items.Add(member);
                                }
                            }
                            else
                            {
                                MessageBox.Show($"Group {groupName} does not have any members.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error retrieving group members: " + ex.Message);
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