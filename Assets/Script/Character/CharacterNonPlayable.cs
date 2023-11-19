/// <author>Thoams Krahl</author>

using UnityEngine;
using UnityEngine.Pool;

namespace ProjectGTA2_Unity.Characters
{
    public class CharacterNonPlayable : Character
    {
        [Header("NPC")]
        [SerializeField] protected NonPlayableBehaviour npcBehaviour;
        private ObjectPool<CharacterNonPlayable> objectPool;

        protected override void StartSetup()
        {
            base.StartSetup();
            SetShirtColor();
        }

        protected override void OnEnbaleSetup()
        {
            SetShirtColor();
        }

        protected override void Death()
        {
            npcBehaviour.IsDead = true;
            base.Death();
            //Destroy(gameObject, deletionTime);
        }

        private void SetShirtColor()
        {
            Material mat = spriteRenderer.material;
            Color color = RandomShirtColor();

            mat.SetColor("_ColorShirt1", color);
            mat.SetColor("_ColorShirt2", color);
        }

        private Color RandomShirtColor()
        {
            float r = Util.RandomFloatNumber(0.0f, 1.0f);
            float g = Util.RandomFloatNumber(0.0f, 1.0f);
            float b = Util.RandomFloatNumber(0.0f, 1.0f);

            Color newShirtColor = new Color(r, g, b);
            return newShirtColor;
        }
        

        public void SetPool(ObjectPool<CharacterNonPlayable> pool)
        {
            objectPool = pool;
        }
    }
}

