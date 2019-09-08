using System;
using System.IO;


namespace x86Emulator
{
    public class Memory
    {
        public Byte[] memory;

        public Memory(UInt64 size)
        {
            memory = new Byte[size];
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

        public UInt32 getCode8(UInt32 eip, int index)
        {
            return memory[eip + index];
        }


        public Int32 getSignedCode8(UInt32 eip, int index)
        {
            return memory[eip + index];
        }

        public UInt32 getCode32(UInt32 eip, int index)
        {
            UInt32 ret = 0;
            for (int i = 0; i < 4; i++)
            {
                ret |= getCode8(eip, index + i) << (i * 8);
            }
            return ret;
        }


        public Int32 getSignedCode32(UInt32 eip, int index)
        {
            return (Int32)getCode32(eip, index);
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

        public void push32(Emulator emu, UInt32 value)
        {
            UInt32 address = emu.register.getRegister32((int)Register.Registers.ESP) - 4;
            emu.register.setRegister32((int)Register.Registers.ESP, address);
            setMemory32(address, value);
        }

        public UInt32 pop32(Emulator emu)
        {
            UInt32 address = emu.register.getRegister32((int)Register.Registers.ESP);
            UInt32 ret = getMemory32(address);
            emu.register.setRegister32((int)Register.Registers.ESP, address + 4);
            return ret;
        }
    }
}
