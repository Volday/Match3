using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Match3.Core
{
    public static class GameLoop
    {
        public static bool isActive { private set; get; } = false;

        private enum State { Update, Render, StartFrame }
        private static State state = State.StartFrame;

        private static Dictionary<Guid, GameObject> gameObjects = new Dictionary<Guid, GameObject>();
        private static Dictionary<Type, Stack<GameObject>> objectPool = new Dictionary<Type, Stack<GameObject>>();

        private static List<GameObject> renderers = new List<GameObject>();

        private static DateTime loopStartTime;
        private static float deltaTime = 0;

        public static void Restart()
        {
            Stop();

            state = State.Update;
            isActive = true;
            loopStartTime = DateTime.UtcNow;

            Loop();
        }

        public static void Stop()
        {
            isActive = false;

            foreach (var gameObject in gameObjects)
            {
                GameObject.Destroy(gameObject.Value);
            }
        }

        private static async void Loop()
        {
            while (isActive)
            {
                switch (state)
                {
                    case State.StartFrame:
                        state = State.Update;
                        await StartFrame();
                        break;
                    case State.Update:
                        Update();
                        state = State.Render;
                        break;
                    case State.Render:
                        state = State.StartFrame;
                        Render();
                        break;
                    default: break;
                }
            }
        }

        private static void Update()
        {
            var gameObjectsCopy = gameObjects.ToList();
            foreach (var gameObject in gameObjectsCopy)
            {
                if (gameObject.Value != null)
                {
                    gameObject.Value.Update(deltaTime);
                }
            }
        }

        private static void Render()
        {
            foreach (var gameObject in renderers)
            {
                var renderer = gameObject;
                renderer.Render();
            }
        }

        private async static Task StartFrame()
        {
            deltaTime = (float)(DateTime.UtcNow - loopStartTime).TotalSeconds;
            loopStartTime = DateTime.UtcNow;
            await Task.Delay(10);
        }

        public static void RegisterGameObject(GameObject gameObject)
        {
            if (gameObject == null) return;
            if (!gameObjects.ContainsKey(gameObject.guid))
            {
                gameObjects.Add(gameObject.guid, gameObject);
                gameObject.OnCreate();
                AddRenderer(gameObject);
            }
        }

        private static void AddRenderer(GameObject gameObject)
        {
            if (gameObject != null)
            {
                var renderPriority = gameObject.renderPriority;
                var index = 0;
                for (int i = 0; i < renderers.Count; i++)
                {
                    if ((renderers[i]).renderPriority >= renderPriority)
                    { 
                        index = i;
                        break;
                    }
                }
                renderers.Insert(index, gameObject);
                gameObject.SetVisibility(true);
            }
        }

        private static void RemoveRenderer(GameObject gameObject)
        {
            var renderer = gameObject;
            renderer.SetVisibility(false);
            renderers.Remove(gameObject);
        }

        public static void DeactivateGameObject(GameObject gameObject)
        {
            if (gameObject == null) return;
            if (!gameObjects.ContainsKey(gameObject.guid)) return;

            gameObjects.Remove(gameObject.guid);
            
            var type = gameObject.GetType();
            if (!objectPool.ContainsKey(type))
            {
                objectPool.Add(type, new Stack<GameObject>());
            }

            objectPool[type].Push(gameObject);
            RemoveRenderer(gameObject);
        }

        public static T ActivateGameObject<T>() where T : GameObject
        {
            var type = typeof(T);
            if (!objectPool.ContainsKey(type)) return null;

            if (objectPool[type].Count == 0) return null;

            var gameObject = objectPool[type].Pop();
            if (gameObject == null) return null;

            gameObjects.Add(gameObject.guid, gameObject);
            gameObject.OnCreate();
            AddRenderer(gameObject);
            return (T)gameObject;
        }
    }
}
