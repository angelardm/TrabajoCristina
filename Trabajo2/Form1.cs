using MySql.Data.MySqlClient;
using System.Data;
using System.Text;
using System.Windows.Forms;
using OxyPlot.Series;
using OxyPlot.WindowsForms;
using OxyPlot;
using System.Diagnostics;
using Org.BouncyCastle.Tls;
using System.Collections.Generic;
using System;
using System.Data.SqlClient;
using System.Linq;
using Trabajo2;

namespace Trabajo2
{
    public partial class Form1 : Form
    {

        private decimal? PrecioTotal { get; set; } = decimal.Zero;

        Conexion connection;

        internal List<Animal> ListaAnimalesBD { get; private set; }

        Animal animalSeleccionado;


        public Form1()
        {
            InitializeComponent();
            CargarDatos();
        }


        private void CargarDatos()
        {

            connection = new Conexion();
            this.ListaAnimalesBD = connection.GetAnimales();

            if (ListaAnimalesBD.Count > 0)
            {
                dgv_listaAnimal.DataSource = ListaAnimalesBD;



            }
            else
            {
                MessageBox.Show("No se pudieron cargar datos desde la base de datos.");
            }
            connection.CloseCon();
        }


        private void buttonBuscar_Click(object sender, EventArgs e)
        {
            string familia = comboBoxFamilia.Text.Trim();

            string genero = textBoxGenero.Text.Trim();

            string nombre = textBoxNombre.Text.Trim();

            string especie = comboBoxEspecie.Text.Trim();


            List<Animal> resultados = ListaAnimalesBD.Where(animal =>
            (string.IsNullOrEmpty(nombre) || animal.Nombre.ToUpper().Contains(nombre.ToUpper())) &&
            (string.IsNullOrEmpty(familia) || familia == animal.Familia.ToString()) &&
            (string.IsNullOrEmpty(genero) || genero == animal.Genero.ToString()) &&
            (string.IsNullOrEmpty(especie) || especie == animal.Especie.ToString())).ToList();

            if (resultados.Count > 0)
            {
                dgv_listaAnimal.DataSource = resultados;
            }
            else
            {
                MessageBox.Show("No se encontraron resultados para la búsqueda.");
            }
        }


        private void buttonEliminar_Click(object sender, EventArgs e)
        {
            // Preguntar al usuario si realmente desea eliminar el artículo
            DialogResult result = MessageBox.Show($"¿Estás seguro de que deseas eliminar el artículo '{animalSeleccionado.Nombre}'?", "Confirmar Eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                connection.GetCon();
                string nombre = animalSeleccionado.Nombre;
                string consulta = $"DELETE FROM animales WHERE nombre = {nombre}";

                using (MySqlCommand command = new MySqlCommand(consulta, connection.GetCon()))
                {
                    try { 

                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }

                CargarDatos();

                connection.CloseCon();


            }
        }


        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {

            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {

                this.animalSeleccionado = (Animal)dgv_listaAnimal.Rows[e.RowIndex].DataBoundItem;
            }
        }


        /**
        * Método para MODIFICAR un artículo seleccionado previamente de la lista de artículos del DataGridView.
        */
        private void buttonModificar_Click(object sender, EventArgs e)
        {
            // Verificar si hay un animal seleccionado
            if (animalSeleccionado == null)
            {
                MessageBox.Show("Selecciona un artículo para modificar.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Obtener los nuevos valores de los componentes del formulario (si no se introducen nuevos valores, se mantienen los antiguos)
            string especie = comboBoxEspecie.SelectedItem?.ToString() ?? string.Empty;
            string familia = comboBoxFamilia.SelectedItem?.ToString() ?? string.Empty;
            string nombre = string.IsNullOrEmpty(textBoxNombre.Text) ? animalSeleccionado.Nombre : textBoxNombre.Text.Trim();
            decimal precio = string.IsNullOrEmpty(textBoxPrecio.Text) ? animalSeleccionado.Precio : Convert.ToDecimal(textBoxPrecio.Text);
            string genero = string.IsNullOrEmpty(textBoxGenero.Text) ? animalSeleccionado.Genero : textBoxGenero.Text.Trim();

            // Crear un nuevo objeto Articulo con los valores actualizados
            Animal animalActualizado = new Animal
            {
                Especie = especie,
                Genero = genero,
                Familia = familia,
                Nombre = nombre,
                Precio = precio
            };

            // Llamar al método para actualizar en la base de datos
            connection.ActualizarAnimalEnBD(animalActualizado);

            CargarDatos();

        }
        /**
         * Método para INSERTAR un artículo 
         */
        private void buttonInsertar_Click(object sender, EventArgs e)
        {
            // Recoger los valores del formulario en variables para el nuevo Articulo
            string especie = comboBoxEspecie.SelectedItem?.ToString() ?? string.Empty;

            string familia = comboBoxFamilia.SelectedItem?.ToString() ?? string.Empty;

            string nombre = textBoxNombre.Text.Trim();

            string genero = textBoxGenero.Text.Trim();

            decimal precio = Convert.ToDecimal(textBoxPrecio.Text);

            Animal animalInsertado = new Animal
            {

                Especie = especie,
                Familia = familia,
                Genero = genero,
                Nombre = nombre,
                Precio = precio
            };

            // Llamar al método para actualizar en la base de datos
            connection.InsertarArticuloEnBD(animalInsertado);

            // Actualizar la lista de artículos
            CargarDatos();

        }

        /**
         * Método para AGREGAR y SUMAR el valor total de los artículos en el carrito de la compra
         */
        private void buttonCompra_Click(object sender, EventArgs e)
        {
            // Verificar si hay un artículo seleccionado
            if (animalSeleccionado == null)
            {
                MessageBox.Show("Selecciona un animal para comprar.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Incrementar el total de precios
            PrecioTotal += animalSeleccionado.Precio;
            

            // Mostrar el precio total en el TextBox
            textBoxTotal.Text = PrecioTotal?.ToString("C"); // Muestra el valor como moneda

        }
    }


}