using System; 
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ETModel
{
    public class MonoEventComponent : Component
    {
        #region 点击事件

        public void AddButtonClick(Button obj, Action action)
        {
            AddButtonClick(obj, true, action);
        }

        /// <summary> /// 添加ButtonClick事件 /// </summary>
        /// <param name="obj">Button组件</param>
        /// <param name="isRemoveAllListeners">是否清除以前注册的事件</param>
        /// <param name="action">回调函数</param>
        public void AddButtonClick(Button obj, bool isRemoveAllListeners, Action action)
        {
            if (isRemoveAllListeners) obj.onClick.RemoveAllListeners();

            obj.onClick.Add(action);
        }
        
        /// <summary>
        /// 移除所有点击事件
        /// </summary>
        /// <param name="obj">Button组件</param>
        public void RemoveButtonClick(Button obj)
        {
            obj.onClick.RemoveAllListeners();
        }

        #endregion

        #region 触发事件

        /// <summary> /// 添加触发事件 /// </summary>
        /// <param name="obj">GameObject</param>
        /// <param name="eventTriggerType">EventTriggerType</param>
        /// <param name="action">回调函数</param>
        /// <returns></returns>
        public EventTrigger AddEventTrigger(GameObject obj, EventTriggerType eventTriggerType, Action<BaseEventData> action)
        {
            return AddEventTrigger(obj, eventTriggerType, true, action);
        }

        /// <summary> /// 添加触发事件 /// </summary>
        /// <param name="obj">GameObject</param>
        /// <param name="eventTriggerType">EventTriggerType</param>
        /// <param name="isRemoveAllListeners">添加事件是否清空以前添加的事件</param>
        /// <param name="action">回调函数</param>
        /// <returns></returns>
        public EventTrigger AddEventTrigger(GameObject obj, EventTriggerType eventTriggerType, bool isRemoveAllListeners, Action<BaseEventData> action)
        {
            return AddEventTrigger(obj, eventTriggerType, isRemoveAllListeners, false, action);
        }

        /// <summary> /// 添加触发事件 /// </summary>
        /// <param name="obj">GameObject</param>
        /// <param name="eventTriggerType">EventTriggerType</param>
        /// <param name="isRemoveAllListeners">添加事件是否清空以前添加的事件</param>
        /// <param name="isClearTriggers">是否清除GameObject上所有触发事件</param>
        /// <param name="action">回调函数</param>
        /// <returns></returns>
        public EventTrigger AddEventTrigger(GameObject obj, EventTriggerType eventTriggerType, bool isRemoveAllListeners, bool isClearTriggers, Action<BaseEventData> action)
        {
            if (obj == null) return null;

            var eventTrigger = obj.GetComponent<EventTrigger>() ?? obj.AddComponent<EventTrigger>();

            var entry = new EventTrigger.Entry {eventID = eventTriggerType};


            var callback = new UnityAction<BaseEventData>(action);
            entry.callback.AddListener(callback);
            if (isClearTriggers) eventTrigger.triggers.Clear();

            eventTrigger.triggers.Add(entry);
            return eventTrigger;
        }

        #endregion
    }
}