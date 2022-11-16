using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace Gyumolcsok
{
    public partial class Form1 : Form
    {
        MySqlConnection conn = null;
        MySqlCommand cmd = null;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder();
            builder.Server = "localhost";
            builder.UserID = "root";
            builder.Password = "";
            builder.Database = "gyumolcsok";
            conn = new MySqlConnection(builder.ConnectionString);

            try
            {
                //-- terv szerint
                conn.Open();
                cmd = conn.CreateCommand();
            }
            catch (MySqlException ex)
            {
                //-- váratlan hiba!
                MessageBox.Show(ex.Message + Environment.NewLine + "A program leáll!!");
                Environment.Exit(0);

            }
            finally
            {
                //-- Hiba és terv szerinti esetén is lefut
                conn.Close();
            }
            
        }
        private void gyumolcs_update()
        {
            listBox_gyum.Items.Clear();
            cmd.CommandText = "SELECT `id`, `nev`, `egysegar`, `mennyiseg` FROM `gyumolcs` WHERE 1;";
            conn.Open();
            using (MySqlDataReader dr = cmd.ExecuteReader())
            {
                while (dr.Read())
                {

                    gyumolcs uj = new gyumolcs(dr.GetInt32("id"), dr.GetString("nev"), dr.GetInt32("egysegar"), dr.GetInt32("mennyiseg"));
                    listBox_gyum.Items.Add(uj);

                }

            }
            conn.Close();
        }

        private void insert_Click(object sender, EventArgs e)
        {
            if (nincsenek_adatok())
            {
                return;
            }
            //-- Kiírjuk Adatbázisba ----
            cmd.CommandText = "INSERT INTO `gyumolcs` (`id`, `nev`, `egysegar`, `mennyiseg`) VALUES (NULL, @nev, @egysegar, @mennyiseg)";
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@nev", textBox_nev.Text);
            cmd.Parameters.AddWithValue("@egysegar", textBox_egysegar.Text);
            cmd.Parameters.AddWithValue("@mennyiseg", textBox_egysegar.Text);
            conn.Open();
            try
            {

                if (cmd.ExecuteNonQuery() == 1)
                {
                    MessageBox.Show("Sikeresen rögzítve");
                    textBox_id.Text = "";
                    textBox_nev.Text = "";
                    textBox_egysegar.Text = "";
                    textBox_egysegar.Text = "";
                }
                else
                {
                    MessageBox.Show("Sikertelen rögzítés!");
                }

            }
            catch (MySqlException ex)
            {

                MessageBox.Show(ex.Message, "Sikertelen!");

            }
            conn.Close();
            gyumolcs_update();
        }

        private bool nincsenek_adatok()
        {
            if (string.IsNullOrEmpty(textBox_nev.Text))
            {
                MessageBox.Show("Adjon meg egy gyümölcs nevet!");
                textBox_nev.Focus();
                return true;
            }
            if (numericUpDown_mennyiseg.Value > 1000)
            {
                MessageBox.Show("Érvénytelen mennyiség!!!");
                numericUpDown_mennyiseg.Value = 1000;
                numericUpDown_mennyiseg.Focus();
                return true;
            }
            if (string.IsNullOrEmpty(textBox_egysegar.Text))
            {
                MessageBox.Show("Nem adott meg egyséárat!!");
                textBox_egysegar.Focus();
                return true;
            }
            return false;
        }

        private void update_Click(object sender, EventArgs e)
        {
            if (listBox_gyum.SelectedIndex < 0)
            {
                return;
            }
            //--  A felhasználó kijeleöl egy elemet a listából
            gyumolcs kivalasztott_gyumolcs = (gyumolcs)listBox_gyum.SelectedItem;
            cmd.CommandText = "UPDATE `gyumolcs` SET `nev` = @nev, `egysegar` = @egysegar, `mennyiseg`= @mennyiseg WHERE `id` = @id";
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@id", textBox_id.Text.ToString());
            cmd.Parameters.AddWithValue("@nev", textBox_nev.Text);
            cmd.Parameters.AddWithValue("@egysegar", textBox_egysegar.Text);
            cmd.Parameters.AddWithValue("@mennyiseg", numericUpDown_mennyiseg.Value);
            conn.Open();

            if (cmd.ExecuteNonQuery() == 1)
            {

                MessageBox.Show("Módosítás sikeres volt!");
                conn.Close();
                textBox_id.Text = "";
                textBox_nev.Text = "";
                textBox_egysegar.Text = "";
                numericUpDown_mennyiseg.Text = "";
                gyumolcs_update();
            }
            else
            {

                MessageBox.Show("Az adatok módosítása sikertelen volt!");

            }
            if (conn.State == ConnectionState.Open)
            {

                conn.Close();

            }
        }

        private void delete_Click(object sender, EventArgs e)
        {
            if (listBox_gyum.SelectedIndex < 0)
            {
                return;
            }
            cmd.CommandText = "DELETE FROM `gyumolcs` WHERE `id` = @id;";
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@id", textBox_id.Text);
            conn.Open();

            if (cmd.ExecuteNonQuery() == 1)
            {
                MessageBox.Show("Törlés sikeres volt!", "Sikeres!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                conn.Close();
                textBox_id.Text = "";
                textBox_nev.Text = "";
                textBox_egysegar.Text = "";
                textBox_egysegar.Text = "";
                gyumolcs_update();
            }
            else
            {
                MessageBox.Show("Az adatok törlése sikertelen volt!");
            }
            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
        }

        private void listBox_gyum_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox_gyum.SelectedIndex < 0)
            {
                return;
            }
            // --A felhasználó kijelöl egy elemet a listában.
            gyumolcs kivalasztottgyumolcs = (gyumolcs)listBox_gyum.SelectedItem;
            textBox_id.Text = kivalasztottgyumolcs.Id.ToString();
            textBox_nev.Text = kivalasztottgyumolcs.Nev;
            textBox_egysegar.Text = kivalasztottgyumolcs.Egysegar.ToString();
            numericUpDown_mennyiseg.Text = kivalasztottgyumolcs.Mennyiseg.ToString();
        }

        
    }
}
