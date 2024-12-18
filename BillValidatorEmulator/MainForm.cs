using System;
using System.Drawing;
using System.IO.Ports;
using System.Windows.Forms;

namespace BillValidatorEmulator
{
    public partial class MainForm : Form
    {
        private BillValidatorDevice? _validator;
        private bool _isConnected;

        public MainForm()
        {
            InitializeComponent();
            LoadComPorts();
            InitializeControls();
        }

        private void InitializeControls()
        {
            // Panel de LEDs
            pnlPowerLed.BackColor = Color.Gray;
            pnlErrorLed.BackColor = Color.Gray;
            pnlReadyLed.BackColor = Color.Gray;
            pnlProcessingLed.BackColor = Color.Gray;

            // Panel de Sensores
            pnlEntrySensor.BackColor = Color.Green;
            pnlValidatingSensor.BackColor = Color.Green;
            pnlStackingSensor.BackColor = Color.Green;
            pnlReturnSensor.BackColor = Color.Green;
        }

        private void LoadComPorts()
        {
            cmbPorts.Items.AddRange(SerialPort.GetPortNames());
            if (cmbPorts.Items.Count > 0)
                cmbPorts.SelectedIndex = 0;
        }

        private async void btnConnect_Click(object sender, EventArgs e)
        {
            if (!_isConnected)
            {
                if (cmbPorts.SelectedItem == null)
                {
                    MessageBox.Show("Por favor seleccione un puerto COM", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                _validator = new BillValidatorDevice(cmbPorts.SelectedItem.ToString()!);
                _validator.OnStatusChanged += Validator_OnStatusChanged;
                _validator.OnBillAccepted += Validator_OnBillAccepted;
                _validator.OnError += Validator_OnError;
                _validator.OnSensorStatusChanged += Validator_OnSensorStatusChanged;
                _validator.OnLedStatusChanged += Validator_OnLedStatusChanged;

                if (await _validator.Connect())
                {
                    _isConnected = true;
                    btnConnect.Text = "Desconectar";
                    EnableControls(true);
                    AddLog("Conectado exitosamente");
                }
            }
            else
            {
                DisconnectValidator();
            }
        }

        private void DisconnectValidator()
        {
            if (_validator != null)
            {
                _validator.Disconnect();
                _validator.Dispose();
                _validator = null;
            }
            _isConnected = false;
            btnConnect.Text = "Conectar";
            EnableControls(false);
            InitializeControls();
            AddLog("Desconectado");
        }

        private async void btnReset_Click(object sender, EventArgs e)
        {
            if (_validator != null)
            {
                if (await _validator.Reset())
                    AddLog("Reset enviado");
            }
        }

        private async void btnEnable_Click(object sender, EventArgs e)
        {
            if (_validator != null)
            {
                if (await _validator.Enable())
                    AddLog("Validador habilitado");
            }
        }

        private async void btnDisable_Click(object sender, EventArgs e)
        {
            if (_validator != null)
            {
                if (await _validator.Disable())
                    AddLog("Validador deshabilitado");
            }
        }

        private async void btnStack_Click(object sender, EventArgs e)
        {
            if (_validator != null)
            {
                if (await _validator.Stack())
                    AddLog("Comando Stack enviado");
            }
        }

        private async void btnReturn_Click(object sender, EventArgs e)
        {
            if (_validator != null)
            {
                if (await _validator.Return())
                    AddLog("Comando Return enviado");
            }
        }

        private void Validator_OnStatusChanged(object? sender, string status)
        {
            Invoke(() => AddLog($"Estado: {status}"));
        }

        private void Validator_OnBillAccepted(object? sender, decimal value)
        {
            Invoke(() => AddLog($"Billete aceptado: ${value}"));
        }

        private void Validator_OnError(object? sender, string error)
        {
            Invoke(() => AddLog($"Error: {error}"));
        }

        private void Validator_OnSensorStatusChanged(object? sender, SensorStatus status)
        {
            Invoke(() =>
            {
                pnlEntrySensor.BackColor = status.EntrySensorBlocked ? Color.Red : Color.Green;
                pnlValidatingSensor.BackColor = status.ValidatingSensorBlocked ? Color.Red : Color.Green;
                pnlStackingSensor.BackColor = status.StackingSensorBlocked ? Color.Red : Color.Green;
                pnlReturnSensor.BackColor = status.ReturnSensorBlocked ? Color.Red : Color.Green;
            });
        }

        private void Validator_OnLedStatusChanged(object? sender, LedStatus status)
        {
            Invoke(() =>
            {
                pnlPowerLed.BackColor = status.PowerLed ? Color.Blue : Color.Gray;
                pnlErrorLed.BackColor = status.ErrorLed ? Color.Red : Color.Gray;
                pnlReadyLed.BackColor = status.ReadyLed ? Color.Green : Color.Gray;
                pnlProcessingLed.BackColor = status.ProcessingLed ? Color.Yellow : Color.Gray;
            });
        }

        private void AddLog(string message)
        {
            txtLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}{Environment.NewLine}");
        }

        private void EnableControls(bool enabled)
        {
            btnReset.Enabled = enabled;
            btnEnable.Enabled = enabled;
            btnDisable.Enabled = enabled;
            btnStack.Enabled = enabled;
            btnReturn.Enabled = enabled;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            DisconnectValidator();
            base.OnFormClosing(e);
        }
    }
}
