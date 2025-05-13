using GameManagement;
using HighlightSystem;
using LevelSystem;

namespace PoolSystem
{
    public class HighlightPool: PoolBase<Highlight>
    {
        protected override void OnGetObject(Highlight pooledObject)
        {
            base.OnGetObject(pooledObject);
            var levelManager = ManagerType.Level.GetManager<LevelManager>();
            pooledObject.SetColor(levelManager.ActiveShapeColor);
        }

        protected override void OnReturnObject(Highlight pooledObject)
        {
            base.OnReturnObject(pooledObject);
            pooledObject.StopAnimation();
        }
    }
}