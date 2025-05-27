using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyFarm.Inventory
{
    public class ItemBounce : MonoBehaviour
    {
        private Transform spriteTrans;
        private BoxCollider2D coll;
        private bool isGround;
        private float distance;
        private Vector2 direction;
        private Vector3 targetPos;

        private void Awake()
        {
            spriteTrans = transform.GetChild(0);
            coll = GetComponent<BoxCollider2D>();
            coll.enabled = false;
        }

        private void Update()
        {
            Bounce();
        }

        public void InitBounceItem(Vector3 targetPos, Vector2 direction)
        {
            coll.enabled = false;
            this.direction = direction;
            this.targetPos = targetPos;
            distance = Vector3.Distance(targetPos,transform.position);
            
            spriteTrans.position += Vector3.up * Settings.holdHeight;
        }

        private void Bounce()
        {
            isGround = spriteTrans.position.y <= transform.position.y;//判断和影子的y是否重合
            if (Vector3.Distance(transform.position,targetPos)>0.1f)
            {
                transform.position += (Vector3)direction*distance*Time.deltaTime*-Settings.fallGravity;                
            }
            if (!isGround)
            {
                spriteTrans.position+=Vector3.up * Settings.fallGravity*Time.deltaTime;
            }
            else
            {
                spriteTrans.position=transform.position;
                coll.enabled = true;
            }
        }
    }
}

