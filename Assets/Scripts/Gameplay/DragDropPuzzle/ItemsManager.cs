using System;
using PT.GameplayAdditional.Input;
using PT.Logic.Configs;
using Zenject;

namespace Gameplay.DragDropPuzzle
{
    public class ItemsManager : IInitializable, IDisposable
    {
        [Inject] private GameConfig _gameConfig;
        [Inject] private InputManager _inputManager;
        [Inject] private DraggableItem _draggableItem;

        public void Initialize()
        {
            _inputManager.OnDrag += OnDrag;
            _inputManager.OnRelease += OnRelease;
        }
        public void Dispose()
        {
            
        }

        private void OnDrag()
        {
            //handle dragging. make sure to check its happening WHILE the handItemsView was pressed
            
            //if the handItemView was pressed, dragging started, we need to make sure all the HandItemsView buttons are not clickable
        }

        private void OnRelease()
        {
            //handle releasing the objecti f it was picked and moved to the right place
            
            // if not - put it back to the HandItemsView
        }
        
        
    }
}