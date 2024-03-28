using UnityEngine;

namespace FistOfTheFree.Guns.Demo
// class for destroying game objects. very clean way to do it, without making unity bug out.
{
    public class DeathDestroyCallback : MonoBehaviour
    {
        public void DeathEnd()
        {
            Destroy(gameObject);
        }
    }
}