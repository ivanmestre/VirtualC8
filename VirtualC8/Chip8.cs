using System;
using System.IO;
using System.Diagnostics;

namespace VirtualC8
{
    class Chip8
    {
        private int frequency;
        private UInt16 opcode;

        private byte[] memory = new byte[4096];

        private byte[] v_registers = new byte[16];

        private UInt16 index_register;

        private UInt16 program_counter;

        public byte[] pixels = new byte[64 * 32];

        private byte delay_timer;

        public byte sound_duration;

        private UInt16[] stack = new UInt16[16];

        private UInt16 stack_pointer;

        public byte[] keys = new byte[16];

        private byte[] chip8_fontset = new byte[80]
        {
          0xF0, 0x90, 0x90, 0x90, 0xF0, // 0
          0x20, 0x60, 0x20, 0x20, 0x70, // 1
          0xF0, 0x10, 0xF0, 0x80, 0xF0, // 2
          0xF0, 0x10, 0xF0, 0x10, 0xF0, // 3
          0x90, 0x90, 0xF0, 0x10, 0x10, // 4
          0xF0, 0x80, 0xF0, 0x10, 0xF0, // 5
          0xF0, 0x80, 0xF0, 0x90, 0xF0, // 6
          0xF0, 0x10, 0x20, 0x40, 0x40, // 7
          0xF0, 0x90, 0xF0, 0x90, 0xF0, // 8
          0xF0, 0x90, 0xF0, 0x10, 0xF0, // 9
          0xF0, 0x90, 0xF0, 0x90, 0x90, // A
          0xE0, 0x90, 0xE0, 0x90, 0xE0, // B
          0xF0, 0x80, 0x80, 0x80, 0xF0, // C
          0xE0, 0x90, 0x90, 0x90, 0xE0, // D
          0xF0, 0x80, 0xF0, 0x80, 0xF0, // E
          0xF0, 0x80, 0xF0, 0x80, 0x80  // F
        };

        public bool draw_flag;
        public bool sound_flag;

        public double cycleTimer = 0;
        public double sixtyHzTimer = 0;

        Random random;

        public void initialize(int freq)
        {   
            program_counter = 0x200;
            opcode = 0;
            index_register = 0;
            stack_pointer = 0;
            frequency = freq;

            for(int i= 0; i < pixels.Length; i++)
            {
                pixels[i] = 0;
            }
            for (int i = 0; i < stack.Length; i++)
            {
                stack[i] = 0;
            }
            for (int i = 0; i < v_registers.Length; i++)
            {
                v_registers[i] = 0;
            }
            for (int i = 0; i < memory.Length; i++)
            {
                memory[i] = 0;
            }
            for (int i = 0; i < keys.Length; i++)
            {
                keys[i] = 0;
            }
            draw_flag = false;

            for(int i = 0; i < 80; i++)
            {
                memory[i] = chip8_fontset[i];
            }

            delay_timer = 0;
            sound_duration = 0;

            draw_flag = true;

            random = new Random();
        }

        public void loadGame(string path)
        {
            byte[] rom = File.ReadAllBytes(path);
            for (int i = 0; i < rom.Length; i++)
            {
                memory[i + 512] = rom[i];
            }
        }

        public void update(double deltaTimeMs)
        {
            sixtyHzTimer += deltaTimeMs;
            if (sixtyHzTimer >= (1000.0 / 60.0))
            {
                updateTimers();
                sixtyHzTimer -= (1000.0 / 60.0); // Decrement, don't reset to 0
            }

            cycleTimer += deltaTimeMs;
            double cycleIntervalMs = 1000.0 / frequency;

            while (cycleTimer >= cycleIntervalMs)
            {
                emulateCycle();
                cycleTimer -= cycleIntervalMs;
            }
        }

        private void emulateCycle()
        {
            opcode = (UInt16)((memory[program_counter] << 8) | memory[program_counter + 1]);

            switch(opcode & 0xF000)
            {
                case 0x0000:
                    switch(opcode & 0x000F)
                    {
                        case 0x0000:
                            for(int i = 0; i < pixels.Length; i++)
                            {
                                pixels[i] = 0;
                            }
                            draw_flag = true;
                            program_counter += 2;
                            break;

                        case 0x000E:
                            stack_pointer--;
                            program_counter = stack[stack_pointer];
                            program_counter += 2;
                            break;

                        default:
                            Console.WriteLine("Unknown opcode: 0x" + opcode.ToString("X4"));
                            break;
                    }
                    break;

                case 0x1000:
                    program_counter = (UInt16)(opcode & 0x0FFF);
                    break;

                case 0x2000:
                    stack[stack_pointer] = program_counter;
                    stack_pointer++;
                    program_counter = (UInt16)(opcode & 0x0FFF);
                    break;

                case 0x3000:
                    if(v_registers[(opcode & 0x0F00) >> 8] == (opcode & 0x00FF))
                    {
                        program_counter += 4;
                    } else
                    {
                        program_counter += 2;
                    }
                    break;

                case 0x4000:
                    if (v_registers[(opcode & 0x0F00) >> 8] != (opcode & 0x00FF))
                    {
                        program_counter += 4;
                    }
                    else
                    {
                        program_counter += 2;
                    }
                    break;

                case 0x5000:
                    if (v_registers[(opcode & 0x0F00) >> 8] == v_registers[(opcode & 0x00F0) >> 4])
                    {
                        program_counter += 4;
                    }
                    else
                    {
                        program_counter += 2;
                    }
                    break;

                case 0x6000:
                    v_registers[(opcode & 0x0F00) >> 8] = (byte) (opcode & 0x00FF);
                    program_counter += 2;
                    break;

                case 0x7000:
                    v_registers[(opcode & 0x0F00) >> 8] += (byte)(opcode & 0x00FF);
                    program_counter += 2;
                    break;

                case 0x8000:
                    switch(opcode & 0x000F)
                    {
                        case 0x0000:
                            v_registers[(opcode & 0x0F00) >> 8] = v_registers[(opcode & 0x00F0) >> 4];
                            program_counter += 2;
                            break;

                        case 0x0001:
                            v_registers[(opcode & 0x0F00) >> 8] |= v_registers[(opcode & 0x00F0) >> 4];
                            program_counter += 2;
                            break;

                        case 0x0002:
                            v_registers[(opcode & 0x0F00) >> 8] &= v_registers[(opcode & 0x00F0) >> 4];
                            program_counter += 2;
                            break;

                        case 0x0003:
                            v_registers[(opcode & 0x0F00) >> 8] ^= v_registers[(opcode & 0x00F0) >> 4];
                            program_counter += 2;
                            break;

                        case 0x0004:
                            if(v_registers[(opcode & 0x00F0) >> 4] > (0xFF - v_registers[(opcode & 0x0F00) >> 8]))
                            {
                                v_registers[0xF] = 1;
                            } else
                            {
                                v_registers[0xF] = 0;
                            }
                            v_registers[(opcode & 0x0F00) >> 8] += v_registers[(opcode & 0x00F0) >> 4];
                            program_counter += 2;
                            break;

                        case 0x0005:
                            if (v_registers[(opcode & 0x0F00) >> 8] >= v_registers[(opcode & 0x00F0) >> 4])
                            {
                                v_registers[0xF] = 1;
                            }
                            else
                            {
                                v_registers[0xF] = 0;
                            }
                            v_registers[(opcode & 0x0F00) >> 8] -= v_registers[(opcode & 0x00F0) >> 4];
                            program_counter += 2;
                            break;

                        case 0x0006:
                            if((v_registers[(opcode & 0x0F00) >> 8] & 0x1) != 0)
                            {
                                v_registers[0xF] = 1;
                            } else
                            {
                                v_registers[0xF] = 0;
                            }
                            v_registers[(opcode & 0x0F00) >> 8] >>= 1;
                            program_counter += 2;
                            break;

                        case 0x0007:
                            if (v_registers[(opcode & 0x00F0) >> 4] >= v_registers[(opcode & 0x0F00) >> 8])
                            {
                                v_registers[0xF] = 1;
                            }
                            else
                            {
                                v_registers[0xF] = 0;
                            }
                            v_registers[(opcode & 0x0F00) >> 8] = (byte) (v_registers[(opcode & 0x00F0) >> 4] - v_registers[(opcode & 0x0F00) >> 8]);
                            program_counter += 2;
                            break;

                        case 0x000E:
                            if ((v_registers[(opcode & 0x0F00) >> 8] >> 7) != 0)
                            {
                                v_registers[0xF] = 1;
                            }
                            else
                            {
                                v_registers[0xF] = 0;
                            }
                            v_registers[(opcode & 0x0F00) >> 8] <<= 1;
                            program_counter += 2;
                            break;

                        default:
                            Console.WriteLine("Unknown opcode: 0x" + opcode.ToString("X4"));
                            break;
                    }
                    break;

                case 0x9000:
                    if (v_registers[(opcode & 0x0F00) >> 8] != v_registers[(opcode & 0x00F0) >> 4])
                    {
                        program_counter += 4;
                    }
                    else
                    {
                        program_counter += 2;
                    }
                    break;

                case 0xA000:
                    index_register = (UInt16) (opcode & 0x0FFF);
                    program_counter += 2;
                    break;

                case 0xB000:
                    program_counter = (UInt16) (v_registers[0] + (opcode & 0x0FFF));
                    break;

                case 0xC000:
                    v_registers[(opcode & 0x0F00) >> 8] = (byte) (random.Next(0, 256) & (opcode & 0x00FF));
                    program_counter += 2;
                    break;

                case 0xD000:
                    UInt16 x = v_registers[(opcode & 0x0F00) >> 8];
                    UInt16 y = v_registers[(opcode & 0x00F0) >> 4];
                    UInt16 height = (UInt16) (opcode & 0x000F);
                    UInt16 pixel;

                    v_registers[0xF] = 0;
                    for(int yline = 0; yline < height; yline++)
                    {
                        pixel = memory[index_register + yline];
                        for(int xline = 0; xline < 8; xline++)
                        {
                            if((pixel & (0x80 >> xline)) != 0)
                            {
                                if (pixels[(x + xline + ((y + yline) * 64)) % (64 * 32)] == 1)
                                {
                                    v_registers[0xF] = 1;
                                }
                                pixels[(x + xline + ((y + yline) * 64)) % (64 * 32)] ^= 1;
                            }
                        }
                    }

                    draw_flag = true;
                    program_counter += 2;
                    break;

                case 0xE000:
                    switch(opcode & 0x00FF)
                    {
                        case 0x009E:
                            if(keys[v_registers[(opcode & 0x0F00) >> 8]] != 0)
                            {
                                program_counter += 4;
                            } else
                            {
                                program_counter += 2;
                            }
                            break;

                        case 0x00A1:
                            if (keys[v_registers[(opcode & 0x0F00) >> 8]] == 0)
                            {
                                program_counter += 4;
                            }
                            else
                            {
                                program_counter += 2;
                            }
                            break;

                        default:
                            Console.WriteLine("Unknown opcode: 0x" + opcode.ToString("X4"));
                            break;
                    }
                    break;

                case 0xF000:
                    switch(opcode & 0x00FF)
                    {
                        case 0x0007:
                            v_registers[(opcode & 0x0F00) >> 8] = delay_timer;
                            program_counter += 2;
                            break;

                        case 0x000A:
                            int j = 0;
                            bool pressed = false;
                            while(j < keys.Length && !pressed)
                            {
                                if(keys[j] != 0)
                                {
                                    v_registers[(opcode & 0x0F00) >> 8] = (byte) j;
                                    pressed = true;
                                }
                                j++;
                            }

                            if (!pressed)
                            {
                                return;
                            }

                            program_counter += 2;
                            break;

                        case 0x0015:
                            delay_timer = v_registers[(opcode & 0x0F00) >> 8];
                            program_counter += 2;
                            break;

                        case 0x0018:
                            sound_duration = v_registers[(opcode & 0x0F00) >> 8];
                            program_counter += 2;
                            break;

                        case 0x001E:
                            index_register += v_registers[(opcode & 0x0F00) >> 8];
                            program_counter += 2;
                            break;

                        case 0x0029:
                            index_register = (UInt16) (v_registers[(opcode & 0x0F00) >> 8] * 0x5);
                            program_counter += 2;
                            break;

                        case 0x0033:
                            memory[index_register] = (byte) (v_registers[(opcode & 0x0F00) >> 8] / 100);
                            memory[index_register + 1] = (byte) ((v_registers[(opcode & 0x0F00) >> 8] / 10) % 10);
                            memory[index_register + 2] = (byte) ((v_registers[(opcode & 0x0F00) >> 8] ) % 10);
                            program_counter += 2;
                            break;

                        case 0x0055:
                            for(int i = 0; i <= ((opcode & 0x0F00) >> 8); i++)
                            {
                                memory[index_register + i] = v_registers[i];
                            }
                            program_counter += 2;
                            break;

                        case 0x0065:
                            for (int i = 0; i <= ((opcode & 0x0F00) >> 8); i++)
                            {
                                v_registers[i] = memory[index_register + i];
                            }
                            program_counter += 2;
                            break;

                        default:
                            Console.WriteLine("Unknown opcode: 0x" + opcode.ToString("X4"));
                            break;
                    }
                    break;

                default:
                    Console.WriteLine("Unknown opcode: 0x" + opcode.ToString("X4"));
                    break;
            }
        }

        private void updateTimers()
        {
            if (delay_timer > 0)
            {
                delay_timer--;
            }
        }

        public void debugRender()
        {
            for (int y = 0; y < 32; ++y)
            {
                for (int x = 0; x < 64; ++x)
                {
                    if (pixels[(y * 64) + x] == 0)
                        Console.Write("O");
                    else
                        Console.Write(" ");
                }
                Console.Write("\n");
            }
            Console.Write("\n");
        }
    }
}
