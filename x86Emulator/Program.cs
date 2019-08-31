using System;



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
            const UInt32 MEMORY_SIZE = 1024 * 1024;

            // 引数チェック
            if (args.Length != 1)
            {
                Console.WriteLine("usage x86Emulator filename");
                return;
            }

            Emulator emu = new Emulator(MEMORY_SIZE, 0x0000, 0x7c00);
            
            // バイナリファイルをメモリ展開
            if (emu.allocate(args[0], 0) != 0) return;                       

            while (true)
            {
                emu.fetch();
                if (emu.decode() != 0) break; 
                if (emu.execute() != 0) break;
            }

            emu.dumpRegisters();
            return;
        }
    }

    

    
    
}
