using System;
using System.Linq;
using System.Reflection;
using ConstructEngine.Objects;
using ConstructEngine.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace ConstructEngine.Objects
{
    public class SceneAreaTransition : ConstructObject, IObject
    {
        public Scene.IScene Scene { get; set; }

        private bool resetScene = false;


        public SceneAreaTransition()
        {

        }

        public override void Load()
        {
            if (Values.ContainsKey("ResetScene"))
            {
                if (Values["ResetScene"] as bool? == true)
                {
                    resetScene = true;
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (Rectangle != null)
            {
                if (Rectangle.Intersects(Player.KinematicBase.Collider.Rect))
                {
                    if (resetScene)
                    {
                        CurrentSceneManager.ReloadCurrentScene();
                    }
                    else
                    {
                        CurrentSceneManager.AddScene(ChangeSceneWithAssembly());
                    }
                }
            }
        }

        private Scene.IScene ChangeSceneWithAssembly()
        {
            if (Values.ContainsKey("Scene"))
            {
                string className = Values["Scene"] as string;

                Assembly assembly = AppDomain.CurrentDomain.GetAssemblies()
                    .FirstOrDefault(a =>
                        a.GetTypes().Any(t => t.Name == className && typeof(Scene.IScene).IsAssignableFrom(t)));


                if (assembly != null)
                {


                    Type type = assembly.GetTypes()
                        .First(t => t.Name == className && typeof(Scene.IScene).IsAssignableFrom(t));



                    Scene.IScene instance = (Scene.IScene)Activator.CreateInstance(type, CurrentSceneManager);

                    return instance;

                }

            
            }
            return null;
        }
    }
}