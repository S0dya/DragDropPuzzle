using System.Threading;
using Cysharp.Threading.Tasks;

namespace PT.Logic.PersistentScene.Loading
{
    public interface ILoadingStep
    {
        UniTask Load(CancellationToken token);
    }
}