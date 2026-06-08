using UnityEngine;

namespace PT.Tools.Effects.SAP
{
    public class SpriteRendererProxy : MonoBehaviour, ISpriteRendererProxy
    {
        [SerializeField] private SpriteRenderer spriteRenderer;

        public Sprite Sprite
        {
            get => spriteRenderer.sprite;
            set => spriteRenderer.sprite = value;
        }
    }
}