namespace BillValidatorEmulator
{
    public static class Commands
    {
        // Comandos básicos
        public static readonly byte RESET = 0x30;
        public static readonly byte STATUS = 0x31;
        public static readonly byte ENABLE = 0x3E;
        public static readonly byte DISABLE = 0x3F;
        public static readonly byte STACK = 0x35;
        public static readonly byte RETURN = 0x36;
        public static readonly byte HOLD = 0x38;

        // Comandos de configuración
        public static readonly byte SET_SECURITY = 0x32;
        public static readonly byte SET_INHIBITS = 0x34;
        public static readonly byte SET_DIRECTION = 0x3C;
        public static readonly byte SET_OPTIONAL_FUNCTION = 0x37;
        public static readonly byte GET_VERSION = 0x33;

        // Bytes de control
        public static readonly byte STX = 0x02;
        public static readonly byte ETX = 0x03;
        public static readonly byte DLE = 0x10;
        public static readonly byte ACK = 0x06;
        public static readonly byte NAK = 0x15;

        public static byte[] BuildCommand(byte command)
        {
            byte[] cmd = new byte[5];
            cmd[0] = STX;
            cmd[1] = 0x00;
            cmd[2] = command;
            cmd[3] = ETX;
            cmd[4] = CalculateChecksum(cmd, 1, 3);
            return cmd;
        }

        public static byte[] BuildCommandWithData(byte command, byte[] data)
        {
            byte[] cmd = new byte[5 + data.Length];
            cmd[0] = STX;
            cmd[1] = 0x00;
            cmd[2] = command;
            System.Array.Copy(data, 0, cmd, 3, data.Length);
            cmd[3 + data.Length] = ETX;
            cmd[4 + data.Length] = CalculateChecksum(cmd, 1, 3 + data.Length);
            return cmd;
        }

        public static byte[] EnableAllBills()
        {
            byte[] data = new byte[8];
            for (int i = 0; i < 8; i++)
            {
                data[i] = 0xFF;
            }
            return BuildCommandWithData(SET_INHIBITS, data);
        }

        public static byte[] SetDirection(bool faceUp, bool frontFirst)
        {
            byte direction = 0x00;
            if (faceUp) direction |= 0x01;
            if (frontFirst) direction |= 0x02;
            return BuildCommandWithData(SET_DIRECTION, new byte[] { direction });
        }

        private static byte CalculateChecksum(byte[] data, int start, int length)
        {
            byte sum = 0;
            for (int i = start; i < start + length; i++)
            {
                sum ^= data[i];
            }
            return sum;
        }
    }
}
