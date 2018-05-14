using System;
using ETModel;
using UnityEngine;

namespace ETHotfix
{
    public abstract class AUIFactory : IUIFactory
    {
        public virtual UI Create(Scene scene, string type, GameObject parent)
        {
            try
            {
                ResourcesComponent resourcesComponent = ETModel.Game.Scene.GetComponent<ResourcesComponent>();
                
                resourcesComponent.LoadBundle($"{type}.unity3d");

                GameObject bundleGameObject = (GameObject) resourcesComponent.GetAsset($"{type}.unity3d", $"{type}");

                GameObject gameobject = UnityEngine.Object.Instantiate(bundleGameObject);

                gameobject.layer = LayerMask.NameToLayer(LayerNames.UI);

                UI ui = ComponentFactory.Create<UI, GameObject>(gameobject);

                return ui;
            }
            catch (Exception e)
            {
                Log.Error(e);

                return null;
            }
        }

        public virtual void Remove(string type)
        {
            ETModel.Game.Scene.GetComponent<ResourcesComponent>().UnloadBundle($"{type}.unity3d");
        }
    }
}