using UnityEngine;

namespace DungeonCrawler
{
    public abstract class Reference<T> : ScriptableObject
    {
        private T myObject;
        
        public T GetObject()
        {
            return myObject;
        }

        public void SetObject(T newObject)
        {
            
        }
    }
}