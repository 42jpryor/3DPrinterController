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

        // Background worker for receiving data over serial port
        private BackgroundWorker serialPortReceiver = new BackgroundWorker();
        string receivedData = string.Empty;

        int printXMax = 350;
        int printYMax = 350;
        int printZMax = 400;


        public MainControl()
        {
            InitializeComponent();
            
            tableLayoutPanel1.Dock = DockStyle.Fill;

            btnClosePort.Enabled = false;
        }

        private void MainControl_Load(object sender, EventArgs e)
        {
            // Add all coms to listbox
            string[] ports = System.IO.Ports.SerialPort.GetPortNames();
            listBoxPorts.Items.AddRange(ports);

            txtX.Maximum = printXMax;
            txtY.Maximum = printYMax;
            txtZ.Maximum = printZMax;

            // Set up background worker
            serialPortReceiver.WorkerSupportsCancellation = true;
            serialPortReceiver.WorkerReportsProgress = true;
            serialPortReceiver.DoWork += new DoWorkEventHandler(serialPortReceiver_DoWork);

            // Start background worker
            serialPortReceiver.RunWorkerAsync();

        }

        private void serialPortReceiver_DoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                if (serialPortReceiver.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }
                else
                {
                    // Read data from serial port if open
                    if (serialPort.IsOpen)
                    {
                        try
                        {
                            string data = serialPort.ReadLine();

                            // X:180.00 Y:180.00 Z:12.85 E:0.00 Count X:14400 Y:14400 Z:10280
                            // If data starts with X: then it is a position report
                            if (data.StartsWith("X:"))
                            {
                                // Split data into array
                                string[] dataSplit = data.Split(' ');

                                // Get X position
                                string xPosition = dataSplit[0].Substring(2);

                                // Get Y position
                                string yPosition = dataSplit[1].Substring(2);

                                // Get Z position
                                string zPosition = dataSplit[2].Substring(2);

                                // Writeline each position
                                Console.WriteLine("X: " + xPosition);
                                Console.WriteLine("Y: " + yPosition);
                                Console.WriteLine("Z: " + zPosition);

                                // Update textboxes using Invoke
                                this.Invoke((MethodInvoker)delegate
                                {
                                    txtX.Text = xPosition;
                                    txtY.Text = yPosition;
                                    txtZ.Text = zPosition;
                                });
                            }

                            txtDataReceived.Invoke(new Action(() =>
                                txtDataReceived.AppendText(data + Environment.NewLine)
                            ));

                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }
                }
            }
        }

        private void btnOpenPort_Click(object sender, EventArgs e)
        {
            btnOpenPort.Enabled = false;
            btnClosePort.Enabled = true;

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
        }
        
        private void btnClosePort_Click(object sender, EventArgs e)
        {
            btnOpenPort.Enabled = true;
            btnClosePort.Enabled = false;

            try
            {
                serialPort.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void btnHome_Click(object sender, EventArgs e)
        {
            if (serialPort.IsOpen)
            {
                serialPort.WriteLine("G28");
            }
        }
        
        private void btnMove_Click(object sender, EventArgs e)
        {
            if (serialPort.IsOpen)
            {
                serialPort.WriteLine("G1 X" + txtX.Text + " Y" + txtY.Text + " Z" + txtZ.Text);
            }
        }


    }
}
