using System;
using System.Threading.Tasks;
using ETModel;
using UnityEngine;

namespace ETHotfix
{
    [ObjectSystem]
    public class SceneHelperAwakeSystem : AwakeSystem<SceneHelperComponent>
    {
        public override void Awake(SceneHelperComponent self)
        {
            self.Awake();
        }
    }

    public class SceneHelperComponent : Component
    {
        public static SceneHelperComponent Instance { get; private set; }

        public Session Session;

        public void Awake()
        {
            Instance = this;

            Session = null;
        }

        #region 网络操作

        /// <summary>
        /// 创建验证服务器连接
        /// </summary>
        public Session CreateRealmSession()
        {
            var connetEndPoint = NetworkHelper.ToIPEndPoint(GlobalConfigComponent.Instance.GlobalProto.Address);

            return ComponentFactory.Create<Session, ETModel.Session>(ETModel.Game.Scene.GetComponent<NetOuterComponent>().Create(connetEndPoint));
        }

        /// <summary>
        /// 创建网关服务器连接
        /// </summary>
        /// <param name="address">连接IP和端口</param>
        /// <param name="key">连接秘钥</param>
        /// <returns></returns>
        public async Task<Session> CreateGateSession(string address,long key)
        {
            try
            {
                // 连接到Gate服务器
            
                var connetEndPoint = NetworkHelper.ToIPEndPoint(address);
                
                var gateSession = ETModel.Game.Scene.GetComponent<NetOuterComponent>().Create(connetEndPoint);

                Session = ComponentFactory.Create<Session, ETModel.Session>(gateSession);
            
                // 请求进入Gate服务器

                var gateResponse = (EntherGateResponse) await Session.Call(new EntherGateRequest() {Key = key});

                if (gateResponse.Error != 0)
                {
                    Session.Dispose();

                    Session = null;
                    
                    Debug.Log("无法进入到服务器、原因EntherGateRequest");
                }
                else
                {
                    // 添加心跳组件

                    if (Game.Scene.GetComponent<PingComponent>() == null)
                    {
                        Game.Scene.AddComponent<PingComponent, long, Session, Action>(4000, Session, null);
                    }
                }
            }
            catch (Exception e)
            {
                Session?.Dispose();

                Session = null;
            }

            return Session;
        }

        #endregion

        #region 组件

        /// <summary>
        /// 事件管理组件
        /// </summary>
        public MonoEventComponent MonoEvent => ETModel.Game.Scene.GetComponent<MonoEventComponent>() ?? ETModel.Game.Scene.AddComponent<MonoEventComponent>();

        /// <summary>
        /// 场景管理组件
        /// </summary>
        public SceneManagerComponent SceneManager => ETModel.Game.Scene.GetComponent<SceneManagerComponent>() ?? ETModel.Game.Scene.AddComponent<SceneManagerComponent>();

        #region 移除组件

        /// <summary>
        /// 移除Mono层组件
        /// </summary>
        /// <typeparam name="TK"></typeparam>
        public void RemoveComponentByMono<TK>() where TK : ETModel.Component
        {
            ETModel.Game.Scene.RemoveComponent<TK>();
        }

        /// <summary>
        /// 移除Hotfix层组件
        /// </summary>
        /// <typeparam name="TK"></typeparam>
        public void RemoveComponentByHotfix<TK>() where TK : Component
        {
            Game.Scene.RemoveComponent<TK>();
        }

        #endregion

        #endregion

        public override void Dispose()
        {
            if (this.IsDisposed) return;

            base.Dispose();

            RemoveComponentByHotfix<PingComponent>(); // 移除心跳组件

            RemoveComponentByMono<MonoEventComponent>(); // 事件管理组件

            RemoveComponentByMono<SceneManagerComponent>(); // 场景管理组件

            this.Session?.Dispose();

            this.Session = null;

            Instance = null;
        }
    }
}