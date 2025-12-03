using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Encodings.Web;
using MonoGameGum;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameGum.Forms.Controls;
using System.Runtime.InteropServices;
using FlatRedBall;
using Monolith.Input;

namespace Monolith.UI.GUM
{
    public class GumHelper
    {
        
        public static Dictionary<string, FrameworkElement> ScreenDictionary = new();

        public static void AddScreenToRoot(FrameworkElement newScreen)
        {
            string key = newScreen.ToString();
            
            if (ScreenDictionary.ContainsKey(key))
            {
                FrameworkElement oldScreen = ScreenDictionary[key];
                
                oldScreen.RemoveFromRoot(); 
                
                ScreenDictionary[key] = newScreen;
            }
            else
            {
                ScreenDictionary.Add(key, newScreen);
            }

            newScreen.AddToRoot();

        }

        public static GumService GumInitialize(Game game, string path)
        {
            GumService.Default.Initialize(game, path);

            FrameworkElement.KeyboardsForUiControl.Add(GumService.Default.Keyboard);
            FrameworkElement.GamePadsForUiControl.AddRange(GumService.Default.Gamepads);
            FrameworkElement.TabReverseKeyCombos.Add(
                new KeyCombo() { PushedKey = Microsoft.Xna.Framework.Input.Keys.Up });
            FrameworkElement.TabKeyCombos.Add(
                new KeyCombo() { PushedKey = Microsoft.Xna.Framework.Input.Keys.Down });

            return GumService.Default;
        }

        
        
        public static void RemoveScreenOfType<T>() where T : FrameworkElement
        {
            var screen = ScreenDictionary.Values.FirstOrDefault(s => s is T);
            if (screen != null)
            {
                RemoveScreenFromRoot(screen);
            }
        }

        public static void RemoveScreenFromRoot(FrameworkElement Screen)
        {
            Screen.RemoveFromRoot();
            ScreenDictionary.Remove(ScreenDictionary.First(kvp => kvp.Value == Screen).Key);
        }

        public static void Wipe()
        {
            foreach (var screen in ScreenDictionary.Values)
            {
                screen.RemoveFromRoot();
                ScreenDictionary.Remove(screen.ToString());
            }
        }


        public static void UpdateScreenLayout()
        {
            foreach (FrameworkElement screen in ScreenDictionary.Values)
            {
                screen.Visual.UpdateLayout();
            }
        }
    }
}