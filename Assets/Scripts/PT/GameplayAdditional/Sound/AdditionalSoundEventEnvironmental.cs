using NaughtyAttributes;
using PT.Tools.Helper;
using UnityEngine;

namespace PT.GameplayAdditional.Sound
{
    public class AdditionalSoundEventEnvironmental : AdditionalSoundEvent
    {
        [SerializeField][MinMaxSlider(4, 20)] private Vector2 distanceFromSource = new(10, 10);

        [SerializeField] private Transform soundSourceTransform;

        protected override void PlaySound()
        {
            var randomDirection = Utils.GetRandomDirection();
            randomDirection.y = 0;
            var randomPosition = soundSourceTransform.position + (randomDirection * Utils.GetRandomValue(distanceFromSource));

            _audioManager.PlayOneShot(Utils.GetRandomElement(soundEventEnumsToPlay), randomPosition);
        }
    }
}
