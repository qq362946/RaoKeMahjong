using ETModel;

namespace ETHotfix
{
    [ObjectSystem]
    public class UIRaoKeLoginComponentSystem : AwakeSystem<UIRaoKeLoginComponent>
    {
        public override void Awake(UIRaoKeLoginComponent self)
        {
            self.Awake();
        }
    }
    
    public class UIRaoKeLoginComponent : Component
    {
        public void Awake()
        {
            
        }
    }
}