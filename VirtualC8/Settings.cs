using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using SDL2;

namespace VirtualC8
{
    class Settings
    {
        public int xresolution;
        public int yresolution;
        public int chip8clock;
        public bool fullscreen;
        public Dictionary<string, string> keys;

        public Settings()
        {
            this.xresolution = 640;
            this.yresolution = 320;
            this.chip8clock = 1000;
            this.fullscreen = false;
            this.keys = new Dictionary<string, string>();
            keys["1"] = "1";
            keys["2"] = "2";
            keys["3"] = "3";
            keys["C"] = "4";
            keys["4"] = "q";
            keys["5"] = "w";
            keys["6"] = "e";
            keys["D"] = "r";
            keys["7"] = "a";
            keys["8"] = "s";
            keys["9"] = "d";
            keys["E"] = "f";
            keys["A"] = "z";
            keys["0"] = "x";
            keys["B"] = "c";
            keys["F"] = "v";
        }

        public bool LoadSettings()
        {
            if (File.Exists("Settings.json"))
            {
                string jsonString = File.ReadAllText("Settings.json");
                Settings settings = JsonConvert.DeserializeObject<Settings>(jsonString);
                this.xresolution = settings.xresolution;
                this.yresolution = settings.yresolution;
                this.chip8clock = settings.chip8clock;
                this.fullscreen = settings.fullscreen;
                this.keys = settings.keys;
                return true;
            } else
            {
                string jsonString = JsonConvert.SerializeObject(this, Formatting.Indented);
                File.WriteAllText("Settings.Json", jsonString);
                return false;
            }
        }

        public Dictionary<string, SDL.SDL_Keycode> convertKeys()
        {
            Dictionary<string, SDL.SDL_Keycode> sdlKeys = new Dictionary<string, SDL.SDL_Keycode>();
            sdlKeys["1"] = SDL.SDL_GetKeyFromName(this.keys["1"]);
            sdlKeys["2"] = SDL.SDL_GetKeyFromName(this.keys["2"]);
            sdlKeys["3"] = SDL.SDL_GetKeyFromName(this.keys["3"]);
            sdlKeys["C"] = SDL.SDL_GetKeyFromName(this.keys["C"]);
            sdlKeys["4"] = SDL.SDL_GetKeyFromName(this.keys["4"]);
            sdlKeys["5"] = SDL.SDL_GetKeyFromName(this.keys["5"]);
            sdlKeys["6"] = SDL.SDL_GetKeyFromName(this.keys["6"]);
            sdlKeys["D"] = SDL.SDL_GetKeyFromName(this.keys["D"]);
            sdlKeys["7"] = SDL.SDL_GetKeyFromName(this.keys["7"]);
            sdlKeys["8"] = SDL.SDL_GetKeyFromName(this.keys["8"]);
            sdlKeys["9"] = SDL.SDL_GetKeyFromName(this.keys["9"]);
            sdlKeys["E"] = SDL.SDL_GetKeyFromName(this.keys["E"]);
            sdlKeys["A"] = SDL.SDL_GetKeyFromName(this.keys["A"]);
            sdlKeys["0"] = SDL.SDL_GetKeyFromName(this.keys["0"]);
            sdlKeys["B"] = SDL.SDL_GetKeyFromName(this.keys["B"]);
            sdlKeys["F"] = SDL.SDL_GetKeyFromName(this.keys["F"]);
            return sdlKeys;
        }
    }
}
