using System;
using System.Collections;
using MyFarm.Map;
using System.Diagnostics;
using System.Reflection;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class Player : MonoBehaviour
{
        public float moveSpeed;
    
        private Rigidbody2D _rb2d;
        private float _horizontal;
        private float _vertical;
        private Vector2 _movement;
        private Animator[] animators;
        private bool isMoving;
        private bool inputDisable;
        private Vector3 lastMousePos;
        //动画使用工具
        private float mouseX;
        private float mouseY;
        private bool useTool;
        private void Awake()
        {
            _rb2d = GetComponent<Rigidbody2D>();
            animators = GetComponentsInChildren<Animator>();
        }

        private void OnEnable()
        {
            EventHandler.BeforeSceneEvent += OnBeforeSceneEvent;
            EventHandler.AfterSceneEvent += OnAfterSceneEvent;
            EventHandler.MoveToPosition += OnMoveToPosition;
            EventHandler.MouseClickedEvent += OnMouseClickedEvent;
            EventHandler.MouseReleaseEvent += OnMouseReleaseEvent;
            EventHandler.UpdateGameStateEvent += OnUpdateGameStateEvent;
        }

        private void OnDisable()
        {
            EventHandler.BeforeSceneEvent -= OnBeforeSceneEvent;
            EventHandler.AfterSceneEvent -= OnAfterSceneEvent;
            EventHandler.MoveToPosition -= OnMoveToPosition;
            EventHandler.MouseClickedEvent -= OnMouseClickedEvent;
            EventHandler.MouseReleaseEvent -= OnMouseReleaseEvent;
            EventHandler.UpdateGameStateEvent -= OnUpdateGameStateEvent;
        }
        private void Update()
        {
            if (!inputDisable)
            {
                Debug.Log("Player is moving");
                PlayerInput();
            }
              
            else
                isMoving = false;//关闭移动动画
            SwitchAnimators();
        }

        private void FixedUpdate()
        {
            if (!inputDisable)
            {
                Movement();
            }
            
        }

        private void OnUpdateGameStateEvent(GameState gameState)
        {
            // 获取调用堆栈信息
            StackTrace stackTrace = new StackTrace();
            // 获取上一帧的调用方法（索引1，因为0是当前方法）
            StackFrame callerFrame = stackTrace.GetFrame(1);
            MethodBase callerMethod = callerFrame.GetMethod();
    
           // Debug.Log($"切换游戏状态为 {gameState}，由 {callerMethod.DeclaringType.Name}.{callerMethod.Name} 调用");
            switch (gameState)
            {
                case GameState.GamePlay:
                    inputDisable = false;
                    break;
                case GameState.Pause:
                    inputDisable = true;
                    break;
                case GameState.PlayerStop:
                    inputDisable = true;
                    break;
            }
        }

        private void OnMouseClickedEvent(Vector3 mouseWorldPos, ItemDetails itemDetails)
        {
            lastMousePos = mouseWorldPos;
            mouseX = mouseWorldPos.x-transform.position.x;
            mouseY = mouseWorldPos.y-transform.position.y;
            //算出上下左右的偏移量 因为规定使用工具是按照格子来的
            if (Mathf.Abs(mouseX)>Mathf.Abs(mouseY))//左右偏移量大
                mouseY = 0;
            else
                mouseX = 0;
            //如果使用的是工具
            if (itemDetails is Equipment {equipmentType: not Equipment.EquipmentType.FishingTool})
            {
                StartCoroutine(UseToolDelayRoutine(mouseWorldPos, itemDetails));
            }else if (itemDetails is Equipment {equipmentType: Equipment.EquipmentType.FishingTool}&& !FishManager.Instance.isThrowing)
            {
                StartCoroutine(UseToolsRoutine(mouseWorldPos,itemDetails));
            }
            //TODO: 考虑释放魔素是否还需要道具，不需要道具的话就只需要判断当前是否是准备释放魔素状态（感觉还是要搞一个道具作为媒介比较好）
            else if (itemDetails is Consumable {consumableType: Consumable.ConsumableType.Magic})
            {
                //调用魔素逻辑
                ApplyMagicToCrop(mouseWorldPos,itemDetails);
            }
            else
                EventHandler.CallExcuteActionAfterAnimation(mouseWorldPos,itemDetails);
        }
        
        private void OnMouseReleaseEvent(Vector3 mouseWorldPos, ItemDetails itemDetails)
        {
            if (itemDetails is Equipment {equipmentType: Equipment.EquipmentType.FishingTool}&& FishManager.Instance.fishGameState==FishState.Charge)//执行挥杆钓鱼的逻辑
            {
                StartCoroutine(UseToolNoDelayRoutine(lastMousePos,itemDetails));
            }
        }
        private IEnumerator UseToolDelayRoutine(Vector3 mouseWorldPos, ItemDetails itemDetails)
        {
            useTool = true;
            OnUpdateGameStateEvent(GameState.PlayerStop);
            yield return null;
            foreach (var anim in animators)
            {
                //每套动作对应一个animator 执行动画
                anim.SetTrigger("UseTool");
                //人物面朝方向
                anim.SetFloat("InputX",mouseX);
                anim.SetFloat("InputY",mouseY);
            }
            yield return new WaitForSeconds(0.45f);//这个时间是大致执行完锄地的一半动画的点
            EventHandler.CallExcuteActionAfterAnimation(mouseWorldPos, itemDetails);
            yield return new WaitForSeconds(0.25f);//等待动画结束
            useTool = false;
            OnUpdateGameStateEvent(GameState.GamePlay);
        }
        /// <summary>
        /// 针对于要使用多连串动作动画的情况，前面的几个动作
        /// </summary>
        /// <param name="mouseWorldPos"></param>
        /// <param name="itemDetails"></param>
        /// <returns></returns>
        private IEnumerator UseToolsRoutine(Vector3 mouseWorldPos, ItemDetails itemDetails)
        {
            useTool = true;
            OnUpdateGameStateEvent(GameState.PlayerStop);
            yield return null;
            foreach (var anim in animators)
            {
                //每套动作对应一个animator 执行动画
                anim.SetTrigger("UseTool");
                //人物面朝方向
                anim.SetFloat("InputX",mouseX);
                anim.SetFloat("InputY",mouseY);
            }
            yield return new WaitForSeconds(0.45f);//这个时间是大致执行完锄地的一半动画的点
            EventHandler.CallExcuteActionAfterAnimation(mouseWorldPos, itemDetails);
            yield return new WaitForSeconds(0.25f);//等待动画结束
            useTool = false;
        }
        /// <summary>
        /// 无延迟的执行逻辑，例如松手 会立马丢鱼竿
        /// </summary>
        /// <param name="mouseWorldPos"></param>
        /// <param name="itemDetails"></param>
        /// <returns></returns>
        private IEnumerator UseToolNoDelayRoutine(Vector3 mouseWorldPos, ItemDetails itemDetails)
        {
            useTool = true;
            OnUpdateGameStateEvent(GameState.PlayerStop);
            yield return null;
            foreach (var anim in animators)
            {
                //每套动作对应一个animator 执行动画
                anim.SetTrigger("UseTool");
                //人物面朝方向
                anim.SetFloat("InputX",mouseX);
                anim.SetFloat("InputY",mouseY);
            }
            EventHandler.CallExcuteActionAfterAnimation(mouseWorldPos, itemDetails);
            yield return new WaitForSeconds(0.25f);//等待动画结束
            useTool = false;
            //OnUpdateGameStateEvent(GameState.GamePlay);
        }
        private void OnMoveToPosition(Vector3 position)
        {
            transform.position = position;
        }

        private void OnAfterSceneEvent()
        {
            OnUpdateGameStateEvent(GameState.GamePlay);
        }

        private void OnBeforeSceneEvent()
        {
            OnUpdateGameStateEvent(GameState.Pause);
        }

        private void PlayerInput()
        {
            _horizontal = Input.GetAxisRaw("Horizontal");
            _vertical = Input.GetAxisRaw("Vertical");
            if (_horizontal != 0 && _vertical != 0)
            {
                _horizontal *= 0.6f;
                _vertical *= 0.6f;
            }
            //走路状态速度
            if (Input.GetKey(KeyCode.LeftShift))
            {
                _horizontal *= 0.5f;
                _vertical *= 0.5f;
            }
            _movement = new Vector2(_horizontal, _vertical);
            isMoving = _movement != Vector2.zero;
        }

        private void Movement()
        {
            _rb2d.MovePosition(_rb2d.position+ _movement * (moveSpeed * Time.deltaTime));
        }

        private void SwitchAnimators()
        {
            foreach (var anim in animators)
            {
                anim.SetBool("IsMoving", isMoving);
                anim.SetFloat("MouseX", mouseX);
                anim.SetFloat("MouseY", mouseY);
                if (isMoving)
                {
                    anim.SetFloat("InputX", _horizontal);
                    anim.SetFloat("InputY", _vertical);
                }
            }
        }
        /// <summary>
        /// 给植物添加魔素
        /// </summary>
        /// <param name="mouseWorldPos"></param>
        /// <param name="itemDetails"></param>
        private void ApplyMagicToCrop(Vector3 mouseWorldPos, ItemDetails itemDetails)
        {
            TileDetails tileDetails = GridMapManager.Instance.GetTileDetailsFromWorldPosition(mouseWorldPos);
           //TODO:解决这个问题  MagicAmount应该是在比如cropDeatail里的
           // EventHandler.CallApplyMagicEvent(tileDetails,itemDetails.MagicAmount);
        }
        
}
