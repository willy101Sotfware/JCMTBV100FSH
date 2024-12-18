using System;
using System.IO.Ports;
using System.Threading.Tasks;

namespace BillValidatorEmulator
{
    public class BillValidatorDevice : IDisposable
    {
        private SerialPort _serialPort;
        private bool _isDisposed;
        private bool _isInitialized;
        private DeviceStatus _currentStatus;

        public event EventHandler<string>? OnStatusChanged;
        public event EventHandler<decimal>? OnBillAccepted;
        public event EventHandler<string>? OnError;
        public event EventHandler<SensorStatus>? OnSensorStatusChanged;
        public event EventHandler<LedStatus>? OnLedStatusChanged;

        public BillValidatorDevice(string portName, int baudRate = 9600)
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
            _currentStatus = DeviceStatus.Disconnected;
        }

        public async Task<bool> Connect()
        {
            try
            {
                if (Array.IndexOf(SerialPort.GetPortNames(), _serialPort.PortName) == -1)
                {
                    OnError?.Invoke(this, $"El puerto {_serialPort.PortName} no está disponible.");
                    return false;
                }

                try
                {
                    if (_serialPort.IsOpen)
                    {
                        _serialPort.Close();
                        await Task.Delay(500);
                    }
                }
                catch
                {
                    // Ignorar errores al cerrar
                }

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
                        await Task.Delay(1000);
                        continue;
                    }
                }

                if (!_serialPort.IsOpen)
                {
                    OnError?.Invoke(this, "No se pudo abrir el puerto después de varios intentos.");
                    UpdateStatus(DeviceStatus.Error);
                    return false;
                }

                if (!_isInitialized)
                {
                    if (!await Reset())
                    {
                        OnError?.Invoke(this, "Error durante la inicialización.");
                        UpdateStatus(DeviceStatus.Error);
                        return false;
                    }
                    _isInitialized = true;
                }

                UpdateStatus(DeviceStatus.Connected);
                OnStatusChanged?.Invoke(this, "Conectado correctamente.");
                return true;
            }
            catch (Exception ex)
            {
                OnError?.Invoke(this, $"Error al conectar: {ex.Message}");
                UpdateStatus(DeviceStatus.Error);
                return false;
            }
        }

        public void Disconnect()
        {
            if (_serialPort?.IsOpen == true)
            {
                _serialPort.DataReceived -= SerialPort_DataReceived;
                _serialPort.Close();
            }
            _isInitialized = false;
            UpdateStatus(DeviceStatus.Disconnected);
            OnStatusChanged?.Invoke(this, "Desconectado.");
        }

        public async Task<bool> Reset()
        {
            try
            {
                _isInitialized = false;
                UpdateStatus(DeviceStatus.Resetting);
                bool result = await SendCommand(Commands.BuildCommand(Commands.RESET));
                if (result)
                {
                    UpdateStatus(DeviceStatus.Connected);
                }
                return result;
            }
            catch (Exception ex)
            {
                OnError?.Invoke(this, $"Error al realizar Reset: {ex.Message}");
                UpdateStatus(DeviceStatus.Error);
                return false;
            }
        }

        public async Task<bool> Enable()
        {
            try
            {
                UpdateStatus(DeviceStatus.Enabling);
                
                if (!await Reset())
                {
                    OnError?.Invoke(this, "Error en el reset inicial");
                    return false;
                }

                await Task.Delay(1000);

                if (!await SendCommand(Commands.EnableAllBills()))
                {
                    OnError?.Invoke(this, "Error al habilitar canales de billetes");
                    return false;
                }

                if (!await SendCommand(Commands.SetDirection(true, true)))
                {
                    OnError?.Invoke(this, "Error al configurar dirección del billete");
                    return false;
                }

                if (!await SendCommand(Commands.BuildCommand(Commands.ENABLE)))
                {
                    OnError?.Invoke(this, "Error al habilitar el validador");
                    return false;
                }

                UpdateStatus(DeviceStatus.Enabled);
                return true;
            }
            catch (Exception ex)
            {
                OnError?.Invoke(this, $"Error en la secuencia de habilitación: {ex.Message}");
                UpdateStatus(DeviceStatus.Error);
                return false;
            }
        }

        public async Task<bool> Disable()
        {
            UpdateStatus(DeviceStatus.Disabled);
            return await SendCommand(Commands.BuildCommand(Commands.DISABLE));
        }

        public async Task<bool> Stack()
        {
            UpdateStatus(DeviceStatus.Stacking);
            bool result = await SendCommand(Commands.BuildCommand(Commands.STACK));
            if (result)
            {
                UpdateStatus(DeviceStatus.Enabled);
            }
            return result;
        }

        public async Task<bool> Return()
        {
            UpdateStatus(DeviceStatus.Returning);
            bool result = await SendCommand(Commands.BuildCommand(Commands.RETURN));
            if (result)
            {
                UpdateStatus(DeviceStatus.Enabled);
            }
            return result;
        }

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
            if (response.Length < 5 || response[0] != Commands.STX)
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
                case 0x31: // Status response
                    ProcessStatusResponse(response);
                    break;
                default:
                    OnStatusChanged?.Invoke(this, $"Comando recibido: {command:X2}");
                    break;
            }
        }

        private void ProcessStatusResponse(byte[] response)
        {
            if (response.Length < 6) return;

            var sensorStatus = new SensorStatus
            {
                EntrySensorBlocked = (response[3] & 0x01) != 0,
                ValidatingSensorBlocked = (response[3] & 0x02) != 0,
                StackingSensorBlocked = (response[3] & 0x04) != 0,
                ReturnSensorBlocked = (response[3] & 0x08) != 0
            };

            OnSensorStatusChanged?.Invoke(this, sensorStatus);
        }

        private void UpdateStatus(DeviceStatus newStatus)
        {
            _currentStatus = newStatus;
            var ledStatus = new LedStatus
            {
                PowerLed = true, // Siempre encendido si hay conexión
                ErrorLed = newStatus == DeviceStatus.Error,
                ReadyLed = newStatus == DeviceStatus.Enabled,
                ProcessingLed = newStatus == DeviceStatus.Enabling || 
                              newStatus == DeviceStatus.Stacking || 
                              newStatus == DeviceStatus.Returning ||
                              newStatus == DeviceStatus.Resetting
            };

            OnLedStatusChanged?.Invoke(this, ledStatus);
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
