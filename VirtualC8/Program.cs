using System;
using SDL2;
using System.Collections.Generic;

namespace VirtualC8
{
    class Program
    {
        static void Main(string[] args)
        {
            Settings settings = new Settings();
            settings.LoadSettings();

            Dictionary<string, SDL.SDL_Keycode> keys = settings.convertKeys();

            SDL.SDL_SetHint(SDL.SDL_HINT_WINDOWS_DISABLE_THREAD_NAMING, "1");
            if (SDL.SDL_Init(SDL.SDL_INIT_VIDEO | SDL.SDL_INIT_AUDIO) < 0)
            {
                Console.WriteLine("Unable to initialize SDL. Error: {0}", SDL.SDL_GetError());
            }

            var window = IntPtr.Zero;
            var renderer = IntPtr.Zero;

            SDL.SDL_WindowFlags flags = SDL.SDL_WindowFlags.SDL_WINDOW_RESIZABLE;

            if (settings.fullscreen)
            {
                flags |= SDL.SDL_WindowFlags.SDL_WINDOW_FULLSCREEN_DESKTOP;
            }

            window = SDL.SDL_CreateWindow("Chip8-emu", SDL.SDL_WINDOWPOS_CENTERED, SDL.SDL_WINDOWPOS_CENTERED
                , settings.xresolution, settings.yresolution, flags);

            if (window == IntPtr.Zero)
            {
                Console.WriteLine("Unable to create a window. SDL. Error: {0}", SDL.SDL_GetError());
            }

            renderer = SDL.SDL_CreateRenderer(window, -1, SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED | SDL.SDL_RendererFlags.SDL_RENDERER_TARGETTEXTURE);

            if (renderer == IntPtr.Zero)
            {
                Console.WriteLine("Unable to create a renderer. SDL. Error: {0}", SDL.SDL_GetError());
            }

            SDL.SDL_Event e;
            bool quit = false;


            Chip8 chip8 = new Chip8();
            chip8.initialize(settings.chip8clock);

            chip8.loadGame(args[0]);

            Console.WriteLine();

            while (!quit)
            {
                while (SDL.SDL_PollEvent(out e) != 0)
                {
                    switch (e.type)
                    {
                        case SDL.SDL_EventType.SDL_QUIT:
                            quit = true;
                            break;

                        case SDL.SDL_EventType.SDL_KEYDOWN:
                            if (e.key.keysym.sym == SDL.SDL_Keycode.SDLK_ESCAPE)
                            {
                                quit = true;
                            }
                            else if (e.key.keysym.sym == keys["1"])
                            {
                                chip8.keys[0x1] = 1;
                            }
                            else if (e.key.keysym.sym == keys["2"])
                            {
                                chip8.keys[0x2] = 1;
                            }
                            else if (e.key.keysym.sym == keys["3"])
                            {
                                chip8.keys[0x3] = 1;
                            }
                            else if (e.key.keysym.sym == keys["C"])
                            {
                                chip8.keys[0xC] = 1;
                            }
                            else if (e.key.keysym.sym == keys["4"])
                            {
                                chip8.keys[0x4] = 1;
                            }
                            else if (e.key.keysym.sym == keys["5"])
                            {
                                chip8.keys[0x5] = 1;
                            }
                            else if (e.key.keysym.sym == keys["6"])
                            {
                                chip8.keys[0x6] = 1;
                            }
                            else if (e.key.keysym.sym == keys["D"])
                            {
                                chip8.keys[0xD] = 1;
                            }
                            else if (e.key.keysym.sym == keys["7"])
                            {
                                chip8.keys[0x7] = 1;
                            }
                            else if (e.key.keysym.sym == keys["8"])
                            {
                                chip8.keys[0x8] = 1;
                            }
                            else if (e.key.keysym.sym == keys["9"])
                            {
                                chip8.keys[0x9] = 1;
                            }
                            else if (e.key.keysym.sym == keys["E"])
                            {
                                chip8.keys[0xE] = 1;
                            }
                            else if (e.key.keysym.sym == keys["A"])
                            {
                                chip8.keys[0xA] = 1;
                            }
                            else if (e.key.keysym.sym == keys["0"])
                            {
                                chip8.keys[0x0] = 1;
                            }
                            else if (e.key.keysym.sym == keys["B"])
                            {
                                chip8.keys[0xB] = 1;
                            }
                            else if (e.key.keysym.sym == keys["F"])
                            {
                                chip8.keys[0xF] = 1;
                            }                            
                            break;

                        case SDL.SDL_EventType.SDL_KEYUP:
                            if (e.key.keysym.sym == SDL.SDL_Keycode.SDLK_ESCAPE)
                            {
                                quit = true;
                            }
                            else if (e.key.keysym.sym == keys["1"])
                            {
                                chip8.keys[0x1] = 0;
                            }
                            else if (e.key.keysym.sym == keys["2"])
                            {
                                chip8.keys[0x2] = 0;
                            }
                            else if (e.key.keysym.sym == keys["3"])
                            {
                                chip8.keys[0x3] = 0;
                            }
                            else if (e.key.keysym.sym == keys["C"])
                            {
                                chip8.keys[0xC] = 0;
                            }
                            else if (e.key.keysym.sym == keys["4"])
                            {
                                chip8.keys[0x4] = 0;
                            }
                            else if (e.key.keysym.sym == keys["5"])
                            {
                                chip8.keys[0x5] = 0;
                            }
                            else if (e.key.keysym.sym == keys["6"])
                            {
                                chip8.keys[0x6] = 0;
                            }
                            else if (e.key.keysym.sym == keys["D"])
                            {
                                chip8.keys[0xD] = 0;
                            }
                            else if (e.key.keysym.sym == keys["7"])
                            {
                                chip8.keys[0x7] = 0;
                            }
                            else if (e.key.keysym.sym == keys["8"])
                            {
                                chip8.keys[0x8] = 0;
                            }
                            else if (e.key.keysym.sym == keys["9"])
                            {
                                chip8.keys[0x9] = 0;
                            }
                            else if (e.key.keysym.sym == keys["E"])
                            {
                                chip8.keys[0xE] = 0;
                            }
                            else if (e.key.keysym.sym == keys["A"])
                            {
                                chip8.keys[0xA] = 0;
                            }
                            else if (e.key.keysym.sym == keys["0"])
                            {
                                chip8.keys[0x0] = 0;
                            }
                            else if (e.key.keysym.sym == keys["B"])
                            {
                                chip8.keys[0xB] = 0;
                            }
                            else if (e.key.keysym.sym == keys["F"])
                            {
                                chip8.keys[0xF] = 0;
                            }
                            break;
                    }
                }

                chip8.update();

                if (chip8.draw_flag)
                {
                    IntPtr texture = SDL.SDL_CreateTexture(renderer, SDL.SDL_PIXELFORMAT_RGBA8888, 2, 64, 32);

                    SDL.SDL_SetRenderTarget(renderer, texture);
                    SDL.SDL_SetRenderDrawColor(renderer, 255, 255, 255, 255);

                    for (int i = 0; i < chip8.pixels.Length; i++)
                    {
                        int y = i / 64;
                        int x = i - 64 * y;
                        if (chip8.pixels[i] == 1)
                        {
                            SDL.SDL_RenderDrawPoint(renderer, x, y);
                        }
                    }

                    chip8.draw_flag = false;

                    SDL.SDL_SetRenderTarget(renderer, IntPtr.Zero);
                    SDL.SDL_RenderCopyEx(renderer, texture, IntPtr.Zero, IntPtr.Zero,
                        0, IntPtr.Zero, SDL.SDL_RendererFlip.SDL_FLIP_NONE);

                    SDL.SDL_RenderPresent(renderer);

                    SDL.SDL_DestroyTexture(texture);
                }

                if (chip8.sound_duration > 0)
                {
                    Console.WriteLine("Beep");
                    //Console.Beep(500, chip8.sound_duration * 1000 / 60);
                    chip8.sound_duration = 0;
                }
            }

            SDL.SDL_DestroyWindow(renderer);
            SDL.SDL_DestroyWindow(window);
            SDL.SDL_Quit();
        }
    }
}