using MWS.Models;
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
using ZXing.QrCode.Internal;

namespace MWS.Views
{
    public partial class ProductionDetailAddItemView : Form
    {
        ProductionDetailView productionDetailView;
        TrnProductionModel trnProductionModel;

        private SerialPort serialPort;
        private string diagnosticBuffer = "";
        public ProductionDetailAddItemView(ProductionDetailView _productionDetailView, TrnProductionModel _trnProductionModel)
        {
            InitializeComponent();

            productionDetailView = _productionDetailView;
            trnProductionModel = _trnProductionModel;

            GetItemList();
        }
        public void GetItemList()
        {
            Controllers.TrnProductionItemController trnProductionItemController = new Controllers.TrnProductionItemController();
            if (trnProductionItemController.DropDownItem().Any())
            {
                comboBoxItem.DataSource = trnProductionItemController.DropDownItem();
                comboBoxItem.ValueMember = "Id";
                comboBoxItem.DisplayMember = "Item";
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            DisconnectPort();
            productionDetailView.ConnectToPort1("COM1");
            Close();
            productionDetailView.textBoxWeight.Text = "";
            productionDetailView.textBoxWeight.Focus();
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            DialogResult saveDialogResult = MessageBox.Show("Confirm?", "MWS", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (saveDialogResult == DialogResult.Yes)
            {
                Controllers.TrnProductionItemController trnProductionItemController = new Controllers.TrnProductionItemController();
                String[] addItem = trnProductionItemController.AddProductionItemOthers(trnProductionModel.Id, Convert.ToInt32(comboBoxItem.SelectedValue), Convert.ToDecimal(textBoxWeight.Text));
                if (addItem[1].Equals("0") == false)
                {
                    DisconnectPort();
                    productionDetailView.ConnectToPort1("COM1");
                    Close();
                    productionDetailView.UpdateProductionItemListDataSource();
                    productionDetailView.textBoxWeight.Text = "";
                    productionDetailView.textBoxWeight.Focus();
                }
                else
                {
                    MessageBox.Show(addItem[0], "MWS", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private bool ConnectToPort2(string portName)
        {
            try
            {
                DisconnectPort();

                serialPort = new SerialPort(portName);
                serialPort.BaudRate = 19200;
                serialPort.Parity = Parity.None;
                serialPort.DataBits = 8;
                serialPort.StopBits = StopBits.One;
                serialPort.ReadTimeout = 1500;
                serialPort.Handshake = Handshake.None;

                serialPort.RtsEnable = true;
                serialPort.DtrEnable = true;

                serialPort.DataReceived += DataReceivedHandler;
                serialPort.Open();

                return true;
            }
            catch (Exception ex)
            {
                //MessageBox.Show($"Error: {ex.Message}");
                return false;
            }
        }
        private void DisconnectPort()
        {
            try
            {
                if (serialPort != null)
                {
                    serialPort.DataReceived -= DataReceivedHandler;

                    if (serialPort.IsOpen)
                    {
                        serialPort.DiscardInBuffer();
                        serialPort.DiscardOutBuffer();
                        serialPort.Close();
                    }
                    serialPort.Dispose();
                    serialPort = null;
                }
            }
            catch { }
        }
        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (sender as SerialPort);
            try
            {
                if (sp != null && sp.IsOpen)
                {
                    string incoming = sp.ReadExisting();
                    diagnosticBuffer += incoming;

                    while (diagnosticBuffer.Contains("\r"))
                    {
                        int index = diagnosticBuffer.IndexOf("\r");
                        string completeLine = diagnosticBuffer.Substring(0, index).Trim();
                        diagnosticBuffer = diagnosticBuffer.Substring(index + 1);

                        if (!string.IsNullOrWhiteSpace(completeLine))
                        {
                            if (completeLine.StartsWith("T", StringComparison.OrdinalIgnoreCase))
                            {
                                continue;
                            }

                            string digitsOnly = "";
                            foreach (char c in completeLine)
                            {
                                if (char.IsDigit(c)) digitsOnly += c;
                            }

                            if (!string.IsNullOrEmpty(digitsOnly) && double.TryParse(digitsOnly, out double rawNumber))
                            {
                                double finalWeight = rawNumber / 1000.0;

                                this.BeginInvoke(new MethodInvoker(delegate {
                                    textBoxWeight.Text = finalWeight.ToString("0.000");
                                }));
                            }
                            else
                            {
                                this.BeginInvoke(new MethodInvoker(delegate {
                                    textBoxWeight.Text = $"RAW: {completeLine}";
                                }));
                            }
                        }
                    }
                }
            }
            catch { }
        }
        private void ProductionDetailAddItemView_Load(object sender, EventArgs e)
        {
            ConnectToPort2("COM1");
        }
    }
}
