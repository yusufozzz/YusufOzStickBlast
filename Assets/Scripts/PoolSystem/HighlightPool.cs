using HighlightSystem;

namespace PoolSystem
{
    public class HighlightPool: PoolBase<Highlight>
    {
        protected override void OnReturnObject(Highlight pooledObject)
        {
            base.OnReturnObject(pooledObject);
            pooledObject.StopAnimation();
        }
    }
}