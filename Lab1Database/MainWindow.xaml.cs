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
                        reader.Close();
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
                            if (ComboBoxDirectors.Items.Count > 0)
                            {
                                ComboBoxDirectors.SelectedIndex = 0;
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
                PrintDataFromMovieTitle(ListBoxMovie.SelectedValue.ToString());
            }
            Dispatcher.Invoke(() =>
            {
                //ListBoxDirector.SelectedIndex = -1;
                bool nothingSelected = ListBoxMovie.SelectedIndex != -1;
                ButtonMovieDelete.IsEnabled = nothingSelected;
                ButtonMovieUpdate.IsEnabled = nothingSelected;
            });
        }

        private void PrintDataFromMovieTitle(string movieTitle)
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
            if (ListBoxDirector.SelectedIndex >= 0)
                PrintDataFromDirectorName((string)ListBoxDirector.SelectedItem);
            Dispatcher.Invoke(() =>
            {
                //ListBoxMovie.SelectedIndex = -1;
                bool nothingSelected = ListBoxDirector.SelectedIndex != -1;
                ButtonDirectorDelete.IsEnabled = nothingSelected;
                ButtonDirectorUpdate.IsEnabled = nothingSelected;
            });
        }

        private void PrintDataFromDirectorName(string DirectorFullName)
        {
            Task.Run(() =>
            {
                using (conn = new SqlConnection(Datastring))
                {
                    try
                    {
                        conn.Open();
                        SqlCommand command = new SqlCommand($@"DROP VIEW IF EXISTS DirectorsView ", conn); //Drop old view
                        command.ExecuteNonQuery();
                        command = new SqlCommand($@"CREATE VIEW DirectorsView AS SELECT FirstName +' '+ LastName AS FullName, Id FROM Directors", conn); //Create new view
                        command.ExecuteNonQuery();
                        command = new SqlCommand(@"SELECT Id FROM DirectorsView WHERE FullName = '" + DirectorFullName + "'", conn);
                        SqlDataReader reader = command.ExecuteReader();
                        reader.Read();
                        int Id = (int)reader[0];
                        reader.Close();

                        command = new SqlCommand(@"SELECT FirstName FROM Directors WHERE Id=" + Id, conn);
                        reader = command.ExecuteReader();
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
            UpdateMovie((string)ListBoxMovie.SelectedItem, TextBoxTitle.Text, ComboBoxDirectors.SelectedItem.ToString());
        }

        private void UpdateMovie(string title, string newTitle, string DirectorFullName)
        {
            Task.Run(() =>
            {
                using (conn = new SqlConnection(Datastring))
                {
                    try
                    {
                        DisableInput();
                        conn.Open();
                        SqlCommand command = new SqlCommand($@"DROP VIEW IF EXISTS DirectorsView ", conn); //Drop old view
                        command.ExecuteNonQuery();
                        command = new SqlCommand($@"CREATE VIEW DirectorsView AS SELECT FirstName +' '+ LastName AS FullName, Id FROM Directors", conn); //Create new view
                        command.ExecuteNonQuery();

                        command = new SqlCommand(@"SELECT Id FROM DirectorsView WHERE FullName = '" + DirectorFullName + "'", conn);
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
                        EnableInput();
                        if (title != newTitle)
                        {
                            Dispatcher.Invoke(() =>
                            {
                                ListBoxMovie.Items.Insert(ListBoxMovie.Items.IndexOf(title), newTitle);
                                ListBoxMovie.SelectedIndex = ListBoxMovie.Items.IndexOf(title) - 1;
                                ListBoxMovie.Items.RemoveAt(ListBoxMovie.Items.IndexOf(title));
                            });
                        }
                        Dispatcher.Invoke(() =>
                        {
                            ButtonMovieUpdate.IsEnabled = true;
                            ButtonMovieDelete.IsEnabled = true;
                        });
                    }
                }
            });
        }

        private void ButtonUpdateDirectorClick(object sender, RoutedEventArgs e)
        {
            if ((string)ListBoxDirector.SelectedItem != TextBoxFirstName.Text + " " + TextBoxLastName.Text)
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
                        DisableInput();
                        conn.Open();
                        SqlCommand command = new SqlCommand($@"DROP VIEW IF EXISTS DirectorsView ", conn); //Drop old view
                        command.ExecuteNonQuery();
                        command = new SqlCommand($@"CREATE VIEW DirectorsView AS SELECT FirstName +' '+ LastName AS FullName, Id FROM Directors", conn); //Create new view
                        command.ExecuteNonQuery();

                        command = new SqlCommand(@"SELECT Id FROM DirectorsView WHERE FullName = '" + directorFullName + "'", conn);
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
                            ComboBoxDirectors.SelectedIndex = ComboBoxDirectors.Items.IndexOf((string)reader[0]);
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
                        Dispatcher.Invoke(() =>
                        {
                            ListBoxDirector.Items.Insert(ListBoxDirector.Items.IndexOf(directorFullName), newFirstName + " " + newLastName);
                            ListBoxDirector.SelectedIndex = ListBoxDirector.Items.IndexOf(directorFullName) - 1;
                            ListBoxDirector.Items.RemoveAt(ListBoxDirector.Items.IndexOf(directorFullName));
                        });
                        EnableInput();
                    }
                }
            });
        }

        private void ButtonAddMovie_Click(object sender, RoutedEventArgs e)
        {
            AddMovie(TextBoxTitle.Text.Trim());
        }

        private void AddMovie(string newMovieTitle)
        {
            Task.Run(() =>
            {
                using (conn = new SqlConnection(Datastring))
                {
                    try
                    {
                        DisableInput();
                        conn.Open();
                        SqlCommand command = new SqlCommand(@"SELECT MAX(Id) FROM Movies", conn);   //To get Movie Id for new Movie
                        SqlDataReader reader = command.ExecuteReader();
                        reader.Read();
                        int newMovieId = (int)reader[0] + 1;
                        reader.Close();


                        Dispatcher.Invoke(() =>
                        {
                            string director = ComboBoxDirectors.SelectedValue.ToString();

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
                                MessageBox.Show($"Duplicate title entry.\n- A movie titled '{newMovieTitle}' already exists.");
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
                        UpdateMoviesListboxes(false);
                        Dispatcher.Invoke(() =>
                        {
                            ListBoxMovie.SelectedItem = newMovieTitle;
                        });
                        EnableInput();
                    }
                }
            });
        }

        private void ButtonDeleteMovie_Click(object sender, RoutedEventArgs e)
        {
            DeleteMovie();
        }

        private void DeleteMovie()
        {
            Task.Run(() =>
            {
                using (conn = new SqlConnection(Datastring))
                {
                    try
                    {
                        DisableInput();
                        conn.Open();
                        Dispatcher.Invoke(() =>
                        {
                            string director = ComboBoxDirectors.SelectedValue.ToString();
                            string MovieTitle = (string)ListBoxMovie.SelectedItem;

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
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                    }
                    finally
                    {
                        conn.Close();
                        UpdateMoviesListboxes(false);
                        EnableInput();
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
                        DisableInput();
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
                        EnableInput();
                    }
                }
            });
        }

        private void ButtonDirectorAddClick(object sender, RoutedEventArgs e)
        {
            AddDirector(TextBoxFirstName.Text.Trim(), TextBoxLastName.Text.Trim());
        }

        private void AddDirector(string newFirstName, string newLastName)
        {
            Task.Run(() =>
            {
                using (conn = new SqlConnection(Datastring))
                {
                    try
                    {
                        DisableInput();
                        conn.Open();
                        SqlCommand command = new SqlCommand(@"SELECT MAX(Id) FROM Directors", conn);   //To get Movie Id for new Movie
                        SqlDataReader reader = command.ExecuteReader();
                        reader.Read();
                        int newDirectorId = (int)reader[0] + 1;
                        reader.Close();

                        Dispatcher.Invoke(() =>
                        {


                            //Look for duplicate of title
                            command = new SqlCommand($@"SELECT COUNT(Id) FROM Directors WHERE FirstName LIKE '{newFirstName}' AND LastName LIKE '{newLastName}'", conn);
                            reader = command.ExecuteReader();
                            reader.Read();

                            bool nameExists = (int)reader[0] > 0;

                            reader.Close();

                            if (!nameExists)
                            {
                                //Add Director
                                command = new SqlCommand($@"INSERT INTO Directors( Id, FirstName, LastName) VALUES ({newDirectorId},'{newFirstName}', '{newLastName}')", conn);
                                command.ExecuteNonQuery();
                                Dispatcher.Invoke(() =>
                                {
                                    ListBoxDirector.Items.Add(newFirstName + " " + newLastName);
                                    ComboBoxDirectors.Items.Add(newFirstName + " " + newLastName);
                                });
                            }
                            else
                            {
                                MessageBox.Show($"Duplicate title entry.\n- A director named {newFirstName} {newLastName} already exists.");
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
                        EnableInput();
                        Dispatcher.Invoke(() =>
                        {
                            ListBoxDirector.SelectedItem = newFirstName + " " + newLastName;
                        });
                    }
                }
            });
        }

        private void ButtonDirectorDeleteClick(object sender, RoutedEventArgs e)
        {
            DeleteDirector((string)ListBoxDirector.SelectedItem);
        }

        private void DeleteDirector(string director)
        {
            Task.Run(() =>
            {
                using (conn = new SqlConnection(Datastring))
                {
                    try
                    {
                        DisableInput();
                        conn.Open();
                        Dispatcher.Invoke(() =>
                        {
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
                            //Check if movie dependent on director
                            command = new SqlCommand($@"SELECT COUNT(DirectorId) FROM Movies WHERE DirectorId LIKE {directorId}", conn);
                            reader = command.ExecuteReader();
                            reader.Read();
                            int movieExists = (int)reader[0];
                            reader.Close();


                            if (movieExists < 1)
                            {
                                //Delete Director
                                command = new SqlCommand($@"DELETE FROM Directors WHERE Id = {directorId}", conn);
                                command.ExecuteNonQuery();
                                Dispatcher.Invoke(() =>
                                {
                                    ListBoxDirector.Items.RemoveAt(ListBoxDirector.Items.IndexOf(director));
                                    if (ComboBoxDirectors.SelectedIndex == ComboBoxDirectors.Items.IndexOf(director))
                                        ComboBoxDirectors.SelectedIndex = 0;
                                    ComboBoxDirectors.Items.RemoveAt(ComboBoxDirectors.Items.IndexOf(director));
                                    ListBoxDirector.SelectedIndex = -1;
                                });
                            }
                            else
                            {
                                MessageBox.Show("Can't delete a director with a movie dependent.");
                                ButtonDirectorUpdate.IsEnabled = true;
                                ButtonDirectorDelete.IsEnabled = true;
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
                        EnableInput();

                    }
                }
            });
        }

        private void DisableInput()
        {
            Dispatcher.Invoke(() =>
            {
                ListBoxMovie.IsEnabled = false;
                ListBoxDirector.IsEnabled = false;
                ButtonMovieUpdate.IsEnabled = false;
                ButtonAddMovie.IsEnabled = false;
                ButtonMovieDelete.IsEnabled = false;
                ButtonDirectorUpdate.IsEnabled = false;
                ButtonDirectorAdd.IsEnabled = false;
                ButtonDirectorDelete.IsEnabled = false;
                TextBoxTitle.IsEnabled = false;
                TextBoxFirstName.IsEnabled = false;
                TextBoxLastName.IsEnabled = false;
                ComboBoxDirectors.IsEnabled = false;
            });
        }

        private void EnableInput()
        {
            Dispatcher.Invoke(() =>
            {
                ListBoxMovie.IsEnabled = true;
                ListBoxDirector.IsEnabled = true;
                ButtonAddMovie.IsEnabled = TextBoxTitle.Text != "";
                ButtonDirectorAdd.IsEnabled = TextBoxFirstName.Text != "" && TextBoxLastName.Text != "";
                TextBoxTitle.IsEnabled = true;
                TextBoxFirstName.IsEnabled = true;
                TextBoxLastName.IsEnabled = true;
                ComboBoxDirectors.IsEnabled = true;
            });
        }

        private void TextBoxTitleTextChanged(object sender, TextChangedEventArgs e)
        {
            bool hasNoText = String.IsNullOrEmpty(TextBoxTitle.Text);
            if (ButtonAddMovie != null && ButtonMovieUpdate != null)
            {
                ButtonAddMovie.IsEnabled = !hasNoText;
                if (ListBoxMovie.SelectedIndex != -1)
                    ButtonMovieUpdate.IsEnabled = !hasNoText;
            }
        }

        private void TextBoxFirstNameTextChanged(object sender, TextChangedEventArgs e)
        {
            if (TextBoxLastName != null)
            {
                bool hasNoText = String.IsNullOrEmpty(TextBoxFirstName.Text) || String.IsNullOrEmpty(TextBoxLastName.Text);
                if (ButtonDirectorAdd != null && ButtonDirectorUpdate != null)
                {
                    ButtonDirectorAdd.IsEnabled = !hasNoText;
                    if (ListBoxDirector.SelectedIndex != -1)
                        ButtonDirectorUpdate.IsEnabled = !hasNoText;
                }
            }
        }

        private void textBoxLastNameChanged(object sender, TextChangedEventArgs e)
        {
            if (TextBoxFirstName != null)
            {
                bool hasNoText = String.IsNullOrEmpty(TextBoxFirstName.Text) || String.IsNullOrEmpty(TextBoxLastName.Text);
                if (ButtonDirectorAdd != null && ButtonDirectorUpdate != null)
                {
                    ButtonDirectorAdd.IsEnabled = !hasNoText;
                    if (ListBoxDirector.SelectedIndex != -1)
                        ButtonDirectorUpdate.IsEnabled = !hasNoText;
                }
            }
        }
    }
}
