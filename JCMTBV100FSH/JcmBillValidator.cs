using System;
using System.IO.Ports;
using System.Threading.Tasks;

namespace JCMTBV100FSH
{
    public class JcmBillValidator : IDisposable
    {
        private SerialPort _serialPort;
        private bool _isDisposed;

        public event EventHandler<string> OnStatusChanged;
        public event EventHandler<decimal> OnBillAccepted;
        public event EventHandler<string> OnError;

        public JcmBillValidator(string portName, int baudRate = 9600)
        {
            _serialPort = new SerialPort
            {
                PortName = portName,
                BaudRate = baudRate,
                DataBits = 8,
                StopBits = StopBits.One,
                Parity = Parity.None,
                ReadTimeout = 1000,
                WriteTimeout = 1000
            };
        }

        public bool Connect()
        {
            try
            {
                if (!_serialPort.IsOpen)
                {
                    _serialPort.Open();
                    _serialPort.DataReceived += SerialPort_DataReceived;
                }
                return true;
            }
            catch (Exception ex)
            {
                OnError?.Invoke(this, $"Error al conectar: {ex.Message}");
                return false;
            }
        }

        public void Disconnect()
        {
            if (_serialPort?.IsOpen == true)
            {
                _serialPort.Close();
            }
        }

        public async Task<bool> Reset()
        {
            return await SendCommand(JcmCommands.RESET);
        }

        public async Task<bool> Enable()
        {
            return await SendCommand(JcmCommands.ENABLE);
        }

        public async Task<bool> Disable()
        {
            return await SendCommand(JcmCommands.DISABLE);
        }

        public async Task<bool> Stack()
        {
            return await SendCommand(JcmCommands.STACK);
        }

        public async Task<bool> Return()
        {
            return await SendCommand(JcmCommands.RETURN);
        }

        private async Task<bool> SendCommand(byte command)
        {
            try
            {
                if (!_serialPort.IsOpen)
                {
                    throw new InvalidOperationException("Puerto serial no está abierto");
                }

                byte[] cmd = JcmCommands.BuildCommand(command);
                await Task.Run(() => _serialPort.Write(cmd, 0, cmd.Length));
                return true;
            }
            catch (Exception ex)
            {
                OnError?.Invoke(this, $"Error al enviar comando: {ex.Message}");
                return false;
            }
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                if (_serialPort.BytesToRead < 5) return;

                byte[] buffer = new byte[_serialPort.BytesToRead];
                _serialPort.Read(buffer, 0, buffer.Length);

                ProcessResponse(buffer);
            }
            catch (Exception ex)
            {
                OnError?.Invoke(this, $"Error al recibir datos: {ex.Message}");
            }
        }

        private void ProcessResponse(byte[] response)
        {
            // Implementar el procesamiento de respuesta según la documentación del JBV-100-FSH
            // Este es un ejemplo básico
            if (response.Length >= 5 && response[0] == JcmCommands.STX)
            {
                byte command = response[2];
                switch (command)
                {
                    case 0x71: // Ejemplo: Billete aceptado
                        OnBillAccepted?.Invoke(this, 100.00M); // El valor dependerá del billete
                        break;
                    case 0x31: // Status
                        OnStatusChanged?.Invoke(this, "Status recibido");
                        break;
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    Disconnect();
                    _serialPort?.Dispose();
                }
                _isDisposed = true;
            }
        }
    }
}
