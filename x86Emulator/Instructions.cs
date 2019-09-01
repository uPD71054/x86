using System;


namespace x86Emulator
{
    public delegate void Instruction(Emulator emu);
    public static class Instructions
    {
        public static Instruction[] instructions;

        static Instructions()
        {
            instructions = new Instruction[256];

            instructions[0x01] = add_rm32_r32;
            instructions[0x83] = code_83;
            instructions[0x89] = mov_rm32_r32;
            instructions[0x8B] = mov_r32_rm32;
            for (int i = 0; i < 8; i++)
            {
                instructions[0xB8 + i] = mov_r32_imm32;
            }
            instructions[0xC7] = mov_rm32_imm32;
            instructions[0xE9] = near_jump;
            instructions[0xEB] = short_jump;
            instructions[0xFF] = code_ff;
        }

        // opcode 0x01
        private static void add_rm32_r32(Emulator emu)
        {
            emu.register.eip += 1;
            ModRM modrm = new ModRM();
            modrm.Parse(emu);
            UInt32 r32 = modrm.get_r32(emu);
            UInt32 rm32 = modrm.get_rm32(emu);
            modrm.set_rm32(emu, rm32 + r32);
        }



        // opcode 0x83
        private static void code_83(Emulator emu)
        {
            emu.register.eip += 1;
            ModRM modrm = new ModRM();
            modrm.Parse(emu);

            switch (modrm.opcode)
            {
                case 5:
                    sub_rm32_imm8(emu, modrm);
                    break;
                default:
                    Console.WriteLine("not implemented: 83 /%d", modrm.opcode);
                    System.Environment.Exit(0);
                    break;
            }
        }

        private static void sub_rm32_imm8(Emulator emu, ModRM modrm)
        {
            UInt32 rm32 = modrm.get_rm32(emu);
            UInt32 imm8 = (UInt32)emu.memory.getSignedCode8(emu.register.eip, 0);
            emu.register.eip += 1;
            modrm.set_rm32(emu, rm32 - imm8);
        }



        // opcode 0x89
        private static void mov_rm32_r32(Emulator emu)
        {
            emu.register.eip += 1;
            ModRM modrm = new ModRM();
            modrm.Parse(emu);
            UInt32 r32 = modrm.get_r32(emu);
            modrm.set_rm32(emu, r32);
        }



        // opcode 0x8B
        private static void mov_r32_rm32(Emulator emu)
        {
            emu.register.eip += 1;
            ModRM modrm = new ModRM();
            modrm.Parse(emu);
            UInt32 rm32 = modrm.get_rm32(emu);
            modrm.set_r32(emu, rm32);
        }



        // opcode 0xB8～0xBF
        private static void mov_rm32_imm32(Emulator emu)
        {
            emu.register.eip++;
            ModRM modrm = new ModRM();
            modrm.Parse(emu);
            UInt32 value = emu.memory.getCode32(emu.register.eip, 0);
            emu.register.eip += 4;
            modrm.set_rm32(emu, value);
        }



        // opcode 0xC7
        private static void mov_r32_imm32(Emulator emu)
        {
            Byte reg = (Byte)(emu.memory.getCode8(emu.register.eip, 0) - 0xB8);
            UInt32 value = emu.memory.getCode32(emu.register.eip, 1);
            emu.register.registers[reg] = value;
            emu.register.eip += 5;
        }



        // opcode 0xE9
        private static void near_jump(Emulator emu)
        {
            SByte diff = (SByte)emu.memory.getSignedCode8(emu.register.eip, 1);
            emu.register.eip += (UInt32)(diff + 5);
        }



        // opcode 0xEB
        private static void short_jump(Emulator emu)
        {
            SByte diff = (SByte)emu.memory.getSignedCode8(emu.register.eip, 1);
            emu.register.eip += (UInt32)(diff + 2);
        }



        // opcode 0xFF
        private static void code_ff(Emulator emu)
        {
            emu.register.eip += 1;
            ModRM modrm = new ModRM();
            modrm.Parse(emu);

            switch (modrm.opcode)
            {
                case 0:
                    inc_rm32(emu, modrm);
                    break;
                default:
                    Console.WriteLine("not implemented: 83 /%d", modrm.opcode);
                    System.Environment.Exit(1);
                    break;
            }
        }

        private static void inc_rm32(Emulator emu, ModRM modrm)
        {
            UInt32 value = modrm.get_rm32(emu);
            modrm.set_rm32(emu, value + 1);
        }

    }
}
