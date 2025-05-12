using UnityEngine;
using UnityEngine.UI;

namespace UISystem.Buttons
{
    public abstract class ButtonBase: MonoBehaviour
    {
        private Button _button;
        
        public Button Button
        {
            get
            {
                if (_button == null)
                {
                    _button = GetComponent<Button>();
                }
                return _button;
            }
        }

        private void Awake()
        {
            SetUp();
        }

        protected virtual void SetUp()
        {
            if (Button == null)
            {
                Debug.LogError("Button component is missing on " + gameObject.name);
                return;
            }
            
            Button.onClick.AddListener(OnButtonClick);
        }
        
        protected virtual void OnButtonClick()
        {
            Debug.Log("Button clicked: " + gameObject.name);
        }
        
        protected virtual void UnsubscribeEvents()
        {
            if (Button != null)
            {
                Button.onClick.RemoveListener(OnButtonClick);
            }
        }
        
        protected virtual void OnDestroy()
        {
            UnsubscribeEvents();
        }
        
        protected virtual void OnDisable()
        {
            UnsubscribeEvents();
        }
    }
}