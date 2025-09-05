using SDL3;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;

namespace VirtualC8
{
    class Program
    {
        const int TARGET_FPS = 60;
        const double TARGET_FRAME_TIME_MS = 1000.0 / TARGET_FPS;
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

            SDL.SetHint(SDL.Hints.RenderDriver, "direct3d12,vulkan,metal,opengl");

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

            chip8.loadGame("INVADERS");

            Console.WriteLine();

            IntPtr texture = SDL.CreateTexture(renderer, SDL.PixelFormat.RGBA8888, SDL.TextureAccess.Target, 64, 32);
            SDL.SetTextureScaleMode(texture, SDL.ScaleMode.Nearest);

            ulong perfFrequency = SDL.GetPerformanceFrequency();
            ulong lastFrameStart = SDL.GetPerformanceCounter();

            Dictionary<SDL.Keycode, int> chip8KeyMap = new()
            {
                { keys["1"], 0x1 },
                { keys["2"], 0x2 },
                { keys["3"], 0x3 },
                { keys["C"], 0xC },
                { keys["4"], 0x4 },
                { keys["5"], 0x5 },
                { keys["6"], 0x6 },
                { keys["D"], 0xD },
                { keys["7"], 0x7 },
                { keys["8"], 0x8 },
                { keys["9"], 0x9 },
                { keys["E"], 0xE },
                { keys["A"], 0xA },
                { keys["0"], 0x0 },
                { keys["B"], 0xB },
                { keys["F"], 0xF },
            };

            while (!quit)
            {
                ulong frameStart = SDL.GetPerformanceCounter();
                // Calculate time since last frame in milliseconds
                double deltaTimeMs = (frameStart - lastFrameStart) / (double)perfFrequency * 1000.0;
                lastFrameStart = frameStart;

                while (SDL.PollEvent(out e))
                {
                    switch (e.Type)
                    {
                        case (uint) SDL.EventType.Quit:
                            quit = true;
                            break;
                        case (uint) SDL.EventType.KeyDown:
                        case (uint) SDL.EventType.KeyUp:
                            if (e.Key.Key == SDL.Keycode.Escape)
                            {
                                quit = true;
                            }
                            if (chip8KeyMap.TryGetValue(e.Key.Key, out int chip8Index))
                            {
                                byte state = (e.Type == (uint) SDL.EventType.KeyDown) ? (byte)1 : (byte)0;
                                chip8.keys[chip8Index] = state;
                            }
                            break;
                    }
                }

                chip8.update(deltaTimeMs);

                if (chip8.draw_flag)
                {
                    uint[] pixelBuffer = new uint[64 * 32];

                    const uint onColor = 0xFFFFFFFF;  // White
                    const uint offColor = 0xFF000000; // Black

                    for (int i = 0; i < chip8.pixels.Length; i++)
                    {
                        pixelBuffer[i] = (chip8.pixels[i] == 1) ? onColor : offColor;
                    }

                    GCHandle handle = default;
                    try
                    {
                        handle = GCHandle.Alloc(pixelBuffer, GCHandleType.Pinned);
                        IntPtr pPixels = handle.AddrOfPinnedObject();

                        SDL.UpdateTexture(texture, IntPtr.Zero, pPixels, 64 * sizeof(uint));
                    }
                    finally
                    {
                        if (handle.IsAllocated)
                        {
                            handle.Free();
                        }
                    }

                    chip8.draw_flag = false;
                }

                if (chip8.sound_duration > 0)
                {
                    Console.WriteLine("Beep");
                    //Console.Beep(500, chip8.sound_duration * 1000 / 60);
                    chip8.sound_duration = 0;
                }

                SDL.RenderClear(renderer);
                SDL.RenderTexture(renderer, texture, IntPtr.Zero, IntPtr.Zero);
                SDL.RenderPresent(renderer);

                ulong frameEnd = SDL.GetPerformanceCounter();
                double elapsedMS = (frameEnd - frameStart) / (double)perfFrequency * 1000.0;

                if (elapsedMS < TARGET_FRAME_TIME_MS)
                {
                    SDL.Delay((uint)(TARGET_FRAME_TIME_MS - elapsedMS));
                }
            }

            SDL.DestroyTexture(texture);
            SDL.DestroyRenderer(renderer);
            SDL.DestroyWindow(window);
            SDL.Quit();
        }
    }
}