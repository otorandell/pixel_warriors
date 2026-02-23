using UnityEngine;

namespace PixelWarriors
{
    public interface IScreen
    {
        void Build(Transform canvasParent);
        void Show();
        void Hide();
        void Destroy();
    }
}
