using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Lab1Database
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            UpdateDirectorListboxes();
        }

        private SqlConnection conn;
        private String Datastring = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=FilmDatabase;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

        private void UpdateDirectorListboxes()
        {
            Task.Run(() =>
            {
                using (conn = new SqlConnection(Datastring))
                {
                    try
                    {
                        conn.Open();
                        SqlCommand command = new SqlCommand(@"SELECT FirstName + ' ' + LastName FROM Directors", conn);
                        SqlDataReader reader = command.ExecuteReader();
                        Dispatcher.Invoke(() =>
                        {
                            ListBoxDirector.Items.Clear();
                            while (reader.Read())
                            {
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    ListBoxDirector.Items.Add(reader[i]);
                                }
                            }
                        });
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                    }
                    finally
                    {
                        conn.Close();
                        UpdateMoviesListboxes(true);
                    }
                }
            });
        }

        private void UpdateMoviesListboxes(bool needToUpdateComboBoxDirectors)
        {
            Task.Run(() =>
            {
                using (conn = new SqlConnection(Datastring))
                {
                    try
                    {
                        conn.Open();
                        SqlCommand command = new SqlCommand(@"SELECT Title FROM Movies", conn);
                        SqlDataReader reader = command.ExecuteReader();
                        Dispatcher.Invoke(() =>
                        {
                            ListBoxMovie.Items.Clear();
                            while (reader.Read())
                            {
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    ListBoxMovie.Items.Add(reader[i]);
                                }
                            }
                        });
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                    }
                    finally
                    {
                        conn.Close();
                        if (needToUpdateComboBoxDirectors)
                            UpdateComboBoxDirectors();
                    }
                }
            });
        }
        private void UpdateComboBoxDirectors()
        {
            Task.Run(() =>
            {
                using (conn = new SqlConnection(Datastring))
                {
                    try
                    {
                        conn.Open();
                        SqlCommand command = new SqlCommand(@"SELECT FirstName + ' ' + LastName FROM Directors", conn);
                        SqlDataReader reader = command.ExecuteReader();
                        Dispatcher.Invoke(() =>
                        {
                            ComboBoxDirectors.Items.Clear();
                            while (reader.Read())
                            {
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    ComboBoxDirectors.Items.Add(reader[i]);
                                }
                            }
                        });
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            });
        }

        private void ListBoxMovie_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                TextBoxTitle.Text = (string)ListBoxMovie.SelectedItem;
            });
            PrintDataFromMovieId(ListBoxMovie.SelectedIndex + 1);
        }

        private void PrintDataFromMovieId(int Id)
        {
            Task.Run(() =>
            {
                using (conn = new SqlConnection(Datastring))
                {
                    try
                    {
                        conn.Open();
                        SqlCommand command = new SqlCommand(@"SELECT DirectorId FROM Movies WHERE Id=" + Id, conn);
                        SqlDataReader reader = command.ExecuteReader();
                        reader.Read();
                        int DirectorId = (int)reader[0];
                        Dispatcher.Invoke(() =>
                        {
                            LabelDirectorId.Content = "ID: " + DirectorId;
                            ComboBoxDirectors.SelectedIndex = DirectorId - 1;
                        });
                        command = new SqlCommand(@"SELECT FirstName FROM Directors WHERE Id='" + DirectorId + "'", conn);
                        reader.Close();
                        reader = command.ExecuteReader();
                        reader.Read();
                        Dispatcher.Invoke(() =>
                        {
                            TextBoxFirstName.Text = (string)reader[0];
                        });
                        command = new SqlCommand(@"SELECT LastName FROM Directors WHERE Id='" + DirectorId + "'", conn);
                        reader.Close();
                        reader = command.ExecuteReader();
                        reader.Read();
                        Dispatcher.Invoke(() =>
                        {
                            TextBoxLastName.Text = (string)reader[0];
                        });
                        command = new SqlCommand(@"SELECT Id FROM Movies WHERE Id=" + Id, conn);
                        reader.Close();
                        reader = command.ExecuteReader();
                        reader.Read();
                        int MovieId = (int)reader[0];
                        Dispatcher.Invoke(() =>
                        {
                            LabelMovieId.Content = "ID: " + MovieId;
                        });
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            });
        }

        private void ButtonUpdateClick(object sender, RoutedEventArgs e)
        {
            Match firstName = Regex.Match(ComboBoxDirectors.SelectedItem.ToString(), @"\w+");
            UpdateMovie((string)ListBoxMovie.SelectedItem, TextBoxTitle.Text, firstName.Value);
        }

        private void UpdateMovie(string title, string newTitle, string DirectorFirstName)
        {
            Task.Run(() =>
            {
                using (conn = new SqlConnection(Datastring))
                {
                    try
                    {
                        conn.Open();
                        SqlCommand command = new SqlCommand(@"SELECT Id FROM Directors WHERE FirstName = '" + DirectorFirstName + "'", conn);
                        SqlDataReader reader = command.ExecuteReader();
                        reader.Read();
                        int NewDirectorId = (int)reader[0];
                        reader.Close();
                        command = new SqlCommand(@"UPDATE Movies SET Title = '" + newTitle + "' WHERE Title = '" + title + "'", conn);
                        command.ExecuteNonQuery();
                        command = new SqlCommand(@"UPDATE Movies SET DirectorId = '" + NewDirectorId + "' WHERE Title = '" + title + "'", conn);
                        command.ExecuteNonQuery();
                        if (title != newTitle)
                        {
                            Dispatcher.Invoke(() =>
                            {
                                ListBoxMovie.Items.Insert(ListBoxMovie.Items.IndexOf(title), newTitle);
                                ListBoxMovie.SelectedIndex = ListBoxMovie.Items.IndexOf(title) - 1;
                                ListBoxMovie.Items.RemoveAt(ListBoxMovie.Items.IndexOf(title));
                            });
                        }
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            });
        }
    }
}
