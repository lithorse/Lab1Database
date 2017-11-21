using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
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
                        UpdateMoviesListboxes();
                    }
                }
            });
        }

        private void UpdateMoviesListboxes()
        {
            Task.Run(() =>
            {
                using (conn = new SqlConnection(Datastring))
                {
                    try
                    {
                        conn.Open();
                        SqlCommand command = new SqlCommand(@"SELECT Title + ' ' FROM Movies", conn);
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

        private void ButtonUpdateClick(object sender, RoutedEventArgs e)
        {
            //Updatedi();
        }

        private void ListBoxMovie_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                TextBoxFirstName.Text = (string)ListBoxDirector.SelectedItem;
                TextBoxTitle.Text = (string)ListBoxMovie.SelectedItem;
                //ComboBoxDirectors.SelectedIndex = ListBoxMovie.SelectedIndex - 1;   //Fel atm
            });
        }
    }
}
