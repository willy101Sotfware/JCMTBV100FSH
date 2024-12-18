using System.IO.Ports;

namespace JCMTBV100FSH
{
    public partial class MainForm : Form
    {
        private JcmBillValidator? _validator;
        private bool _isConnected = false;

        public MainForm()
        {
            InitializeComponent();
            LoadComPorts();
        }

        private void LoadComPorts()
        {
            cmbPorts.Items.AddRange(SerialPort.GetPortNames());
            if (cmbPorts.Items.Count > 0)
                cmbPorts.SelectedIndex = 0;
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (!_isConnected)
            {
                if (cmbPorts.SelectedItem == null)
                {
                    MessageBox.Show("Por favor seleccione un puerto COM", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                _validator = new JcmBillValidator(cmbPorts.SelectedItem.ToString()!);
                _validator.OnStatusChanged += Validator_OnStatusChanged;
                _validator.OnBillAccepted += Validator_OnBillAccepted;
                _validator.OnError += Validator_OnError;

                if (_validator.Connect())
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
            AddLog("Desconectado");
        }

        private void EnableControls(bool enabled)
        {
            btnReset.Enabled = enabled;
            btnEnable.Enabled = enabled;
            btnDisable.Enabled = enabled;
            btnStack.Enabled = enabled;
            btnReturn.Enabled = enabled;
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

        private void AddLog(string message)
        {
            txtLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}{Environment.NewLine}");
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            DisconnectValidator();
            base.OnFormClosing(e);
        }
    }
}
