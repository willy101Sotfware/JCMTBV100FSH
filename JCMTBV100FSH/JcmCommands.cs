using System;

namespace JCMTBV100FSH
{
    public static class JcmCommands
    {
        // Comandos básicos del JBV-100-FSH
        public static readonly byte RESET = 0x30;
        public static readonly byte STATUS = 0x31;
        public static readonly byte ENABLE = 0x3E;
        public static readonly byte DISABLE = 0x3F;
        public static readonly byte STACK = 0x35;
        public static readonly byte RETURN = 0x36;
        public static readonly byte HOLD = 0x38;

        // Bytes de control
        public static readonly byte STX = 0x02;
        public static readonly byte ETX = 0x03;
        public static readonly byte DLE = 0x10;

        // Métodos para construir comandos
        public static byte[] BuildCommand(byte command)
        {
            byte[] cmd = new byte[5];
            cmd[0] = STX;
            cmd[1] = 0x00; // Dirección por defecto
            cmd[2] = command;
            cmd[3] = ETX;
            cmd[4] = CalculateChecksum(cmd, 1, 3);
            return cmd;
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
