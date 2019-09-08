using System;
using System.IO;

namespace x86Emulator
{
    public enum Registers : int
    {
        EAX, ECX, EDX, EBX, ESP, EBP, ESI, EDI
    }

    public class Emulator
    {
        const UInt32 MEMORY_SIZE = 1024 * 1024;

        private uint code;

        // 汎用レジスタ
        public UInt32[] registers = new UInt32[Enum.GetNames(typeof(Registers)).Length];
        // EFLAGレジスタ
        public UInt32 eflags;
        // プログラムカウンタ
        public UInt32 eip;

        public Byte[] memory;

        public static Instruction[] ins;

        public Emulator(UInt64 size, UInt32 eip, UInt32 esp)
        {
            foreach (var i in registers)
            {
                registers[i] = 0;
            }
            this.eip = eip;
            registers[(int)Registers.ESP] = esp;

            memory = new Byte[size];
        }


        public void fetch()
        {
            code = getCode8(0);
            Console.WriteLine("EIP = 0x{0:X8}, Code = 0x{1:X2}", eip, code);
        }


        public int decode()
        {
            if (Instructions.instructions[code] == null)
            {
                Console.WriteLine("\n\nNot Implemented: {0:X2}", code);
                return -1;
            }
            return 0;
        }


        public int execute()
        {
            Instructions.instructions[code](this);
            if (eip == 0x00)
            {
                Console.WriteLine("\n\nend of program.\n\n");
                return -1;
            }
            return 0;
        }


        public int allocate(string path, int offset = 0)
        {
            try
            {
                using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    fs.Read(memory, offset, 0x200);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("File Cannot Open.");
                return -1;
            }
            return 0;
        }


        public void dumpRegisters()
        {
            for (int i = 0; i < Enum.GetNames(typeof(Registers)).Length; i++)
            {
                Console.WriteLine("{0} = 0x{1:X8}", Enum.GetName(typeof(Registers), i), registers[i]);
            }
            Console.WriteLine("EIP = 0x{0:X8}", eip);
        }




        public UInt32 getRegister32(int index)
        {
            return registers[index];
        }


        public void setRegister32(int index, UInt32 value)
        {
            registers[index] = value;
        }


        public UInt32 getCode8(int index)
        {
            return memory[eip + index];
        }


        public Int32 getSignedCode8(int index)
        {
            return memory[eip + index];
        }

        public UInt32 getCode32(int index)
        {
            UInt32 ret = 0;
            for (int i = 0; i < 4; i++)
            {
                ret |= getCode8(index + i) << (i * 8);
            }
            return ret;
        }


        public Int32 getSignedCode32(int index)
        {
            return (Int32)getCode32(index);
        }


        public void setMemory8(UInt32 address, UInt32 value)
        {
            memory[address] = (Byte)(value & 0xFF);
        }


        public void setMemory32(UInt32 address, UInt32 value)
        {
            for (int i = 0; i < 4; i++)
            {
                setMemory8((UInt32)(address + i), value >> (i * 8));
            }
        }


        public UInt32 getMemory8(UInt32 address)
        {
            return memory[address];
        }


        public UInt32 getMemory32(UInt32 address)
        {
            UInt32 ret = 0;
            for (int i = 0; i < 4; i++)
            {
                ret |= getMemory8((UInt32)(address + i)) << (8 * i);
            }
            return ret;
        }

        public void push32(UInt32 value)
        {
            UInt32 address = getRegister32((int)Registers.ESP) - 4;
            setRegister32((int)Registers.ESP, address);
            setMemory32(address, value);
        }

        public UInt32 pop32()
        {
            UInt32 address = getRegister32((int)Registers.ESP);
            UInt32 ret = getMemory32(address);
            setRegister32((int)Registers.ESP, address + 4);
            return ret;
        }
    }
}
