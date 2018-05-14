using System;
using System.Collections.Generic;
using System.Linq;
using ETModel;
using UnityEngine;

namespace ETHotfix
{
    /// <summary>
    /// UI的层级类型
    /// </summary>
    public enum UILayer
    {
        UiHiden = 0,
        Bottom = 1,
        Medium = 2,
        Top = 3,
        TopMost = 4,
    }
	
    [ObjectSystem]
    public class UIManageComponentAwakeSystem : AwakeSystem<UIManageComponent>
    {
        public override void Awake(UIManageComponent self)
        {
            self.Awake();
        }
    }

    [ObjectSystem]
    public class UIManageComponentLoadSystem : LoadSystem<UIManageComponent>
    {
        public override void Load(UIManageComponent self)
        {
            self.Load();
        }
    }
    
    public class UIManageComponent : Component
    {
        private GameObject Root;
		private readonly Dictionary<string, IUIFactory> UiTypes = new Dictionary<string, IUIFactory>();
		private readonly Dictionary<string, UI> uis = new Dictionary<string, UI>();
		
		public void Awake()
		{
			this.Root = GameObject.Find("Global/UI/");
			
			this.Load();
			
			InitializeLayer();
		}

		#region 析构

		public override void Dispose()
		{
			if (this.IsDisposed)
			{
				return;
			}

			base.Dispose();

			foreach (string type in uis.Keys.ToArray())
			{
				UI ui;
				if (!uis.TryGetValue(type, out ui))
				{
					continue;
				}
				uis.Remove(type);
				ui.Dispose();
			}

			this.UiTypes.Clear();
			this.uis.Clear();
		}


		#endregion

		#region 加载

		public void Load()
		{
			UiTypes.Clear();
            
			Type[] types = ETModel.Game.Hotfix.GetHotfixTypes();

			foreach (Type type in types)
			{
				object[] attrs = type.GetCustomAttributes(typeof (UIFactoryAttribute), false);
				if (attrs.Length == 0)
				{
					continue;
				}

				UIFactoryAttribute attribute = attrs[0] as UIFactoryAttribute;
				if (UiTypes.ContainsKey(attribute.Type))
				{
					Log.Debug($"已经存在同类UI Factory: {attribute.Type}");
					throw new Exception($"已经存在同类UI Factory: {attribute.Type}");
				}
				object o = Activator.CreateInstance(type);
				IUIFactory factory = o as IUIFactory;
				if (factory == null)
				{
					Log.Error($"{o.GetType().FullName} 没有继承 IUIFactory");
					continue;
				}
				this.UiTypes.Add(attribute.Type, factory);
			}
		}

		#endregion

		#region 初始化层

		private void InitializeLayer()
		{
			for (var i = 0; i < this.Root.transform.childCount; i++)
			{
				var transform = this.Root.transform.GetChild(i);

				if (transform.GetComponent<Canvas>() == null) continue;

				foreach (UILayer layer in Enum.GetValues(typeof(UILayer)))
				{
					var gameObject = new GameObject(layer.ToString());

					var rect = gameObject.AddComponent<RectTransform>();

					gameObject.AddComponent<CanvasRenderer>();

					gameObject.transform.SetParent(transform, false);

//					rect.anchorMin = Vector2.zero;
//
//					rect.anchorMax = Vector2.one;
//
//					rect.localScale = Vector3.one;
//
//					rect.anchoredPosition3D = Vector3.zero;
//
//					rect.offsetMax =Vector2.zero;
//					
//					rect.offsetMin =Vector2.zero;
				}
			}
		}

		#endregion

		#region 创建UI

		private UI CreateUI(string type)
		{
			UI ui = UiTypes[type].Create(this.GetParent<Scene>(), type, Root);

			uis.Add(type, ui);

			return ui;
		}

		public UI Create(string type, UILayer layer)
		{
			try
			{
				var ui = CreateUI(type);

				var cavasName = ui.GameObject.GetComponent<CanvasConfig>().CanvasName;

				ui.GameObject.transform.SetParent(this.Root.Get<GameObject>(cavasName).transform.Find(layer.ToString()), false);

				return ui;
			}
			catch (Exception e)
			{
				throw new Exception($"{type} UI 错误: {e}");
			}
		}

		public UI Create(string type)
		{
			try
			{
				var ui = CreateUI(type);

				string cavasName = ui.GameObject.GetComponent<CanvasConfig>().CanvasName;

				ui.GameObject.transform.SetParent(this.Root.Get<GameObject>(cavasName).transform, false);

				return ui;
			}
			catch (Exception e)
			{
				throw new Exception($"{type} UI 错误: {e}");
			}
		}

		public UI Create(string type, Transform parent)
		{
			try
			{
				var ui = CreateUI(type);

				ui.GameObject.transform.SetParent(parent, false);
				
				return ui;
			}
			catch (Exception e)
			{
				throw new Exception($"{type} UI 错误: {e}");
			}
		}

		#endregion

		#region 基础操作
		
		/// <summary>
		/// 刷新父节点
		/// </summary>
		/// <param name="root"></param>
		public void UpdateRoot(string root = "Global/UI/")
		{
			this.Root = GameObject.Find(root);
		}

		public void Add(string type, UI ui)
		{
			this.uis.Add(type, ui);
		}

		public void Remove(string type)
		{
			UI ui;
			if (!uis.TryGetValue(type, out ui))
			{
				return;
			}
			UiTypes[type].Remove(type);
			uis.Remove(type);
			ui.Dispose();
		}

		public void RemoveAll()
		{
			foreach (string type in this.uis.Keys.ToArray())
			{
				UI ui;
				if (!this.uis.TryGetValue(type, out ui))
				{
					continue;
				}
				this.uis.Remove(type);
				ui.Dispose();
			}
		}

		public UI Get(string type)
		{
			UI ui;
			this.uis.TryGetValue(type, out ui);
			return ui;
		}

		public List<string> GetUITypeList()
		{
			return new List<string>(this.uis.Keys);
		}

		#endregion
    }
}