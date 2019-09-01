using System;
using System.IO;

/* ************************************************************************
 * 
 * x86Emulator
 * 自作エミュレータで学ぶx86アーキテクチャ掲載x86エミュレータのC#移植版
 * Chapter3.2
 * 
 *********************************************************************** */

namespace x86Emulator
{
    class Program
    {
        static void Main(string[] args)
        {
            // 引数チェック
            if (args.Length != 1)
            {
                Console.WriteLine("usage x86Emulator filename");
                return;
            }



            Emulator emu = new Emulator(1024 * 1024, 0x0000, 0x7c00);
            // ファイルオープン
            using (var fs = new FileStream(args[0], FileMode.Open, FileAccess.Read))
            {
                fs.Read(emu.memory, 0, emu.memory.Length);
            }

            emu.initInstructions();

            uint code;
            while (emu.eip < 1024*1024)
            {
                code = emu.getCode8(0);
                Console.WriteLine("EIP = 0x{0:X8}, Code = 0x{1:X2}", emu.eip, code);

                if (emu.instructions[code] == null)
                {
                    Console.WriteLine("\n\nNot Implemented: {0:X2}", code);
                    break;
                }

                emu.instructions[code]();

                if (emu.eip == 0x00)
                {
                    Console.WriteLine("\n\nend of program.\n\n");
                    break;
                }
            }

            emu.dumpRegisters();
            return;
        }
    }

    

    public class Emulator
    {
        const UInt32 MEMORY_SIZE = 1024 * 1024;
        private enum Register : int
        {
            EAX, ECX, EDX, EBX, ESP, EBP, ESI, EDI
        }


        // 汎用レジスタ
        public UInt32[] registers = new UInt32[Enum.GetNames(typeof(Register)).Length];
        // EFLAGレジスタ
        public UInt32 eflags;
        // メモリ
        public Byte[] memory;
        // プログラムカウンタ
        public UInt32 eip;

        public Emulator(UInt64 size, UInt32 eip, UInt32 esp)
        {
            memory = new Byte[size];

            foreach (var i in registers)
            {
                registers[i] = 0;
            }

            this.eip = eip;
            this.registers[(int)Register.ESP] = esp;
        }

        public void dumpRegisters()
        {
            for (int i = 0; i < Enum.GetNames(typeof(Register)).Length; i++)
            {
                Console.WriteLine("{0} = 0x{1:X8}", Enum.GetName(typeof(Register), i), registers[i]);
            }
            Console.WriteLine("EIP = 0x{0:X8}", eip);
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


        void mov_r32_imm32()
        {
            Byte reg = (Byte)(getCode8(0) - 0xB8);
            UInt32 value = getCode32(1);
            registers[reg] = value;
            eip += 5;
        }

        void short_jump()
        {
            SByte diff = (SByte)getSignedCode8(1);
            eip += (UInt32)(diff + 2);
        }

        void near_jump()
        {
            SByte diff = (SByte)getSignedCode8(1);
            eip += (UInt32)(diff + 5);
        }



        public delegate void Instruction();
        public Instruction[] instructions = new Instruction[256];

        public void initInstructions()
        {
            for (int i = 0; i < 8; i++)
            {
                instructions[0xB8 + i] = mov_r32_imm32;
            }
            instructions[0xEB] = short_jump;
            instructions[0xE9] = near_jump;
        }

    }
    
}
