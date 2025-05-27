using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishBounce : MonoBehaviour
{
        private Transform spriteTrans;//鱼漂
        private bool isGround;
        private Vector2 direction;
        private Vector3 targetPos;
        private Vector3 startPos;
        private float Length;
        public AnimationCurve curve;   // 自定义运动曲线
        private float peakHeight;      // 最高点高度
        private float elapsedTime;
        private float duration;    // 持续时间
        private float currentVerticalSpeed; // 当前垂直速度
        private Vector3 initialVelocity;

        private Transform playerTransform => FindObjectOfType<Player>().transform;
        private void Awake()
        {
            spriteTrans = transform.GetChild(0);
        }
        private void Update()
        {
            if (!isGround)
            {
                ApplyPhysicsMovement();
                RotateFloating();
                CheckGround();
            }
           
        }
   
        public void InitBounceItem(float posX, float posY,float sliderValue)
        {
            // 方向归一化
            direction = new Vector2(
                posX != 0 ? Mathf.Sign(posX) : 0,
                posY != 0 ? Mathf.Sign(posY) : 0
            );
            Debug.Log("posX"+posX+"Mathf.Sign(posX)"+Mathf.Sign(posX));
            Debug.Log("direction"+direction);
            // 根据方向设置持续时间
            duration = direction.x == 0 ? 
                Mathf.Lerp(0.5f, 1.0f, sliderValue) : 
                Mathf.Lerp(0.2f, 0.5f, sliderValue);
            Debug.Log("duration"+duration);
            elapsedTime = 0f;
            CalculatePhysicsParameters(sliderValue);
        }
        private void CalculatePhysicsParameters(float sliderValue)
        {
            //出生在玩家头顶
            targetPos = playerTransform.position + (Vector3)direction * sliderValue * Settings.floatingDistance;//目标位置
            Debug.Log("targetPos "+targetPos );
            spriteTrans.position = playerTransform.position + Vector3.up * 2f;//鱼漂初始位置
            transform.position = playerTransform.position;
            // 计算高度参数
            startPos = new Vector3(playerTransform.position.x, spriteTrans.position.y, playerTransform.position.z); 
            CalculateInitialVelocity();
          
        }
        private void ApplyPhysicsMovement()
        {
            elapsedTime += Time.deltaTime;
            //transform.position=  Vector3.MoveTowards(transform.position, targetPos, Settings.speed * Time.deltaTime);
            float xPos = initialVelocity.x * elapsedTime;
            float yPos = initialVelocity.y * elapsedTime - 0.5f * Settings.fallAcceleration * elapsedTime * elapsedTime;
            // 如果有水平方向的移动
            if (direction.x != 0)
            {
                spriteTrans.position = startPos + new Vector3(direction.x * xPos, yPos, 0);
            }
            else// 如果只是竖直方向的移动
            {
                spriteTrans.position = startPos + Vector3.up * yPos;
            }
            
            transform.position=  Vector3.MoveTowards(transform.position, targetPos, initialVelocity.x * Time.deltaTime);
            /*Debug.Log("运动中transform.position"+transform.position);*/
        }
        void CalculateInitialVelocity()
        {
            peakHeight = (Settings.fallAcceleration * duration * duration) / 8f;
            float verticalVelocity = Mathf.Sqrt(2 * Settings.fallAcceleration * peakHeight);
            Debug.Log("verticalVelocity"+verticalVelocity);
            float horizontalVelocity = (transform.position- targetPos).magnitude / duration;
            Debug.Log("horizontalVelocity"+horizontalVelocity);
            initialVelocity = new Vector3(horizontalVelocity, verticalVelocity, 0);
            // 验证总时间是否匹配
            float riseTime = verticalVelocity / Settings.fallAcceleration;
            float fallTime = Mathf.Sqrt(2 * peakHeight / Settings.fallAcceleration);
            float totalTime = riseTime + fallTime;
        
            if (!Mathf.Approximately(totalTime, duration))
            {
                Debug.LogWarning($"计算出的总时间({totalTime})与设定的duration({duration})不匹配。可能需要调整参数");
            }
        }
        
        private void RotateFloating()
        {
            // 简单计算当前应该旋转的角度(随时间均匀旋转两圈)
            float currentRotation = (elapsedTime / duration) * Settings.totalRotation;
            spriteTrans.rotation = Quaternion.Euler(0, 0, currentRotation);
        }
 
        private void CheckGround()
        {
            // 检查是否碰到水面(目标y位置)
            isGround = spriteTrans.position.y <= transform.position.y;
            if (isGround)
            {
                EventHandler.CallStartingFishing(transform);
            }
        }

        /// <summary>
        /// 运动的相关函数
        /// 通过控制Y轴不均匀运动来达成抛物线的效果，
        /// </summary>
        /*private void Bounce()
        {
            transform.position=  Vector3.MoveTowards(transform.position, targetPos, Settings.speed * Time.deltaTime);
            Parabolic();

            isGround = spriteTrans.position.y <= transform.position.y;//判断和影子的y是否重合
            if (isGround)
            {
                //执行甩鱼竿操作，生成一个浮漂
                EventHandler.CallStartingFishing(transform);
            }
        }

        private void Parabolic()
        {
            // 已经过的时间
            if (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / duration;
                float curveValue = curve.Evaluate(t);
                float newY = Mathf.Lerp(startHeight, targetHeight, curveValue);
                spriteTrans.position = new Vector3(spriteTrans.position.x, newY, spriteTrans.position.z);
            }
        }*/
}
