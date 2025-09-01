using System;
using SDL3;
using System.Collections.Generic;

namespace VirtualC8
{
    class Program
    {
        static void Main(string[] args)
        {
            Settings settings = new Settings();
            settings.LoadSettings();

            Dictionary<string, SDL.Keycode> keys = settings.convertKeys();

            if (!SDL.Init(SDL.InitFlags.Video | SDL.InitFlags.Audio))
            {
                Console.WriteLine("Unable to initialize SDL. Error: {0}", SDL.GetError());
            }

            var window = IntPtr.Zero;
            var renderer = IntPtr.Zero;

            SDL.WindowFlags flags = SDL.WindowFlags.Resizable;

            if (settings.fullscreen)
            {
                flags |= SDL.WindowFlags.Fullscreen;
            }

            window = SDL.CreateWindow("Chip8-emu", settings.xresolution, settings.yresolution, flags);

            if (window == IntPtr.Zero)
            {
                Console.WriteLine("Unable to create a window. SDL. Error: {0}", SDL.GetError());
            }

            renderer = SDL.CreateRenderer(window, null);

            if (renderer == IntPtr.Zero)
            {
                Console.WriteLine("Unable to create a renderer. SDL. Error: {0}", SDL.GetError());
            }

            SDL.Event e;
            bool quit = false;


            Chip8 chip8 = new Chip8();
            chip8.initialize(settings.chip8clock);

            chip8.loadGame(args[0]);

            Console.WriteLine();

            IntPtr texture = SDL.CreateTexture(renderer, SDL.PixelFormat.RGBA8888, SDL.TextureAccess.Target, 64, 32);
            SDL.SetTextureScaleMode(texture, SDL.ScaleMode.Nearest);

            while (!quit)
            {
                while (SDL.PollEvent(out e))
                {
                    switch (e.Type)
                    {
                        case (uint) SDL.EventType.Quit:
                            quit = true;
                            break;

                        case (uint) SDL.EventType.KeyDown:
                            if (e.Key.Key == SDL.Keycode.Escape)
                            {
                                quit = true;
                            }
                            else if (e.Key.Key == keys["1"])
                            {
                                chip8.keys[0x1] = 1;
                            }
                            else if (e.Key.Key == keys["2"])
                            {
                                chip8.keys[0x2] = 1;
                            }
                            else if (e.Key.Key == keys["3"])
                            {
                                chip8.keys[0x3] = 1;
                            }
                            else if (e.Key.Key == keys["C"])
                            {
                                chip8.keys[0xC] = 1;
                            }
                            else if (e.Key.Key == keys["4"])
                            {
                                chip8.keys[0x4] = 1;
                            }
                            else if (e.Key.Key == keys["5"])
                            {
                                chip8.keys[0x5] = 1;
                            }
                            else if (e.Key.Key == keys["6"])
                            {
                                chip8.keys[0x6] = 1;
                            }
                            else if (e.Key.Key == keys["D"])
                            {
                                chip8.keys[0xD] = 1;
                            }
                            else if (e.Key.Key == keys["7"])
                            {
                                chip8.keys[0x7] = 1;
                            }
                            else if (e.Key.Key == keys["8"])
                            {
                                chip8.keys[0x8] = 1;
                            }
                            else if (e.Key.Key == keys["9"])
                            {
                                chip8.keys[0x9] = 1;
                            }
                            else if (e.Key.Key == keys["E"])
                            {
                                chip8.keys[0xE] = 1;
                            }
                            else if (e.Key.Key == keys["A"])
                            {
                                chip8.keys[0xA] = 1;
                            }
                            else if (e.Key.Key == keys["0"])
                            {
                                chip8.keys[0x0] = 1;
                            }
                            else if (e.Key.Key == keys["B"])
                            {
                                chip8.keys[0xB] = 1;
                            }
                            else if (e.Key.Key == keys["F"])
                            {
                                chip8.keys[0xF] = 1;
                            }                            
                            break;

                        case (uint) SDL.EventType.KeyUp:
                            if (e.Key.Key == SDL.Keycode.Escape)
                            {
                                quit = true;
                            }
                            else if (e.Key.Key == keys["1"])
                            {
                                chip8.keys[0x1] = 0;
                            }
                            else if (e.Key.Key == keys["2"])
                            {
                                chip8.keys[0x2] = 0;
                            }
                            else if (e.Key.Key == keys["3"])
                            {
                                chip8.keys[0x3] = 0;
                            }
                            else if (e.Key.Key == keys["C"])
                            {
                                chip8.keys[0xC] = 0;
                            }
                            else if (e.Key.Key == keys["4"])
                            {
                                chip8.keys[0x4] = 0;
                            }
                            else if (e.Key.Key == keys["5"])
                            {
                                chip8.keys[0x5] = 0;
                            }
                            else if (e.Key.Key == keys["6"])
                            {
                                chip8.keys[0x6] = 0;
                            }
                            else if (e.Key.Key == keys["D"])
                            {
                                chip8.keys[0xD] = 0;
                            }
                            else if (e.Key.Key == keys["7"])
                            {
                                chip8.keys[0x7] = 0;
                            }
                            else if (e.Key.Key == keys["8"])
                            {
                                chip8.keys[0x8] = 0;
                            }
                            else if (e.Key.Key == keys["9"])
                            {
                                chip8.keys[0x9] = 0;
                            }
                            else if (e.Key.Key == keys["E"])
                            {
                                chip8.keys[0xE] = 0;
                            }
                            else if (e.Key.Key == keys["A"])
                            {
                                chip8.keys[0xA] = 0;
                            }
                            else if (e.Key.Key == keys["0"])
                            {
                                chip8.keys[0x0] = 0;
                            }
                            else if (e.Key.Key == keys["B"])
                            {
                                chip8.keys[0xB] = 0;
                            }
                            else if (e.Key.Key == keys["F"])
                            {
                                chip8.keys[0xF] = 0;
                            }
                            break;
                    }
                }

                chip8.update();

                if (chip8.draw_flag)
                {
                    SDL.SetRenderTarget(renderer, texture);

                    SDL.SetRenderDrawColor(renderer, 0, 0, 0, 255);
                    SDL.RenderClear(renderer);

                    SDL.SetRenderDrawColor(renderer, 255, 255, 255, 255);
                    for (int i = 0; i < chip8.pixels.Length; i++)
                    {
                        int y = i / 64;
                        int x = i - 64 * y;
                        if (chip8.pixels[i] == 1)
                        {
                            SDL.RenderPoint(renderer, x, y);
                        }
                    }

                    chip8.draw_flag = false;

                    SDL.SetRenderTarget(renderer, IntPtr.Zero);

                    SDL.RenderTexture(renderer, texture, IntPtr.Zero, IntPtr.Zero);

                    SDL.RenderPresent(renderer);
                }

                if (chip8.sound_duration > 0)
                {
                    Console.WriteLine("Beep");
                    //Console.Beep(500, chip8.sound_duration * 1000 / 60);
                    chip8.sound_duration = 0;
                }
            }

            SDL.DestroyTexture(texture);
            SDL.DestroyRenderer(renderer);
            SDL.DestroyWindow(window);
            SDL.Quit();
        }
    }
}