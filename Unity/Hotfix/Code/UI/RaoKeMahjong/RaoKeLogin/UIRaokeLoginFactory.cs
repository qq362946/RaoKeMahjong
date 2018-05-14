using ETModel;
using UnityEngine;

namespace ETHotfix
{
    [UIFactory(UIType.UIRaoKeLogin)]
    public class UIRaokeLoginFactory : AUIFactory
    {
        public override UI Create(Scene scene, string type, GameObject parent)
        {
            var ui = base.Create(scene, type, parent);

            ui.AddComponent<UIRaoKeLoginComponent>();

            return ui;
        }
    }
}