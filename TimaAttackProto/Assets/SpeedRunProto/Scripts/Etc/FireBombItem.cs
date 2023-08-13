using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using System;
using MoreMountains.CorgiEngine;

namespace MoreMountains.InventoryEngine
{
    [Serializable]
    public class FireBombItem : InventoryItem
    {
        public float throwForce = 3f; // 폭탄을 던질 힘
        public override bool Use(string playerID)
        {
            base.Use(playerID);

            GameObject player = GameObject.Find(playerID);
            if (player != null)
            {
                GameObject bomb = player.GetComponent<MMSimpleObjectPooler>().GetPooledGameObject();
                if (bomb != null)
                {
                    bomb.transform.position = player.transform.position;
                    bomb.SetActive(true);

                    bool facingRight = player.GetComponent<Character>().IsFacingRight;
                    Vector2 direction = facingRight ? Quaternion.Euler(0, 0, 40) * Vector2.right : Quaternion.Euler(0, 0, -40) * Vector2.left;

                    Rigidbody2D rb = bomb.GetComponent<Rigidbody2D>();
                    if (rb != null)
                    {
                        rb.AddForce(direction * throwForce, ForceMode2D.Impulse);
                    }
                }
            }
            return true;
        }

    }
}
