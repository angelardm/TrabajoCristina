using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using Trabajo2;

namespace Trabajo2
{
    internal class Conexion
    {
        MySqlConnection conexion;
        string strCon = "server=localhost;database=animales;uid=root;password=";



        /**
         * Método para CONECTAR con la Base de Datos, RECUPERAR los animales de la misma y MOSTRARLOS en un tabla del tipo DataGridView
         */
        public List<Animal> GetAnimales()
        {
            List<Animal> listaAnimales = new List<Animal>();

            try
            {

                GetCon(); // Abrimos conexion


                string consulta = "SELECT * FROM animales";
                using (MySqlCommand command = new MySqlCommand(consulta, conexion))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Animal animal = new Animal();


                            animal.Nombre = reader.GetString("nombre");
                            animal.Especie = reader.GetString("especie");
                            animal.Familia = reader.GetString("familia");
                            animal.Genero = reader.GetString("genero");
                            animal.Precio = reader.GetDecimal("precio");

                            listaAnimales.Add(animal);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }

            return listaAnimales;
        }



        public MySqlConnection GetCon() // Método para abrir y recuperar la conexión
        {
            conexion = new MySqlConnection();
            conexion.Open();
            return conexion;
        }

        public void CloseCon() // Método para cerrar la conexión
        {
            if (conexion != null && conexion.State == ConnectionState.Open)
            {
                conexion.Close();
            }
        }


        /**
        * Método para ACTUALIZAR
        */
        public void ActualizarArticuloEnBD(Animal animal)
        {
            try
            {
                // Abrir la conexión
                GetCon();

                string consulta = "UPDATE animal SET familia = @familia, especie = @especie, genero = @genero, precio = @precio WHERE nombre = @nombre";

                using (MySqlCommand command = new MySqlCommand(consulta, GetCon()))
                {
                    // Asignar valores a los parámetros
                    command.Parameters.AddWithValue("@familia", animal.Familia.ToString());
                    command.Parameters.AddWithValue("@especie", animal.Especie.ToString());
                    command.Parameters.AddWithValue("@genero", animal.Genero);
                    command.Parameters.AddWithValue("@nombre", animal.Nombre);
                    command.Parameters.AddWithValue("@precio", animal.Precio);


                    // Ejecutar la consulta
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                // Cerrar la conexión
                CloseCon();
            }
        }



        /**
         * 
         */
        public void InsertarArticuloEnBD(Animal animal)
        {
            try
            {
                // Abrir la conexión
                GetCon();


                string consulta = "INSERT INTO animales (nombre, familia, especie, genero, precio) VALUES (@nombre,@familia, @especie, @genero, @precio)";

                using (MySqlCommand command = new MySqlCommand(consulta, GetCon()))
                {

                    command.Parameters.AddWithValue("@familia", animal.Familia.ToString());
                    command.Parameters.AddWithValue("@especie", animal.Especie.ToString());
                    command.Parameters.AddWithValue("@genero", animal.Genero);
                    command.Parameters.AddWithValue("@nombre", animal.Nombre);
                    command.Parameters.AddWithValue("@precio", animal.Precio);


                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al insertar el artículo: " + ex.Message);
            }
            finally
            {
                // Cerrar la conexión
                CloseCon();
            }
        }

    }
}