using UnityEngine;

namespace Gameplay.DragDropPuzzle
{
    public class ParticleEffectPlayer : MonoBehaviour
    {
        [SerializeField] private ParticleSystem successParticle;

        public void PlaySuccessEffect(Vector3 position)
        {
            ParticleSystem instance = Instantiate(successParticle, position, Quaternion.identity);
        }
    }
}
