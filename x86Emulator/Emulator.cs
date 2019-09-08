using System;
using System.IO;

namespace x86Emulator
{
    public class Emulator
    {
        const UInt32 MEMORY_SIZE = 1024 * 1024;

        private uint code;

        public Register register;
        public Memory memory;

        public static Instruction[] ins;

        public Emulator(UInt64 size, UInt32 eip, UInt32 esp)
        {
            register = new Register(eip, esp);
            memory = new Memory(MEMORY_SIZE);    
            
        }

        public void fetch()
        {
            code = memory.getCode8(register.eip, 0);
            Console.WriteLine("EIP = 0x{0:X8}, Code = 0x{1:X2}", register.eip, code);
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
            if (register.eip == 0x00)
            {
                Console.WriteLine("\n\nend of program.\n\n");
                return -1;
            }
            return 0;
        }

    }

}
