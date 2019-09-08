using System;
using System.IO;

/* ************************************************************************
 * 
 * x86Emulator
 * 自作エミュレータで学ぶx86アーキテクチャ掲載x86エミュレータのC#移植版
 * Chapter3.10
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

            Emulator emu = new Emulator(1024 * 1024, 0x7c00, 0x7c00);
            
            // バイナリファイルをメモリ展開
            if (emu.Allocate(args[0], 0x7c00) != 0) return;                       

            while (true)
            {
                emu.Fetch();
                if (emu.Decode() != 0) break; 
                if (emu.Execute() != 0) break;
            }

            emu.DumpRegisters();
            return;
        }
    }

}
