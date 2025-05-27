using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MyFarm.Inventory;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class FishUI : MonoBehaviour
{
   private Slider strengthSlider;
   private GameObject fishPanel;
   private GameObject MiniGamePanel;
   private GameObject MiniGameBar;
   public bool ifLooping = true;    // 是否循环
   private bool isCoroutineRunning = false;
   public GameObject floatingPrefab;
   private Transform playerTransform => FindObjectOfType<Player>().transform;
   private Transform objParent;
   private FishDetails currentFish;
   //等待钓鱼参数
   private float offsetX;//判断是在人物左边还是在右边生成面板
   [SerializeField] private TextMeshProUGUI biteTipsText; 
   [SerializeField] private float indicatorDuration = 1.5f;
   
   //钓鱼小游戏参数
   private RectTransform playerHookAreaObject;//玩家控制的方块UI
   private RectTransform hookArea;//方框所在长条
   private RectTransform fishTrans; // 小鱼UI
   private Slider processSlider;    // UI进度条
   private Image processSliderFillImage;    // UI进度条填充条
   private Vector2 fishTransStartPos;
   private Vector2 playerHookAreaObjectStartPos;
   [Header("Movement Settings")] 
   public float maxUpSpeed;      // 最大向上速度
   public float acceleration;    // 上浮加速度
   public float deceleration;    // 下沉减速度
   public float maxDownSpeed;         // 最大向下速度
   public float smoothTime ;      // 速度平滑时间

   [Header("Game Settings")]
   public float baseFishSpeed;   // 基础移动速度
   private float baseBlockSize;    // 基础长块大小
   public float alphaValue;//透明值
   private float moveRange;       // 移动范围限制
   private float bounceDecayFactor = 0.6f; // 每次反弹速度衰减系数（0.6表示保留60%速度）
   private float minBounceSpeed = 5f;     // 最小反弹速度（低于此值则停止）
   private bool isFlashing = false; // 标记是否处于闪烁状态
   private Coroutine flashCoroutine; // 闪烁协程引用
   private bool ifPerfect = true;
   [Header("Progress Settings")]
   public float baseProgressSpeed = 0.3f;    // 基础变化速度
   public float baseFator = 2/3;    // 速度差异因子
   private float currentProgressSpeed = 0f; // 当前变化速度
   private float randomChangeEffect = 0.001f;     //随机方向改变的影响因子
   
   // 游戏状态
   private float currentSpeed = 0f;
   private float targetSpeed = 0f;
   private float velocitySmoothing;
   private bool isGameActive = false;
   private bool isDashing = false;
   private Vector2 moveDirection = Vector2.up;
   
   // 当前参数
   private float currentFishSpeed;
   private float currentBlockSize;
   private float currentFishSize;
   private float currentProgressGain;
   private float currentProgressLoss;
   private float originalFishSpeed;


   private void OnEnable()
   {
      EventHandler.FishRodAccumulation += OnFishRodAccumulation;
      EventHandler.ThrowFishRod += OnThrowFishRod;
      EventHandler.FinishFishing += OnFinishFishing;
      EventHandler.AfterSceneEvent += OnAfterSceneEvent;
      EventHandler.GoFishing += OnGofishingEvent;
   }
   private void OnDisable()
   {
      EventHandler.FishRodAccumulation -= OnFishRodAccumulation;
      EventHandler.ThrowFishRod -= OnThrowFishRod;
      EventHandler.FinishFishing -= OnFinishFishing;
      EventHandler.AfterSceneEvent -= OnAfterSceneEvent;
      EventHandler.GoFishing -= OnGofishingEvent;
   }
   private void Awake()
   {
      fishPanel = transform.GetChild(0).gameObject;
      fishPanel.SetActive(false);
      MiniGamePanel=transform.GetChild(1).gameObject;
      MiniGamePanel.SetActive(false);   
      MiniGameBar=transform.GetChild(1).GetChild(0).gameObject;
      MiniGameBar.SetActive(false);
      strengthSlider=transform.GetChild(0).GetChild(0).GetComponent<Slider>();
      strengthSlider.gameObject.SetActive(false);
      processSlider=MiniGameBar.transform.GetChild(3).GetComponent<Slider>();
      processSliderFillImage = processSlider.fillRect.GetComponent<Image>();
      hookArea=MiniGameBar.transform.GetChild(0).gameObject.GetComponent<RectTransform>();
      playerHookAreaObject=MiniGameBar.transform.GetChild(1).gameObject.GetComponent<RectTransform>();
      fishTrans=MiniGameBar.transform.GetChild(2).gameObject.GetComponent<RectTransform>();
      baseBlockSize = playerHookAreaObject.sizeDelta.y;
      Debug.Log("baseBlockSize"+baseBlockSize);
      float uiBoxHeight = hookArea.rect.height; 
      moveRange = uiBoxHeight / 2; // 假设锚点居中
      fishTransStartPos = fishTrans.anchoredPosition;
      playerHookAreaObjectStartPos = playerHookAreaObject.anchoredPosition;
   }

   private void Update()
   {
     
   }

   private void OnAfterSceneEvent()
   {
      objParent = GameObject.FindWithTag("ObjectParent").transform;
   }
   /// <summary>
   /// 甩杆的动作
   /// </summary>
   /// <param name="mouseWorldPos"></param>
   private void OnThrowFishRod(Vector3 mouseWorldPos)
   {
      if (isCoroutineRunning)
      {
         StopCoroutine(LoopSlider());
         isCoroutineRunning = false;
         strengthSlider.gameObject.SetActive(false);
         fishPanel.SetActive(false);
         FishManager.Instance.isThrowing = false;
         var fishFloating = Instantiate(floatingPrefab, playerTransform.position, Quaternion.identity, objParent);
      
         var mouseX = mouseWorldPos.x-playerTransform.position.x;
         offsetX = mouseX;
         var mouseY = mouseWorldPos.y-playerTransform.position.y;
         //算出上下左右的偏移量 因为规定使用工具是按照格子来的
         if (Mathf.Abs(mouseX)>Mathf.Abs(mouseY))//左右偏移量大
            mouseY = 0;
         else
            mouseX = 0;
         
         fishFloating.GetComponent<FishBounce>().InitBounceItem(mouseX,mouseY,strengthSlider.value);
         FishManager.Instance.fishGameState = FishState.Throw;
         FishingLineManager.Instance.CreateNewLine(FishManager.Instance.fishLineStartTrans,fishFloating.transform.GetChild(0).transform);
         // 5. 将钓鱼线引用存储到鱼漂或FishManager中，以便后续控制
         Debug.Log("甩杆的力度为"+strengthSlider.value);    
      }
   }
   /// <summary>
   /// 蓄力
   /// </summary>
   private void OnFishRodAccumulation()
   {
      if (!isCoroutineRunning && ifLooping)
      {
         fishPanel.SetActive(true);
         strengthSlider.gameObject.SetActive(true);
         strengthSlider.transform.position = GameObject.FindGameObjectWithTag("Player").transform.position+new Vector3(0,3f,0);
         //控制人物不能移动
         EventHandler.CallUpdateGameStateEvent(GameState.PlayerStop);
         FishManager.Instance.isThrowing = true;
         isCoroutineRunning = true;
         FishManager.Instance.fishGameState = FishState.Charge;
         StartCoroutine(LoopSlider());
      }
   }
   /// <summary>
   /// 蓄力挥杆的slider更新函数
   /// </summary>
   /// <returns></returns>
   IEnumerator LoopSlider()
   {
      float timeElapsed = 0f;
      while (true)
      {
         while (timeElapsed < Settings.ThrowDuration)
         {
            timeElapsed += Time.deltaTime;
            strengthSlider.value = Mathf.Lerp(0f, 1f, timeElapsed / Settings.ThrowDuration);
            yield return null;  // 等待下一帧
         }
         timeElapsed = 0f;
         while (timeElapsed < Settings.ThrowDuration)
         {
            timeElapsed += Time.deltaTime;
            strengthSlider.value = Mathf.Lerp(1f, 0f, timeElapsed / Settings.ThrowDuration);
            yield return null;  // 等待下一帧
         }
         timeElapsed = 0f;
         yield return null;
      }
   }

   #region 开启钓鱼小游戏流程

    /// <summary>
   /// 此时已经是鱼上钩的状态，
   /// 默认设定1.5s如果不拉勾就重新钓鱼游戏
   /// </summary>
   private void OnGofishingEvent()
   {
      Debug.Log("等待鱼上钩！");
      RectTransform panelRect = MiniGamePanel.GetComponent<RectTransform>();
      // 计算偏移方向（基于角色朝向）
      Vector3 horizontalOffset = offsetX < 0? playerTransform.right*1.2f:-playerTransform.right*1.5f; // 水平偏移量
      float verticalOffset = 1f;     // 垂直偏移量
    
      // 计算目标位置
      Vector3 targetPosition = playerTransform.position 
                               + horizontalOffset 
                               + Vector3.up * verticalOffset;
    
      panelRect.position = targetPosition;
      MiniGamePanel.SetActive(true);
      ShowBiteTipsText();
      //监听鼠标左键。
      StartCoroutine(WaitForPlayerInput());
   
   }
   private IEnumerator WaitForPlayerInput()
   {
      bool playerClicked = false;
      float timer = 0f;
      float waitDuration = 1.5f; // 与提示文字显示时间一致
    
      while (timer < waitDuration && !playerClicked)
      {
         if (Input.GetMouseButtonDown(0))
         {
            playerClicked = true;
            // 玩家点击后的逻辑 
            OnPlayerClickedBite();
         }
        
         timer += Time.deltaTime;
         yield return null;
      }
    
      if (!playerClicked)
      {
         // 玩家未点击，重新调用钓鱼方法
         EventHandler.CallStartingFishing(FishManager.Instance.currentFishFLoatingTrans);
      }
   }

   private void OnPlayerClickedBite()
   {

      FishManager.Instance.fishGameState = FishState.Playing;
      // 玩家在正确时机点击后的逻辑
      if (FishManager.Instance.currentFish!=null)
      {
         currentFish = FishManager.Instance.currentFish;
      }
  
      // 这里可以开始你的钓鱼小游戏核心玩法
      MiniGameBar.SetActive(true);
      //停止人物移动
      EventHandler.CallUpdateGameStateEvent(GameState.PlayerStop);
      Debug.Log("玩家成功点击！进入钓鱼小游戏");
      if (currentFish.level==0)
      {
         OnSuccess();
         Debug.Log("钓上垃圾了");
         return;
      }
      //开始钓鱼逻辑
      SetupDifficulty();
      StartCoroutine(FishingGameLoop());
   }
   private void ShowBiteTipsText()
   {
      // 显示提示
      biteTipsText.gameObject.SetActive(true);
      biteTipsText.color = new Color(biteTipsText.color.r, biteTipsText.color.g, biteTipsText.color.b, 1);
    
      // 保存初始位置
      Vector3 originalPosition = biteTipsText.rectTransform.anchoredPosition;
    
      // 同时执行淡出和上移动画
      Sequence sequence = DOTween.Sequence();
      sequence.Join(biteTipsText.DOFade(0, indicatorDuration).SetEase(Ease.OutQuad));
      sequence.Join(biteTipsText.rectTransform.DOAnchorPosY(originalPosition.y + 30f, indicatorDuration).SetEase(Ease.OutQuad));
    
      sequence.OnComplete(() => {
         biteTipsText.gameObject.SetActive(false);
         biteTipsText.rectTransform.anchoredPosition = originalPosition;
      });
      //TODO:鼠标点击后要直接停止消失
   }

   #endregion
  
   
   private void SetupDifficulty()
   {
      int fishingLevel = SkillManager.Instance.GetSkillBySkillType<BasicFishingSkill>(SkillType.BasicFishing).skillLevel;
      // 长块大小随钓鱼等级增大
      currentBlockSize = baseBlockSize * (1 + (fishingLevel - 1) * 0.05f);
      playerHookAreaObject.sizeDelta = new Vector2(playerHookAreaObject.sizeDelta.x, currentBlockSize);
      currentFishSize = fishTrans.sizeDelta.y;
      // 鱼的基础速度受鱼难度影响
      currentFishSpeed = baseFishSpeed * (1 + (currentFish.difficultyLevel - 1) * 0.05f);
      originalFishSpeed = currentFishSpeed;

      /*// 进度增益 = 基础 * (1 + 钓鱼等级*0.05) / (1 + 鱼难度*0.1)
      currentProgressGain = baseProgressGain * 
                            (1 + (fishingLevel - 1) * 0.05f) / 
                            (1 + (currentFish.difficultyLevel - 1) * 0.1f);

      // 进度损失 = 基础 * (1 + 鱼难度*0.15) / (1 + 钓鱼等级*0.03)
      currentProgressLoss = baseProgressLoss * 
                            (1 + (currentFish.difficultyLevel - 1) * 0.15f) / 
                            (1 + (fishingLevel - 1) * 0.03f);*/
   }

   private IEnumerator FishingGameLoop()
   {
      ResetGame();
      isGameActive = true;
      
      while (isGameActive)
      {
         MoveFish();
         HandlePlayerInput();
         UpdateProgress();
            
         // 只在非冲刺状态下检查是否触发冲刺
         if (!isDashing && currentFish.canDash)
         {
            CheckForDash();
         }
            
         
         if (processSlider.value >= 1f) 
         {
            OnSuccess();
            yield break; // 立即退出协程
         }
         // 失败条件进度条为0
         if (processSlider.value <= 0f)
         {
               OnFail();
               yield break; 
         }
         yield return null;
      }
   }

   #region 鱼标移动逻辑

   private void MoveFish()
   {
      // 随机改变方向 随机因子与难度等级的乘积
      Debug.Log("随机改变概率"+randomChangeEffect * currentFish.difficultyLevel);
      if (Random.value < randomChangeEffect * currentFish.difficultyLevel)
         moveDirection *= -1;

      // 移动目标
      fishTrans.anchoredPosition += moveDirection * currentFishSpeed * Time.deltaTime;

      // 边界检测 TODO:鱼的边界可能要细扣一下 目前只是图片本身大小
      float limit = moveRange - currentFishSize/2;
      if (Mathf.Abs(fishTrans.anchoredPosition.y) > limit)
      {
         moveDirection *= -1;
         fishTrans.anchoredPosition = new Vector2(
            0,
            Mathf.Clamp(fishTrans.anchoredPosition.y, -limit, limit)
         );
      }
   }

   #endregion

   #region 玩家控制的长块
    private void HandlePlayerInput()
   {
      bool tryMoveUp = Input.GetMouseButton(0);
      float rawTargetSpeed = tryMoveUp ? maxUpSpeed : -maxDownSpeed;
      float topMoveRange = moveRange - currentBlockSize / 2;
      float bottomMoveRange = -moveRange + currentBlockSize / 2;
      // 精确边界检测（带0.1像素缓冲）
      bool atTop = playerHookAreaObject.anchoredPosition.y >= topMoveRange-0.1f;
      bool atBottom = playerHookAreaObject.anchoredPosition.y <= bottomMoveRange+0.1f;
     
      if (atTop || atBottom)
      {
         //在顶部且按向上时 来点振动
         if (atTop && tryMoveUp)
         { 
            float xShake = Mathf.Sin(Time.time * 25f) * 3f;  // 左右抖动（3像素幅度）
            playerHookAreaObject.anchoredPosition = new Vector2(
               xShake, 
               topMoveRange - Mathf.PingPong(Time.time * 10f, 0.5f)
            );
         }
         //在顶部且自由落体时 来个初速度 可以再调节
         if (atTop && !tryMoveUp)
         {
            targetSpeed = rawTargetSpeed;
            currentSpeed = rawTargetSpeed/10;
         }
         //在底部且自由落体时 来点反弹
         if (atBottom && !tryMoveUp)
         {
            float bounceSpeed = Mathf.Abs(currentSpeed * bounceDecayFactor);
            if (bounceSpeed > minBounceSpeed)
            {
               currentSpeed = bounceSpeed; // 向上反弹
               targetSpeed = 0; // 反弹后自然减速
            }
            else
            {
               currentSpeed = 0;
               targetSpeed = 0;
               playerHookAreaObject.anchoredPosition = new Vector2(0, bottomMoveRange); // 对齐到底部
            }
         }
         //在底部且按向上时 来个初速度
         if (atBottom && tryMoveUp)
         {
            targetSpeed = rawTargetSpeed;
            currentSpeed = rawTargetSpeed/5;
         }
      }
      else
      {
         // 正常情况下的平滑运动
         targetSpeed = rawTargetSpeed;
         
      }
      HandleNormalMovement();
      
      playerHookAreaObject.anchoredPosition += Vector2.up * currentSpeed * Time.deltaTime;
      float limit = moveRange - currentBlockSize/2;
      // 最终位置修正（硬限制）
      playerHookAreaObject.anchoredPosition = new Vector2(
         0,
         Mathf.Clamp(playerHookAreaObject.anchoredPosition.y, -limit, limit)
      );
   }
   private void HandleNormalMovement()
   {
      currentSpeed = Mathf.SmoothDamp(
         currentSpeed,
         targetSpeed,
         ref velocitySmoothing,
         smoothTime,
         (targetSpeed > currentSpeed) ? acceleration : deceleration
      );
   }
   #endregion

   #region 鱼冲刺的逻辑
   private void CheckForDash()
   {
      if (Random.value < currentFish.dashProbability * Time.deltaTime)
      {
         StartCoroutine(HandleDash());
      }
   }
   private IEnumerator HandleDash()
   {
      isDashing = true;
      // 冲刺时临时参数
      float originalMaxSpeed = maxUpSpeed;
      float originalGravity = maxDownSpeed;
      maxUpSpeed *= 0.7f;
      maxDownSpeed *= 1.5f;
      currentFishSpeed = originalFishSpeed * currentFish.dashSpeedMultiplier;
        
      yield return new WaitForSeconds(currentFish.dashDuration);
        
      // 恢复参数
      maxUpSpeed = originalMaxSpeed;
      maxDownSpeed = originalGravity;
      currentFishSpeed = originalFishSpeed;
      isDashing = false;
   }

   #endregion

   #region 进度条更新逻辑

   private void UpdateProgress()
   {
      //TODO：可能要和图标本身去优化判断逻辑，目前只是简单的中心点与高度一半
      bool isOverlapping = Mathf.Abs(fishTrans.anchoredPosition.y - playerHookAreaObject.anchoredPosition.y) < currentBlockSize/2;
      //未重叠时把方块虚化作为提示
      Image blockImage = playerHookAreaObject.GetComponent<Image>();
      if (isOverlapping)
      {
         // 如果正在闪烁，立即停止闪烁
         if (isFlashing)
         {
            if (flashCoroutine != null)
            {
               StopCoroutine(flashCoroutine);
               flashCoroutine = null;
            }
            isFlashing = false;
         }
         blockImage.DOKill();
         blockImage.color = new Color(blockImage.color.r, blockImage.color.g, blockImage.color.b, 1f);
      }
      else
      {
         // 非重叠状态且未开始闪烁时，启动闪烁
         if (!isFlashing)
         {
            ifPerfect = false;
            blockImage.DOKill();
            blockImage.color = new Color(blockImage.color.r, blockImage.color.g, blockImage.color.b, 0.3f);
            isFlashing = true;
            flashCoroutine = StartCoroutine(FlashBlock());
         }
      }
      
      currentProgressSpeed = isOverlapping ? baseProgressSpeed * baseFator : -baseProgressSpeed;
      // 应用速度变化
      processSlider.value += currentProgressSpeed * Time.deltaTime;
    
      // 限制进度范围
      processSlider.value = Mathf.Clamp01(processSlider.value);
      UpdateProgressVisual();
   }
   private void UpdateProgressVisual()
   {
      // 计算进度比例 (0~1)
      float progressRatio = processSlider.value;

      // 三色渐变逻辑
      Color targetColor;
      if (progressRatio < 0.3f)
      {
         // 橙色到黄色渐变 (0~0.3)
         targetColor = Color.Lerp(
            new Color(1f, 0.5f, 0f),    // 橙色
            Color.yellow,                // 黄色
            progressRatio / 0.3f        // 过渡比例
         );
      }
      else
      {
         // 黄色到绿色渐变 (0.3~1)
         targetColor = Color.Lerp(
            Color.yellow,                // 黄色
            Color.green,                 // 绿色
            (progressRatio - 0.3f) / 0.7f // 过渡比例
         );
      }

      // 使用DOTween平滑过渡颜色（避免每帧直接修改）
      if (!DOTween.IsTweening(processSliderFillImage))
      {
         processSliderFillImage.DOColor(targetColor, 0.3f)
            .SetEase(Ease.OutQuad);
      }
   }
   private IEnumerator FlashBlock()
   {
      Image blockImage = playerHookAreaObject.GetComponent<Image>();
      float flashDuration = 0.8f; // 完整闪烁周期时长
      blockImage.color = new Color(blockImage.color.r, blockImage.color.g, blockImage.color.b, alphaValue);
      while (true)
      {
         // 从alpha到1
         if (!DOTween.IsTweening(blockImage))
         {
            blockImage.DOFade(1f, flashDuration/2);
         }
         yield return new WaitForSeconds(flashDuration/2);
        
         // 从1到alpha
         if (!DOTween.IsTweening(blockImage))
         {
            blockImage.DOFade(alphaValue, flashDuration/2);
         }
         yield return new WaitForSeconds(flashDuration/2);
      }
   }
   #endregion
   
   private void ResetGame() {
      processSlider.value = 0.3f;
      fishTrans.anchoredPosition = fishTransStartPos;
      playerHookAreaObject.anchoredPosition = playerHookAreaObjectStartPos;
      currentSpeed = 0f;
      targetSpeed = -maxDownSpeed;
   }
   private void OnSuccess() {
      isGameActive = false;
      Debug.Log($"成功钓到: {currentFish.ItemName}!");
      //EventHandler.CallFishingResultEvent(true, currentFish);
      if (ifPerfect && currentFish.level != 0) 
      {
         Debug.Log("完美钓上且不是垃圾");
         //TODO：还没完成完美判定的代码
         FishManager.Instance.ChangePerfectFish();
         //TODO:可以加音效或者动画
      }
      EventHandler.CallFinishFishing();
      FishManager.Instance.AddFishToBag();
      ResetGame();
   }

   private void OnFail() {
      isGameActive = false;
      Debug.Log("钓鱼失败!");
      EventHandler.CallFinishFishing();
      //EventHandler.CallFishingResultEvent(false, null);
      ResetGame();
   }
   private void OnFinishFishing()
   {
      fishPanel.SetActive(false);
      MiniGamePanel.SetActive(false);
      MiniGameBar.SetActive(false);
      strengthSlider.gameObject.SetActive(false);
      currentFish = null;
      isGameActive = false;
   }
}
