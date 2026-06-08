using System;
using UnityEngine;

namespace PT.GameplayAdditional.Trigger
{
    public class TargetTrigger : MonoBehaviour
    {
        enum CollisionHandlingType { Trigger, Collider, Both }

        [SerializeField] private LayerMask collisionLayer;
        [SerializeField] private CollisionHandlingType collisionHandlingType;

        public event Action<CollisionInfo> OnTriggered;
        public event Action<CollisionInfo> OnExited;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if ((collisionHandlingType == CollisionHandlingType.Trigger || collisionHandlingType == CollisionHandlingType.Both) &&
                ((1 << collision.gameObject.layer) & collisionLayer.value) != 0)
            {
                var contactPoint = collision.ClosestPoint(transform.position);
                var fakeNormal = (contactPoint - (Vector2)transform.position).normalized;

                OnTriggered?.Invoke(new CollisionInfo(collision.gameObject, contactPoint, fakeNormal));
            }
        }
        
        private void OnTriggerExit2D(Collider2D collision)
        {
            if ((collisionHandlingType == CollisionHandlingType.Trigger || collisionHandlingType == CollisionHandlingType.Both) &&
                ((1 << collision.gameObject.layer) & collisionLayer.value) != 0)
            {
                var contactPoint = collision.ClosestPoint(transform.position);
                var fakeNormal = (contactPoint - (Vector2)transform.position).normalized;

                OnExited?.Invoke(new CollisionInfo(collision.gameObject, contactPoint, fakeNormal));
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if ((collisionHandlingType == CollisionHandlingType.Collider || collisionHandlingType == CollisionHandlingType.Both) &&
                ((1 << collision.gameObject.layer) & collisionLayer.value) != 0)
            {
                var contact = collision.GetContact(0);
                OnTriggered?.Invoke(new CollisionInfo(collision.gameObject, contact.point, contact.normal));
            }
        }
        
        private void OnCollisionExit2D(Collision2D collision)
        {
            if ((collisionHandlingType == CollisionHandlingType.Collider || collisionHandlingType == CollisionHandlingType.Both) &&
                ((1 << collision.gameObject.layer) & collisionLayer.value) != 0)
            {
                var contactPoint = (Vector2)collision.transform.position;
                var fakeNormal = (contactPoint - (Vector2)transform.position).normalized;

                OnExited?.Invoke(new CollisionInfo(collision.gameObject, contactPoint, fakeNormal));
            }
        }
    }

    public readonly struct CollisionInfo
    {
        public GameObject GameObject { get; }
        public Vector2 CollisionPoint { get; }
        public Vector2 Normal { get; }

        public CollisionInfo(GameObject gameObject, Vector2 collisionPoint, Vector2 normal)
        {
            GameObject = gameObject;
            CollisionPoint = collisionPoint;
            Normal = normal;
        }
    }
}