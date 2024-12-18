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
                ReadTimeout = 2000,
                WriteTimeout = 2000
            };
        }

        public async Task<bool> Connect()
        {
            try
            {
                // Verificar si el puerto existe
                if (Array.IndexOf(SerialPort.GetPortNames(), _serialPort.PortName) == -1)
                {
                    OnError?.Invoke(this, $"El puerto {_serialPort.PortName} no está disponible.");
                    return false;
                }

                // Intentar cerrar el puerto si está abierto
                try
                {
                    if (_serialPort.IsOpen)
                    {
                        _serialPort.Close();
                        await Task.Delay(500); // Esperar a que se libere el puerto
                    }
                }
                catch
                {
                    // Ignorar errores al cerrar
                }

                // Intentar abrir el puerto con reintentos
                for (int i = 0; i < 3; i++)
                {
                    try
                    {
                        _serialPort.Open();
                        _serialPort.DataReceived += SerialPort_DataReceived;
                        break;
                    }
                    catch (UnauthorizedAccessException) when (i < 2)
                    {
                        await Task.Delay(1000); // Esperar antes de reintentar
                        continue;
                    }
                }

                if (!_serialPort.IsOpen)
                {
                    OnError?.Invoke(this, "No se pudo abrir el puerto después de varios intentos.");
                    return false;
                }

                if (!_isInitialized)
                {
                    // Secuencia de inicialización
                    if (!await Reset())
                    {
                        OnError?.Invoke(this, "Error durante la inicialización.");
                        return false;
                    }
                    _isInitialized = true;
                }

                OnStatusChanged?.Invoke(this, "Conectado correctamente.");
                return true;
            }
            catch (UnauthorizedAccessException)
            {
                OnError?.Invoke(this, "Acceso denegado al puerto. Asegúrese de que ninguna otra aplicación lo esté usando.");
            }
            catch (IOException ex)
            {
                OnError?.Invoke(this, $"Error de E/S: {ex.Message}");
            }
            catch (Exception ex)
            {
                OnError?.Invoke(this, $"Error al conectar: {ex.Message}");
            }
            return false;
        }

        public void Disconnect()
        {
            if (_serialPort?.IsOpen == true)
            {
                _serialPort.DataReceived -= SerialPort_DataReceived;
                _serialPort.Close();
            }
            _isInitialized = false;
            OnStatusChanged?.Invoke(this, "Desconectado.");
        }

        public async Task<bool> Reset()
        {
            try
            {
                _isInitialized = false;
                return await SendCommand(JcmCommands.BuildCommand(JcmCommands.RESET));
            }
            catch (Exception ex)
            {
                OnError?.Invoke(this, $"Error al realizar Reset: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> Enable()
        {
            try
            {
                // 1. Enviar comando de reset primero
                if (!await Reset())
                {
                    OnError?.Invoke(this, "Error en el reset inicial");
                    return false;
                }

                // 2. Esperar un momento para que el dispositivo se estabilice
                await Task.Delay(1000);

                // 3. Habilitar todos los canales de billetes
                if (!await SendCommand(JcmCommands.EnableAllBills()))
                {
                    OnError?.Invoke(this, "Error al habilitar canales de billetes");
                    return false;
                }

                // 4. Configurar la dirección del billete (face up y front first)
                if (!await SendCommand(JcmCommands.SetDirection(true, true)))
                {
                    OnError?.Invoke(this, "Error al configurar dirección del billete");
                    return false;
                }

                // 5. Finalmente enviar el comando enable
                if (!await SendCommand(JcmCommands.BuildCommand(JcmCommands.ENABLE)))
                {
                    OnError?.Invoke(this, "Error al habilitar el validador");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                OnError?.Invoke(this, $"Error en la secuencia de habilitación: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> Disable() => await SendCommand(JcmCommands.BuildCommand(JcmCommands.DISABLE));
        public async Task<bool> Stack() => await SendCommand(JcmCommands.BuildCommand(JcmCommands.STACK));
        public async Task<bool> Return() => await SendCommand(JcmCommands.BuildCommand(JcmCommands.RETURN));

        private async Task<bool> SendCommand(byte[] command)
        {
            try
            {
                if (!_serialPort.IsOpen)
                    throw new InvalidOperationException("El puerto serial no está abierto.");

                await Task.Run(() => _serialPort.Write(command, 0, command.Length));
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
            if (response.Length < 5 || response[0] != JcmCommands.STX)
            {
                OnError?.Invoke(this, "Respuesta inválida recibida.");
                return;
            }

            byte command = response[2];
            switch (command)
            {
                case 0x72: // Billete aceptado
                    decimal value = GetBillValue(response[3]);
                    OnBillAccepted?.Invoke(this, value);
                    break;
                default:
                    OnStatusChanged?.Invoke(this, $"Comando recibido: {command:X2}");
                    break;
            }
        }

        private decimal GetBillValue(byte channel)
        {
            return channel switch
            {
                1 => 1000.00M,
                2 => 2000.00M,
                3 => 5000.00M,
                4 => 10000.00M,
                5 => 20000.00M,
                6 => 50000.00M,
                7 => 100000.00M,
                _ => 0.00M,
            };
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
