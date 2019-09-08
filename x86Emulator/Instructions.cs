using System;


namespace x86Emulator
{
    public delegate void Instruction(Emulator emu);
    public static class Instructions
    {
        public static void Initialize(Instruction[] instructions)
        {
            instructions[0x01] = add_rm32_r32;

            instructions[0x3B] = cmp_r32_rm32;

            for (int i = 0; i < 8; i++)
            {
                instructions[0x50 + i] = push_r32;
                instructions[0x58 + i] = pop_r32;
            }

            instructions[0x68] = push_imm32;
            instructions[0x6A] = push_imm8;

            instructions[0x70] = jo;
            instructions[0x71] = jno;
            instructions[0x72] = jc;
            instructions[0x73] = jnc;
            instructions[0x74] = jz;
            instructions[0x75] = jnz;
            instructions[0x78] = js;
            instructions[0x79] = jns;
            instructions[0x7C] = jl;
            instructions[0x7E] = jle;

            instructions[0x83] = code_83;
            instructions[0x89] = mov_rm32_r32;
            instructions[0x8B] = mov_r32_rm32;

            for (int i = 0; i < 8; i++)
            {
                instructions[0xB8 + i] = mov_r32_imm32;
            }

            instructions[0xC3] = ret;
            instructions[0xC7] = mov_rm32_imm32;
            instructions[0xC9] = leave;
            instructions[0xE8] = call_rel32;
            instructions[0xE9] = near_jump;
            instructions[0xEB] = short_jump;
            instructions[0xFF] = code_ff;
        }

        // opcode 0x01
        public static void add_rm32_r32(Emulator emu)
        {
            emu.eip += 1;
            ModRM modrm = new ModRM();
            modrm.Parse(emu);
            UInt32 r32 = modrm.get_r32(emu);
            UInt32 rm32 = modrm.get_rm32(emu);
            modrm.set_rm32(emu, rm32 + r32);
        }



        // opcode 0x3B
        public static void cmp_r32_rm32(Emulator emu)
        {
            emu.eip += 1;
            ModRM modrm = new ModRM();
            modrm.Parse(emu);
            UInt32 r32 = modrm.get_r32(emu);
            UInt32 rm32 = modrm.get_rm32(emu);
            UInt64 result = (UInt64)r32 - (UInt64)rm32;
            emu.update_eflags_sub(r32, rm32, result);
        }
        
        
        
        // opcode 0x50-0x57
        public static void push_r32(Emulator emu)
        {
            Byte reg = (Byte)(emu.getCode8(0) - 0x50);
            emu.push32(emu.getRegister32(reg));
            emu.eip += 1;
        }



        // opcode 0x58-0x5F
        public static void pop_r32(Emulator emu)
        {
            Byte reg = (Byte)(emu.getCode8(0) - 0x58);
            emu.setRegister32(reg, emu.pop32());
            emu.eip += 1;
        }



        // opcode 0x68
        public static void push_imm32(Emulator emu)
        {
            UInt32 value = emu.getCode32(1);
            emu.push32(value);
            emu.eip += 5;
        }



        // opcode 0x6A
        public static void push_imm8(Emulator emu)
        {
            Byte value = (Byte)emu.getCode8(1);
            emu.push32(value);
            emu.eip += 2;
        }



        // opcode 0x70
        public static void jo(Emulator emu)
        {
            int diff = emu.eflags.HasFlag(Eflags.OVERFLOW) ? emu.getSignedCode8(1) : 0;
            emu.eip += (uint)(diff + 2);
        }



        // opcode 0x71
        public static void jno(Emulator emu)
        {
            int diff = emu.eflags.HasFlag(Eflags.OVERFLOW) ? 0 : emu.getSignedCode8(1);
            emu.eip += (uint)(diff + 2);
        }



        // opcode 0x72
        public static void jc(Emulator emu)
        {
            int diff = emu.eflags.HasFlag(Eflags.CARRY) ? emu.getSignedCode8(1) : 0;
            emu.eip += (uint)(diff + 2);
        }



        // opcode 0x73
        public static void jnc(Emulator emu)
        {
            int diff = emu.eflags.HasFlag(Eflags.CARRY) ? 0 : emu.getSignedCode8(1);
            emu.eip += (uint)(diff + 2);
        }



        // opcode 0x74
        public static void jz(Emulator emu)
        {
            int diff = emu.eflags.HasFlag(Eflags.ZERO) ? emu.getSignedCode8(1) : 0;
            emu.eip += (uint)(diff + 2);
        }



        // opcode 0x75
        public static void jnz(Emulator emu)
        {
            int diff = emu.eflags.HasFlag(Eflags.ZERO) ? 0 : emu.getSignedCode8(1);
            emu.eip += (uint)(diff + 2);
        }



        // opcode 0x78
        public static void js(Emulator emu)
        {
            int diff = emu.eflags.HasFlag(Eflags.SIGN) ? emu.getSignedCode8(1) : 0;
            emu.eip += (uint)(diff + 2);
        }



        // opcode 0x79
        public static void jns(Emulator emu)
        {
            int diff = emu.eflags.HasFlag(Eflags.SIGN) ? 0 : emu.getSignedCode8(1);
            emu.eip += (uint)(diff + 2);
        }



        // opcode 0x7C
        public static void jl(Emulator emu)
        {
            int diff = emu.eflags.HasFlag(Eflags.SIGN) != emu.eflags.HasFlag(Eflags.OVERFLOW) ? emu.getSignedCode8(1) : 0;
            emu.eip += (uint)(diff + 2);
        }



        // opcode 0x7E
        public static void jle(Emulator emu)
        {
            int diff = emu.eflags.HasFlag(Eflags.ZERO) || (emu.eflags.HasFlag(Eflags.SIGN) != emu.eflags.HasFlag(Eflags.OVERFLOW)) ? emu.getSignedCode8(1) : 0;
            emu.eip += (uint)(diff + 2);
        }
        
        
        
        // opcode 0x83
        public static void code_83(Emulator emu)
        {
            emu.eip += 1;
            ModRM modrm = new ModRM();
            modrm.Parse(emu);

            switch (modrm.opcode)
            {
                case 0:
                    add_rm32_imm8(emu, modrm);
                    break;
                case 5:
                    sub_rm32_imm8(emu, modrm);
                    break;
                case 7:
                    cmp_rm32_imm8(emu, modrm);
                    break;
                default:
                    Console.WriteLine("not implemented: 83 /%d", modrm.opcode);
                    System.Environment.Exit(0);
                    break;
            }
        }

        private static void add_rm32_imm8(Emulator emu, ModRM modrm)
        {
            UInt32 rm32 = modrm.get_rm32(emu);
            UInt32 imm8 = (UInt32)emu.getSignedCode8(0);
            emu.eip += 1;
            modrm.set_rm32(emu, rm32 + imm8);
        }

        static void sub_rm32_imm8(Emulator emu, ModRM modrm)
        {
            UInt32 rm32 = modrm.get_rm32(emu);
            UInt32 imm8 = (UInt32)emu.getSignedCode8(0);
            emu.eip += 1;
            UInt64 result = (UInt64)rm32 - (UInt64)imm8;
            modrm.set_rm32(emu, (UInt32)result);
            emu.update_eflags_sub(rm32, imm8, result);
        }

        static void cmp_rm32_imm8(Emulator emu, ModRM modrm)
        {
            UInt32 rm32 = modrm.get_rm32(emu);
            UInt32 imm8 = (UInt32)emu.getSignedCode8(0);
            emu.eip += 1;
            UInt64 result = (UInt64)rm32 - (UInt64)imm8;
            emu.update_eflags_sub(rm32, imm8, result);
        }
        


        // opcode 0x89
        public static void mov_rm32_r32(Emulator emu)
        {
            emu.eip += 1;
            ModRM modrm = new ModRM();
            modrm.Parse(emu);
            UInt32 r32 = modrm.get_r32(emu);
            modrm.set_rm32(emu, r32);
        }



        // opcode 0x8B
        public static void mov_r32_rm32(Emulator emu)
        {
            emu.eip += 1;
            ModRM modrm = new ModRM();
            modrm.Parse(emu);
            UInt32 rm32 = modrm.get_rm32(emu);
            modrm.set_r32(emu, rm32);
        }



        // opcode 0xB8～0xBF
        public static void mov_rm32_imm32(Emulator emu)
        {
            emu.eip++;
            ModRM modrm = new ModRM();
            modrm.Parse(emu);
            UInt32 value = emu.getCode32(0);
            emu.eip += 4;
            modrm.set_rm32(emu, value);
        }



        // opcode 0xC3
        public static void ret(Emulator emu)
        {
            emu.eip = emu.pop32();
        }

        public static void leave(Emulator emu)
        {
            UInt32 ebp = emu.getRegister32((int)Registers.EBP);
            emu.setRegister32((int)Registers.ESP, ebp);
            emu.setRegister32((int)Registers.EBP, emu.pop32());

            emu.eip += 1;
        }
        
        
        
        // opcode 0xC7
        public static void mov_r32_imm32(Emulator emu)
        {
            Byte reg = (Byte)(emu.getCode8(0) - 0xB8);
            UInt32 value = emu.getCode32(1);
            emu.registers[reg] = value;
            emu.eip += 5;
        }



        // opcode 0xE8
        public static void call_rel32(Emulator emu)
        {
            Int32 diff = emu.getSignedCode32(1);
            emu.push32(emu.eip + 5);
            emu.eip += (uint)(diff + 5);
        }
        
        
        
        // opcode 0xE9
        public static void near_jump(Emulator emu)
        {
            Int32 diff = emu.getSignedCode32(1);
            emu.eip += (UInt32)(diff + 5);
        }



        // opcode 0xEB
        public static void short_jump(Emulator emu)
        {
            SByte diff = (SByte)emu.getSignedCode8(1);
            emu.eip += (UInt32)(diff + 2);
        }



        // opcode 0xFF
        public static void code_ff(Emulator emu)
        {
            emu.eip += 1;
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
