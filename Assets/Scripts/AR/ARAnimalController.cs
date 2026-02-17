using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace EndangeredAnimalAR.AR
{
    /// <summary>
    /// AR 动物控制器 - 管理 AR 场景中的动物模型
    /// </summary>
    [RequireComponent(typeof(ARRaycastManager))]
    public class ARAnimalController : MonoBehaviour
    {
        [Header("AR 设置")]
        [SerializeField] private ARRaycastManager arRaycastManager;
        [SerializeField] private GameObject placementIndicator;
        
        [Header("动物模型")]
        [SerializeField] private GameObject[] animalPrefabs;
        [SerializeField] private Transform animalContainer;
        
        [Header("交互设置")]
        [SerializeField] private float rotationSpeed = 50f;
        [SerializeField] private float scaleSpeed = 0.5f;
        [SerializeField] private float minScale = 0.3f;
        [SerializeField] private float maxScale = 3f;
        
        private List<ARRaycastHit> hits = new List<ARRaycastHit>();
        private Pose placementPose;
        private bool placementPoseIsValid = false;
        private GameObject currentAnimal;
        private int currentAnimalIndex = 0;
        
        void Start()
        {
            if (arRaycastManager == null)
                arRaycastManager = GetComponent<ARRaycastManager>();
            
            if (placementIndicator != null)
                placementIndicator.SetActive(false);
            
            if (animalContainer == null)
                animalContainer = new GameObject("AnimalContainer").transform;
        }
        
        void Update()
        {
            UpdatePlacementPose();
            UpdatePlacementIndicator();
            
            // 处理触摸输入
            HandleTouchInput();
        }
        
        /// <summary>
        /// 更新放置位置
        /// </summary>
        private void UpdatePlacementPose()
        {
            // 从屏幕中心发射射线
            var screenCenter = Camera.main.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
            
            if (arRaycastManager != null)
            {
                hits.Clear();
                arRaycastManager.Raycast(screenCenter, hits, TrackableType.PlaneWithinPolygon);
                
                placementPoseIsValid = hits.Count > 0;
                if (placementPoseIsValid)
                {
                    placementPose = hits[0].pose;
                    
                    // 让指示器朝向相机
                    var cameraForward = Camera.main.transform.forward;
                    var cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
                    placementPose.rotation = Quaternion.LookRotation(cameraBearing);
                }
            }
        }
        
        /// <summary>
        /// 更新放置指示器
        /// </summary>
        private void UpdatePlacementIndicator()
        {
            if (placementIndicator != null)
            {
                placementIndicator.SetActive(placementPoseIsValid);
                
                if (placementPoseIsValid)
                {
                    placementIndicator.transform.SetPositionAndRotation(
                        placementPose.position, 
                        placementPose.rotation
                    );
                }
            }
        }
        
        /// <summary>
        /// 处理触摸输入
        /// </summary>
        private void HandleTouchInput()
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                
                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        // 单指点击 - 放置或选择动物
                        if (placementPoseIsValid && currentAnimal == null)
                        {
                            PlaceAnimal();
                        }
                        else if (currentAnimal != null)
                        {
                            // 检查是否点击了动物
                            Ray ray = Camera.main.ScreenPointToRay(touch.position);
                            RaycastHit hit;
                            
                            if (Physics.Raycast(ray, out hit))
                            {
                                if (hit.transform.IsChildOf(animalContainer))
                                {
                                    OnAnimalSelected(hit.transform.gameObject);
                                }
                            }
                        }
                        break;
                        
                    case TouchPhase.Moved:
                        if (Input.touchCount == 1 && currentAnimal != null)
                        {
                            // 单指移动 - 旋转动物
                            float rotationAmount = touch.deltaPosition.x * rotationSpeed * Time.deltaTime;
                            currentAnimal.transform.Rotate(0, rotationAmount, 0, Space.World);
                        }
                        else if (Input.touchCount == 2)
                        {
                            // 双指缩放
                            HandlePinchZoom();
                        }
                        break;
                }
            }
        }
        
        /// <summary>
        /// 处理双指缩放
        /// </summary>
        private void HandlePinchZoom()
        {
            if (currentAnimal == null) return;
            
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);
            
            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;
            
            float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float currentMagnitude = (touchZero.position - touchOne.position).magnitude;
            
            float difference = currentMagnitude - prevMagnitude;
            
            // 计算缩放
            float scaleFactor = 1 + (difference * scaleSpeed * Time.deltaTime);
            Vector3 newScale = currentAnimal.transform.localScale * scaleFactor;
            
            // 限制缩放范围
            newScale.x = Mathf.Clamp(newScale.x, minScale, maxScale);
            newScale.y = Mathf.Clamp(newScale.y, minScale, maxScale);
            newScale.z = Mathf.Clamp(newScale.z, minScale, maxScale);
            
            currentAnimal.transform.localScale = newScale;
        }
        
        /// <summary>
        /// 放置动物模型
        /// </summary>
        private void PlaceAnimal()
        {
            if (animalPrefabs == null || animalPrefabs.Length == 0)
            {
                Debug.LogWarning("没有可用的动物预制体");
                return;
            }
            
            // 销毁现有的动物
            if (currentAnimal != null)
            {
                Destroy(currentAnimal);
            }
            
            // 实例化新动物
            GameObject prefab = animalPrefabs[currentAnimalIndex % animalPrefabs.Length];
            currentAnimal = Instantiate(prefab, placementPose.position, placementPose.rotation, animalContainer);
            
            Debug.Log($"放置动物: {prefab.name}");
            
            // 触发放置事件
            OnAnimalPlaced(currentAnimal);
        }
        
        /// <summary>
        /// 切换到下一个动物
        /// </summary>
        public void NextAnimal()
        {
            if (animalPrefabs == null || animalPrefabs.Length == 0) return;
            
            currentAnimalIndex = (currentAnimalIndex + 1) % animalPrefabs.Length;
            Debug.Log($"切换到动物: {currentAnimalIndex}");
            
            // 如果已经有动物在场景中，替换它
            if (currentAnimal != null && placementPoseIsValid)
            {
                PlaceAnimal();
            }
        }
        
        /// <summary>
        /// 切换到上一个动物
        /// </summary>
        public void PreviousAnimal()
        {
            if (animalPrefabs == null || animalPrefabs.Length == 0) return;
            
            currentAnimalIndex = (currentAnimalIndex - 1 + animalPrefabs.Length) % animalPrefabs.Length;
            Debug.Log($"切换到动物: {currentAnimalIndex}");
            
            // 如果已经有动物在场景中，替换它
            if (currentAnimal != null && placementPoseIsValid)
            {
                PlaceAnimal();
            }
        }
        
        /// <summary>
        /// 移除当前动物
        /// </summary>
        public void RemoveAnimal()
        {
            if (currentAnimal != null)
            {
                Destroy(currentAnimal);
                currentAnimal = null;
                Debug.Log("动物已移除");
            }
        }
        
        /// <summary>
        /// 动物被放置时的回调
        /// </summary>
        private void OnAnimalPlaced(GameObject animal)
        {
            // 这里可以添加放置后的逻辑，比如播放动画、声音等
            Debug.Log($"动物已放置: {animal.name}");
            
            // 示例：播放放置动画
            Animator animator = animal.GetComponent<Animator>();
            if (animator != null)
            {
                animator.SetTrigger("Appear");
            }
        }
        
        /// <summary>
        /// 动物被选中时的回调
        /// </summary>
        private void OnAnimalSelected(GameObject animal)
        {
            Debug.Log($"动物被选中: {animal.name}");
            
            // 这里可以添加选中后的逻辑，比如高亮显示、显示信息面板等
            // 示例：播放选中动画
            Animator animator = animal.GetComponent<Animator>();
            if (animator != null)
            {
                animator.SetTrigger("Selected");
            }
        }
        
        /// <summary>
        /// 获取当前动物名称
        /// </summary>
        public string GetCurrentAnimalName()
        {
            if (animalPrefabs == null || animalPrefabs.Length == 0)
                return "未知";
            
            GameObject prefab = animalPrefabs[currentAnimalIndex % animalPrefabs.Length];
            return prefab.name;
        }
        
        /// <summary>
        /// 获取当前动物对象
        /// </summary>
        public GameObject GetCurrentAnimal()
        {
            return currentAnimal;
        }
    }
}