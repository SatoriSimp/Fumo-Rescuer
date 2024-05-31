using System.Collections;
using UnityEngine;

namespace Assets.Enemy_Artillery
{
    public class ArtilleryProjectileScript
        : MonoBehaviour
    {
        public Transform PlayerPosition;

        private void Start()
        {
            Destroy(gameObject, 1);
        }

        // Update is called once per frame
        void Update()
        {
            transform.position = Vector2.MoveTowards(transform.position,
                new Vector2(transform.position.x, transform.position.y - 150),
                Vector2.Distance(transform.position, PlayerPosition.position)
            );
        }
    }
}