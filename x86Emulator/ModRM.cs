using System;


namespace x86Emulator
{
    public class ModRM
    {
        public Byte mod, opcode, regIndex, rm, sib, disp8;
        public UInt32 disp32;

        public ModRM()
        {
            mod = 0;
            opcode = regIndex = 0;
            rm = 0;
            sib = 0;            
            disp32 = disp8 = 0;
        }

        public void Parse(Emulator emu)
        {
            mod = 0;
            opcode = regIndex = 0;
            rm = 0;
            sib = 0;
            disp32 = disp8 = 0;

            Byte code;
            code = (Byte)emu.memory.getCode8(emu.register.eip, 0);

            mod = (Byte)((code & 0xC0) >> 6);
            opcode = regIndex = (Byte)((code & 0x38) >> 3);
            rm = (Byte)(code & 0x07);

            emu.register.eip++;

            if (mod != 0x03 && rm == 0x04)
            {
                sib = (Byte)emu.memory.getCode8(emu.register.eip, 0);
                emu.register.eip++;
            }

            if ((mod == 0x00 && rm == 0x05) || mod == 0x02)
            {
                disp32 = emu.memory.getCode32(emu.register.eip, 0);
                emu.register.eip += 4;
            }
            else if (mod == 0x01)
            {
                disp8 = (Byte)emu.memory.getCode8(emu.register.eip, 0);
                emu.register.eip += 1;
            }
        }

        public UInt32 calc_memory_address(Emulator emu)
        {
            if (mod == 0x00)
            {
                if (rm == 0x04)
                {
                    Console.WriteLine("not implemented ModRM mod = 0x00, rm = 0x04");
                    Environment.Exit(0);
                }
                else if (rm == 5)
                {
                    return disp32;
                }
                else
                {
                    return emu.register.getRegister32(rm);
                }
            }
            else if (mod == 0x01)
            {
                if (rm == 4)
                {
                    Console.WriteLine("not implemented ModRM mod = 0x01, rm = 0x04");
                    Environment.Exit(0);
                }
                else
                {
                    return emu.register.getRegister32(rm) + disp8;
                }
            }
            else if (mod == 0x02)
            {
                if (rm == 0x04)
                {
                    Console.WriteLine("not implemented ModRM mod = 0x02, rm = 0x04");
                    Environment.Exit(0);
                }
                else
                {
                    return emu.register.getRegister32(rm) + disp32;
                }
            }
            else
            {
                Console.WriteLine("not implemented ModRM mod = 0x03");
                Environment.Exit(0);
            }
            return 0;
        }

        public void set_rm32(Emulator emu, UInt32 value)
        {
            if (mod == 0x03)
            {
                emu.register.setRegister32(rm, value);
            }
            else
            {
                UInt32 address = calc_memory_address(emu);
                emu.memory.setMemory32(address, value);
            }
        }

        public UInt32 get_rm32(Emulator emu)
        {
            if (mod == 0x03)
            {
                return emu.register.getRegister32(rm);
            }
            else
            {
                UInt32 address = calc_memory_address(emu);
                return emu.memory.getMemory32(address);
            }
        }

        public void set_r32(Emulator emu, UInt32 value)
        {
            emu.register.setRegister32(regIndex, value);
        }

        public UInt32 get_r32(Emulator emu)
        {
            return emu.register.getRegister32(regIndex);
        }


    }
}
