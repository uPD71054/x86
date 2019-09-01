﻿using System;
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
            
            // バイナリファイルをメモリ展開
            if (emu.allocate(args[0], 0) != 0) return;                       

            while (emu.eip < 1024*1024)
            {
                emu.fetch();
                if (emu.decode() != 0) break; 
                if (emu.execute() != 0) break;
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
        private uint code;

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
            registers[(int)Register.ESP] = esp;

            initInstructions();
        }

        public void fetch()
        {
            code = getCode8(0);
            Console.WriteLine("EIP = 0x{0:X8}, Code = 0x{1:X2}", eip, code);
        }

        public int decode()
        {
            if (instructions[code] == null)
            {
                Console.WriteLine("\n\nNot Implemented: {0:X2}", code);
                return -1;
            }
            return 0;
        }

        public int execute()
        {
            instructions[code]();
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
            for (int i = 0; i < Enum.GetNames(typeof(Register)).Length; i++)
            {
                Console.WriteLine("{0} = 0x{1:X8}", Enum.GetName(typeof(Register), i), registers[i]);
            }
            Console.WriteLine("EIP = 0x{0:X8}", eip);
        }



        private UInt32 getCode8(int index)
        {
            return memory[eip + index];
        }

        private Int32 getSignedCode8(int index)
        {
            return memory[eip + index];
        }

        private UInt32 getCode32(int index)
        {
            UInt32 ret = 0;
            for (int i = 0; i < 4; i++)
            {
                ret |= getCode8(index + i) << (i * 8);
            }
            return ret;
        }

        private Int32 getSignedCode32(int index)
        {
            return (Int32)getCode32(index);
        }


        private void mov_r32_imm32()
        {
            Byte reg = (Byte)(getCode8(0) - 0xB8);
            UInt32 value = getCode32(1);
            registers[reg] = value;
            eip += 5;
        }

        private void short_jump()
        {
            SByte diff = (SByte)getSignedCode8(1);
            eip += (UInt32)(diff + 2);
        }

        private void near_jump()
        {
            SByte diff = (SByte)getSignedCode8(1);
            eip += (UInt32)(diff + 5);
        }



        private delegate void Instruction();
        private Instruction[] instructions = new Instruction[256];

        private void initInstructions()
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
