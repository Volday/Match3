using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Match3.Core
{
    public class GameObject
    {
        public Guid guid;

        public Vector2 size;
        private Vector2 position;
        public Vector2 Position
        {
            get
            {
                return position;
            }
            set
            { 
                var delta = value - position;
                foreach (var child in childrens)
                {
                    child.OnParentPositionChanged(delta);
                }
                position = value;
            }
        }

        public List<GameObject> childrens = new List<GameObject>();

        public int renderPriority;

        protected GameObject() 
        {
            guid = Guid.NewGuid();
        }

        public static T GetGameObject<T>() where T : GameObject, new()
        {
            GameObject gameobjct;
            gameobjct = GameLoop.ActivateGameObject<T>();

            if (gameobjct == null)
            {
                gameobjct = new T();
                GameLoop.RegisterGameObject(gameobjct);
            }

            return (T)gameobjct;
        }

        public static void Destroy(GameObject gameObject)
        {
            gameObject.childrens.Clear();
            GameLoop.DeactivateGameObject(gameObject);
            gameObject.OnDestroy();
        }

        public virtual void OnParentPositionChanged(Vector2 delta) { }

        public virtual void OnCreate() { }
        public virtual void Update(float deltaTime) { }
        public virtual void Render() { }
        public virtual void OnDestroy() { }

        public virtual void SetVisibility(bool value) { }
    }
}
