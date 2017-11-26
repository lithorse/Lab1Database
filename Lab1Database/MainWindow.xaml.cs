using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
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
                        reader.Close();
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
            if (ListBoxMovie.Items.Count != 0) //Needed when list is cleared
            {
                Dispatcher.Invoke(() =>
                {
                    TextBoxTitle.Text = (string)ListBoxMovie.SelectedItem;
                });
                PrintDataFromMovieId(ListBoxMovie.SelectedValue.ToString());
            }
        }

        private void PrintDataFromMovieId(string movieTitle)
        {
            Task.Run(() =>
            {
                using (conn = new SqlConnection(Datastring))
                {
                    try
                    {
                        conn.Open();
                        SqlCommand command = new SqlCommand($@"SELECT DirectorId FROM Movies WHERE Title LIKE '{movieTitle}'", conn);
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
                        command = new SqlCommand($@"SELECT Id FROM Movies WHERE Title LIKE '{movieTitle}'", conn);
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

        private void ListBoxDirectorSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PrintDataFromDirectorId(ListBoxDirector.SelectedIndex + 1);
        }

        private void PrintDataFromDirectorId(int Id)
        {
            Task.Run(() =>
            {
                using (conn = new SqlConnection(Datastring))
                {
                    try
                    {
                        conn.Open();


                        SqlCommand command = new SqlCommand(@"SELECT FirstName FROM Directors WHERE Id=" + Id, conn);
                        SqlDataReader reader = command.ExecuteReader();
                        reader.Read();
                        string firstName = (string)reader[0];
                        Dispatcher.Invoke(() =>
                        {
                            LabelDirectorId.Content = "ID: " + Id;
                            TextBoxFirstName.Text = firstName;
                        });
                        command = new SqlCommand(@"SELECT LastName FROM Directors WHERE Id=" + Id, conn);
                        reader.Close();
                        reader = command.ExecuteReader();
                        reader.Read();
                        Dispatcher.Invoke(() =>
                        {
                            TextBoxLastName.Text = (string)reader[0];
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

        private void ButtonUpdateMovieClick(object sender, RoutedEventArgs e)
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
                        command = new SqlCommand(@"UPDATE Movies SET DirectorId = '" + NewDirectorId + "' WHERE Title = '" + title + "'", conn);
                        command.ExecuteNonQuery();
                        command = new SqlCommand(@"UPDATE Movies SET Title = '" + newTitle + "' WHERE Title = '" + title + "'", conn);
                        command.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                    }
                    finally
                    {
                        conn.Close();
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
                }
            });
        }

        private void ButtonUpdateDirectorClick(object sender, RoutedEventArgs e)
        {
            UpdateDirector((string)ListBoxDirector.SelectedItem, TextBoxFirstName.Text, TextBoxLastName.Text);
        }

        private void UpdateDirector(string directorFullName, string newFirstName, string newLastName)
        {
            Task.Run(() =>
            {
                using (conn = new SqlConnection(Datastring))
                {
                    try
                    {
                        conn.Open();
                        Match oldFirstName = Regex.Match(directorFullName, @"\w+");
                        SqlCommand command = new SqlCommand(@"SELECT Id FROM Directors WHERE FirstName = '" + oldFirstName.Value + "'", conn);
                        SqlDataReader reader = command.ExecuteReader();
                        reader.Read();
                        int directorId = (int)reader[0];
                        reader.Close();
                        command = new SqlCommand(@"UPDATE Directors SET FirstName = '" + newFirstName + "' WHERE Id = " + directorId, conn);
                        command.ExecuteNonQuery();
                        command = new SqlCommand(@"UPDATE Directors SET LastName = '" + newLastName + "' WHERE Id = " + directorId, conn);
                        command.ExecuteNonQuery();
                        command = new SqlCommand(@"SELECT FirstName + ' ' + LastName FROM Directors WHERE Id = " + directorId, conn);
                        reader.Close();
                        reader = command.ExecuteReader();
                        reader.Read();
                        Dispatcher.Invoke(() =>
                        {
                            ComboBoxDirectors.Items.Insert(ComboBoxDirectors.Items.IndexOf(directorFullName), (string)reader[0]);
                            ComboBoxDirectors.Items.RemoveAt(ComboBoxDirectors.Items.IndexOf(directorFullName));
                        });
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                    }
                    finally
                    {
                        conn.Close();
                        Dispatcher.Invoke(() =>
                        {
                            ListBoxDirector.Items.Insert(ListBoxDirector.Items.IndexOf(directorFullName), newFirstName + " " + newLastName);
                            ListBoxDirector.SelectedIndex = ListBoxDirector.Items.IndexOf(directorFullName) - 1;
                            ListBoxDirector.Items.RemoveAt(ListBoxDirector.Items.IndexOf(directorFullName));
                        });
                    }
                }
            });
        }

        private void ButtonAddMovie_Click(object sender, RoutedEventArgs e)
        {
            Task.Run(() =>
            {
                using (conn = new SqlConnection(Datastring))
                {
                    try
                    {
                        conn.Open();
                        SqlCommand command = new SqlCommand(@"SELECT MAX(Id) FROM Movies", conn);   //To get Movie Id for new Movie
                        SqlDataReader reader = command.ExecuteReader();
                        reader.Read();
                        int newMovieId = (int)reader[0] + 1;
                        reader.Close();


                        Dispatcher.Invoke(() =>
                        {
                            string director = ComboBoxDirectors.SelectedValue.ToString();
                            string newMovieTitle = TextBoxTitle.Text.Trim();

                            //Add view
                            command = new SqlCommand($@"DROP VIEW IF EXISTS DirectorsView ", conn); //Drop old view
                            command.ExecuteNonQuery();
                            command = new SqlCommand($@"CREATE VIEW DirectorsView AS SELECT FirstName +' '+ LastName AS FullName, Id FROM Directors", conn); //Create new view
                            command.ExecuteNonQuery();


                            //Look for duplicate of title
                            command = new SqlCommand($@"SELECT COUNT(Title) FROM Movies WHERE Title LIKE '{newMovieTitle}'", conn);
                            reader = command.ExecuteReader();
                            reader.Read();
                            int countMovieName = (int)reader[0];
                            reader.Close();

                            if (countMovieName < 1)
                            {
                                command = new SqlCommand($@"SELECT Id FROM DirectorsView WHERE FullName LIKE '{director}'", conn);
                                reader = command.ExecuteReader();
                                reader.Read();
                                int directorId = (int)reader[0];
                                reader.Close();

                                //Add Movie
                                command = new SqlCommand($@"INSERT INTO Movies( Id, Title, DirectorId) VALUES ({newMovieId},'{newMovieTitle}', {directorId})", conn);
                                command.ExecuteNonQuery();
                            }
                            else
                            {
                                MessageBox.Show($"Duplicate title entry.\n- A movie titled '{newMovieTitle}' by '{director}' already exists.");
                            }

                        });

                    }
                    catch (Exception ee)
                    {
                        MessageBox.Show(ee.Message);
                    }
                    finally
                    {
                        conn.Close();
                        //Dispatcher.Invoke(() =>
                        //{
                        //});
                        UpdateMoviesListboxes(false);
                    }
                }
            });
        }

        private void ButtonDeleteMovie_Click(object sender, RoutedEventArgs e)
        {
            Task.Run(() =>
            {
                using (conn = new SqlConnection(Datastring))
                {
                    try
                    {
                        conn.Open();

                        Dispatcher.Invoke(() =>
                        {
                            string director = ComboBoxDirectors.SelectedValue.ToString();
                            string MovieTitle = TextBoxTitle.Text.Trim();

                            //Add view
                            SqlCommand command = new SqlCommand($@"DROP VIEW IF EXISTS DirectorsView ", conn); //Drop old view
                            command.ExecuteNonQuery();
                            command = new SqlCommand($@"CREATE VIEW DirectorsView AS SELECT FirstName +' '+ LastName AS FullName, Id FROM Directors", conn); //Create new view
                            command.ExecuteNonQuery();

                            //Fetch director's Id
                            command = new SqlCommand($@"SELECT Id FROM DirectorsView WHERE FullName LIKE '{director}'", conn);
                            SqlDataReader reader = command.ExecuteReader();
                            reader.Read();
                            int directorId = (int)reader[0];
                            reader.Close();

                            //Delete Movie
                            command = new SqlCommand($@"DELETE FROM Movies WHERE Title LIKE '{MovieTitle}' AND DirectorId LIKE {directorId}", conn);
                            command.ExecuteNonQuery();
                        });

                    }
                    catch (Exception ee)
                    {
                        MessageBox.Show(ee.Message);
                    }
                    finally
                    {
                        conn.Close();
                        UpdateMoviesListboxes(false);
                    }
                }
            });
        }

        private void NewUpdate()    //for removal later when everything is working
        {
            Task.Run(() =>
            {
                using (conn = new SqlConnection(Datastring))
                {
                    try
                    {
                        Dispatcher.Invoke(() =>
                        {
                            ListBoxMovie.Items.Clear();
                        });
                        conn.Open();
                        SqlCommand command = new SqlCommand(@"SELECT Title FROM Movies", conn);
                        SqlDataReader reader = command.ExecuteReader();

                        Dispatcher.Invoke(() =>
                        {
                            while (reader.Read())
                            {
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    ListBoxMovie.Items.Add(reader[i]);
                                }
                            }
                            reader.Close();
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

        private void ForceUpdate_click(object sender, RoutedEventArgs e)
        {
            NewUpdate();
        }
    }
}