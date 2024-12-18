# JCM JBV-100-FSH Controller

Este proyecto es un controlador para el validador de billetes JCM JBV-100-FSH, implementado en .NET 6.0. Proporciona una interfaz gráfica para interactuar con el dispositivo a través del puerto serial.

## Características

- Conexión serial con el validador JBV-100-FSH
- Interfaz gráfica intuitiva
- Comandos principales implementados:
  - Reset
  - Enable/Disable
  - Stack
  - Return
- Registro de eventos y estados del validador
- Selección de puerto COM

## Requisitos

- .NET 6.0 SDK
- Windows OS
- Puerto COM disponible para la conexión con el validador

## Uso

1. Conecte el validador JBV-100-FSH a un puerto COM de su computadora
2. Ejecute la aplicación
3. Seleccione el puerto COM correspondiente
4. Haga clic en "Conectar"
5. Use los botones de comando para interactuar con el validador

## Desarrollo

El proyecto está estructurado en tres componentes principales:

- `JcmCommands.cs`: Definición de comandos y constantes del protocolo
- `JcmBillValidator.cs`: Clase principal para la comunicación con el validador
- `MainForm.cs`: Interfaz gráfica de usuario

## Licencia

[MIT](LICENSE)
