using UnityEngine;
using Zenject;

namespace PT.Tools.Factories
{
    public interface IFactoryZenject
    {
        GameObject InstantiateObject(GameObject obj, Transform parent);
    }

    public class FactoryZenject : IFactoryZenject
    {
        private readonly DiContainer _container;

        public FactoryZenject(DiContainer container) => _container = container; 

        public GameObject InstantiateObject(GameObject obj, Transform parent) 
            => _container.InstantiatePrefab(obj, parent);
    }
}
