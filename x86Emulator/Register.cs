using System;


namespace x86Emulator
{
    public class Register
    {
        public enum Registers : int
        {
            EAX, ECX, EDX, EBX, ESP, EBP, ESI, EDI
        }

        // 汎用レジスタ
        public UInt32[] registers = new UInt32[Enum.GetNames(typeof(Registers)).Length];
        // EFLAGレジスタ
        public UInt32 eflags;
        // プログラムカウンタ
        public UInt32 eip;

        public Register(UInt32 eip, UInt32 esp)
        {
            foreach (var i in registers)
            {
                registers[i] = 0;
            }
            this.eip = eip;
            registers[(int)Registers.ESP] = esp;
        }

        public UInt32 getRegister32(int index)
        {
            return registers[index];
        }


        public void setRegister32(int index, UInt32 value)
        {
            registers[index] = value;
        }

        public void dumpRegisters()
        {
            for (int i = 0; i < Enum.GetNames(typeof(Registers)).Length; i++)
            {
                Console.WriteLine("{0} = 0x{1:X8}", Enum.GetName(typeof(Registers), i), registers[i]);
            }
            Console.WriteLine("EIP = 0x{0:X8}", eip);
        }

    }
}
