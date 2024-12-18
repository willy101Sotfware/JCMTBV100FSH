using System;
using System.IO.Ports;
using System.Threading.Tasks;

namespace JCMTBV100FSH
{
    public class JcmBillValidator : IDisposable
    {
        private SerialPort _serialPort;
        private bool _isDisposed;
        private bool _isInitialized;

        public event EventHandler<string>? OnStatusChanged;
        public event EventHandler<decimal>? OnBillAccepted;
        public event EventHandler<string>? OnError;

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

        public async Task<bool> Connect()
        {
            try
            {
                if (!_serialPort.IsOpen)
                {
                    _serialPort.Open();
                    _serialPort.DataReceived += SerialPort_DataReceived;
                }

                // Secuencia de inicialización
                if (!_isInitialized)
                {
                    // 1. Reset el dispositivo
                    if (!await SendCommand(JcmCommands.BuildCommand(JcmCommands.RESET)))
                    {
                        OnError?.Invoke(this, "Error en Reset inicial");
                        return false;
                    }
                    await Task.Delay(2000); // Esperar a que el reset se complete

                    // 2. Configurar nivel de seguridad (medio)
                    if (!await SendCommand(JcmCommands.SetSecurityLevel(1)))
                    {
                        OnError?.Invoke(this, "Error configurando nivel de seguridad");
                        return false;
                    }

                    // 3. Configurar dirección de inserción
                    if (!await SendCommand(JcmCommands.SetDirection(true, true)))
                    {
                        OnError?.Invoke(this, "Error configurando dirección");
                        return false;
                    }

                    // 4. Habilitar todos los billetes
                    if (!await SendCommand(JcmCommands.EnableAllBills()))
                    {
                        OnError?.Invoke(this, "Error habilitando billetes");
                        return false;
                    }

                    _isInitialized = true;
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
            _isInitialized = false;
        }

        public async Task<bool> Reset()
        {
            _isInitialized = false;
            return await Connect(); // Esto ejecutará toda la secuencia de inicialización
        }

        public async Task<bool> Enable()
        {
            if (!_isInitialized)
            {
                if (!await Connect())
                {
                    return false;
                }
            }
            return await SendCommand(JcmCommands.BuildCommand(JcmCommands.ENABLE));
        }

        public async Task<bool> Disable()
        {
            return await SendCommand(JcmCommands.BuildCommand(JcmCommands.DISABLE));
        }

        public async Task<bool> Stack()
        {
            return await SendCommand(JcmCommands.BuildCommand(JcmCommands.STACK));
        }

        public async Task<bool> Return()
        {
            return await SendCommand(JcmCommands.BuildCommand(JcmCommands.RETURN));
        }

        private async Task<bool> SendCommand(byte[] command)
        {
            try
            {
                if (!_serialPort.IsOpen)
                {
                    throw new InvalidOperationException("Puerto serial no está abierto");
                }

                await Task.Run(() => _serialPort.Write(command, 0, command.Length));

                // Esperar respuesta
                byte[] response = new byte[5];
                await Task.Run(() => _serialPort.Read(response, 0, 5));

                // Verificar ACK
                return response[2] == JcmCommands.ACK;
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
            if (response.Length >= 5 && response[0] == JcmCommands.STX)
            {
                byte command = response[2];
                switch (command)
                {
                    case 0x71: // Billete insertado y en posición
                        OnStatusChanged?.Invoke(this, "Billete en posición");
                        break;
                    case 0x72: // Billete aceptado
                        byte channel = response[3];
                        decimal value = GetBillValue(channel);
                        OnBillAccepted?.Invoke(this, value);
                        break;
                    case 0x73: // Billete rechazado
                        OnStatusChanged?.Invoke(this, "Billete rechazado");
                        break;
                    case 0x31: // Status
                        ProcessStatusResponse(response);
                        break;
                }
            }
        }

        private void ProcessStatusResponse(byte[] response)
        {
            if (response.Length < 6) return;

            byte status = response[3];
            string statusMessage = "Estado: ";

            if ((status & 0x01) != 0) statusMessage += "Ocupado, ";
            if ((status & 0x02) != 0) statusMessage += "Billete en validador, ";
            if ((status & 0x04) != 0) statusMessage += "Error de checksum, ";
            if ((status & 0x08) != 0) statusMessage += "Validador lleno, ";
            if ((status & 0x10) != 0) statusMessage += "Billete atascado, ";
            if ((status & 0x20) != 0) statusMessage += "Error de cassette, ";
            if ((status & 0x40) != 0) statusMessage += "Error de validador, ";
            if ((status & 0x80) != 0) statusMessage += "Error de comunicación, ";

            OnStatusChanged?.Invoke(this, statusMessage.TrimEnd(' ', ','));
        }

        private decimal GetBillValue(byte channel)
        {
            // Esto debería configurarse según los valores reales de los billetes
            switch (channel)
            {
                case 1: return 1000.00M; // $1000
                case 2: return 2000.00M; // $2000
                case 3: return 5000.00M; // $5000
                case 4: return 10000.00M; // $10000
                case 5: return 20000.00M; // $20000
                case 6: return 50000.00M; // $50000
                case 7: return 100000.00M; // $100000
                default: return 0.00M;
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
