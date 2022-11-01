using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _3DPrinterController.UserControls
{
    public partial class MainControl : UserControl
    {
        SerialPort serialPort = new SerialPort();
        public MainControl()
        {
            InitializeComponent();
            
            tableLayoutPanel1.Dock = DockStyle.Fill;
        }

        private void MainControl_Load(object sender, EventArgs e)
        {
            // Add all coms to listbox
            string[] ports = System.IO.Ports.SerialPort.GetPortNames();
            listBoxPorts.Items.AddRange(ports);


        }

        private void btnOpenPort_Click(object sender, EventArgs e)
        {
            try
            {
                serialPort = new SerialPort(listBoxPorts.SelectedItem.ToString(), 115200, Parity.None, 8, StopBits.One);
                //serialPort.PortName = listBoxPorts.Text;
                serialPort.Open();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // Open serialport

            //serialPort.Open();


        }

        private void btnMove_Click(object sender, EventArgs e)
        {
            if (serialPort.IsOpen)
            {
                serialPort.WriteLine("G1 X" + txtX.Text + " Y" + txtY.Text + " Z" + txtZ.Text);
            }
        }

        private void btnHome_Click(object sender, EventArgs e)
        {
            if (serialPort.IsOpen)
            {
                serialPort.WriteLine("G28");
            }
        }
    }
}
