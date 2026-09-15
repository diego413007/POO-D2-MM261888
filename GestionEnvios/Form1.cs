using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GestionEnvios
{
    public partial class Form1 : Form
    {
        // Colección de tipo Envio para aplicar Polimorfismo
        private List<Envio> listaEnvios = new List<Envio>();
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (File.Exists("datos_envios.json"))
            {
                if (MessageBox.Show("¿Cargar datos de envíos guardados previamente?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    try
                    {
                        // Leer el archivo JSON
                        string jsonString = File.ReadAllText("datos_envios.json");
                        // Deserializar a la lista polimórfica
                        listaEnvios = JsonSerializer.Deserialize<List<Envio>>(jsonString) ?? new List<Envio>(); // "??" para manejar el caso de null
                        // Actualizar el datagridview con los datos cargados
                        ActualizarGrid();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error al cargar los datos: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                btnCalcular.Enabled = false;
                txtCliente.Enabled = false;
                txtPeso.Enabled = false;
                cmbEnvio.Enabled = false;

            }
        }  

        private void btnCalcular_Click(object sender, EventArgs e)
        {
            // Validaciones de campos vacíos, tipo numérico y código único
            if (!ValidarDatos())
            {
                return;
            }

            Envio envio = null;

            try
            {
                string tipo = string.Empty;
                if (cmbEnvio.SelectedIndex != -1)
                {
                    tipo = cmbEnvio.SelectedItem.ToString();
                }

                // Creación del objeto según la opción seleccionada
                if (tipo == "Nacional")
                {
                    envio = new EnvioNacional(codigo: int.Parse(txtCodigo.Text), cliente: txtCliente.Text, peso: double.Parse(txtPeso.Text));
                }
                else if (tipo == "Express")
                {
                    envio = new EnvioExpress(codigo: int.Parse(txtCodigo.Text), cliente: txtCliente.Text, peso: double.Parse(txtPeso.Text));
                }
                else if (tipo == "Internacional")
                {
                    if (!double.TryParse(txtArancel.Text, out double arancel) || arancel < 0)
                    {
                        MessageBox.Show("Ingrese un valor de arancel válido.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    envio = new EnvioInternacional(codigo: int.Parse(txtCodigo.Text), cliente: txtCliente.Text, peso: double.Parse(txtPeso.Text), arancel);
                }
                else if (string.IsNullOrWhiteSpace(tipo))
                {
                    MessageBox.Show("Seleccione un tipo de envío.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Guardar en la lista polimórfica
                listaEnvios.Add(envio);

                // Actualizar interfaz
                lblCalculo.Text = $"Último costo: ${envio.CalcularCosto():F2}";
                ActualizarGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al crear el envío: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            lblCalculo.Text = "Último costo:";
            txtArancel.Clear();
            txtCliente.Clear();
            txtCodigo.Clear();
            txtPeso.Clear();
            cmbEnvio.SelectedIndex = -1;
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        //Eventos para habilitar/deshabilitar controles según el estado de los campos (forzar orden de ingreso)
        private void txtCodigo_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtCodigo.Text))
            {
                txtCliente.Enabled = true;
                if (!string.IsNullOrWhiteSpace(txtCliente.Text))
                    txtPeso.Enabled = true;
                else
                    txtPeso.Enabled = false;
            }
            else
            {
                txtCliente.Enabled = false;
                txtPeso.Enabled = false;
                cmbEnvio.Enabled = false;
                btnCalcular.Enabled = false;
                txtArancel.Enabled = false;
                lblArancel.Enabled = false;
            }
            if (!string.IsNullOrWhiteSpace(txtCodigo.Text) && !string.IsNullOrWhiteSpace(txtCliente.Text) && !string.IsNullOrWhiteSpace(txtPeso.Text))
            {
                cmbEnvio.Enabled = true;
                txtArancel.Enabled = true;
                lblArancel.Enabled = true;
                if (cmbEnvio.SelectedItem != null)
                    btnCalcular.Enabled = true;
            }
        }

        private void txtCliente_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtCliente.Text))
                txtPeso.Enabled = true;
            else
            {
                txtPeso.Enabled = false;
                cmbEnvio.Enabled = false;
                btnCalcular.Enabled = false;
                txtArancel.Enabled = false;
                lblArancel.Enabled = false;
            }
            if (!string.IsNullOrWhiteSpace(txtCodigo.Text) && !string.IsNullOrWhiteSpace(txtCliente.Text) && !string.IsNullOrWhiteSpace(txtPeso.Text))
            {
                cmbEnvio.Enabled = true;
                txtArancel.Enabled = true;
                lblArancel.Enabled = true;
                if (cmbEnvio.SelectedItem != null)
                    btnCalcular.Enabled = true;
            }
        }

        private void txtPeso_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtCodigo.Text) && !string.IsNullOrWhiteSpace(txtCliente.Text) && !string.IsNullOrWhiteSpace(txtPeso.Text))
            {
                cmbEnvio.Enabled = true;
                txtArancel.Enabled = true;
                lblArancel.Enabled = true;
                if (cmbEnvio.SelectedItem != null)
                    btnCalcular.Enabled = true;
            }

            else
            {
                cmbEnvio.Enabled = false;
                btnCalcular.Enabled = false;
                txtArancel.Enabled = false;
                lblArancel.Enabled = false;
            }
        }
        private void cmbEnvio_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(cmbEnvio.SelectedIndex != -1)
            {
                btnCalcular.Enabled = true;
                if (cmbEnvio.SelectedItem.ToString() == "Internacional")
                {
                    txtArancel.Visible = true;
                    lblArancel.Visible = true;
                }
                else
                {
                    txtArancel.Visible = false;
                    lblArancel.Visible = false;
                    txtArancel.Clear();
                }
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (listaEnvios.Count != 0)
            {
                DialogResult result = MessageBox.Show("Desea guardar los datos antes de salir?", "Confirmación", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    // Serializar a texto JSON
                    string jsonString = JsonSerializer.Serialize(listaEnvios);

                    // Guardar en disco
                    File.WriteAllText("datos_envios.json", jsonString);
                }
                else if (result == DialogResult.Cancel)
                {
                    e.Cancel = true; // Cancelar el cierre del formulario
                }
            }
        }

        //Métodos de apoyo
        private void ActualizarGrid()
        {
            dgvHistorial.Rows.Clear();
            // Estructura de las columnas
            if (dgvHistorial.Columns.Count == 0)
            {
                dgvHistorial.Columns.Add("colCodigo", "Código");
                dgvHistorial.Columns.Add("colCliente", "Cliente");
                dgvHistorial.Columns.Add("colTipo", "Tipo");
                dgvHistorial.Columns.Add("colPeso", "Peso (kg)");
                dgvHistorial.Columns.Add("colCosto", "Costo Total");
            }

            // Recorrido polimórfico mediante la referencia de la clase base Envio
            foreach (Envio envio in listaEnvios)
            {
                // Se ejecuta automáticamente la versión adecuada de CalcularCosto()
                dgvHistorial.Rows.Add(
                    envio.Codigo,
                    envio.Cliente,
                    envio.GetType().Name.Replace("Envio", ""),
                    envio.Peso,
                    $"${envio.CalcularCosto():F2}"
                );
            }
        }
        public bool ValidarDatos()
        {
            //Validaciones de campos vacíos y tipo numérico
            if (string.IsNullOrWhiteSpace(txtCodigo.Text) || string.IsNullOrWhiteSpace(txtCliente.Text) || string.IsNullOrWhiteSpace(txtPeso.Text))
            {
                MessageBox.Show("Por favor, complete todos los campos obligatorios.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (!double.TryParse(txtPeso.Text, out double peso) || peso <= 0)
            {
                MessageBox.Show("Ingrese un valor de peso válido.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            //Validación de código único
            foreach (Envio envio in listaEnvios)
            {
                if (Equals(envio.Codigo, int.Parse(txtCodigo.Text)))
                {
                    MessageBox.Show("El código de envío ya existe. Ingrese un código único.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }
            return true;
        }


    }
}

/*Se utilizó herencia en las clases de los distintos tipos de envío (Express, Nacional e Internacional)
para compartir propiedades y métodos comunes, mientras que cada clase derivada implementa su propia 
lógica específica para calcular el costo de envío.*/
/*Se aplicó polimorfismo al crear una lista de tipo Envio, que permite almacenar objetos de las clases derivadas 
(EnvioNacional, EnvioExpress y EnvioInternacional) y llamar al método CalcularCosto() de manera uniforme*/
