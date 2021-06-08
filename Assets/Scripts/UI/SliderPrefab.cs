using Factory.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Factory.UI {
    public class SliderPrefab : MonoBehaviour {
        public Slider slider;
        public Image foreground;
        public Image background;
        public Color backgroundColor;
        public Color foregroundColor;
        public bool interactable;

        public bool useTimer;
        public float time;
        public bool loop;
        Timer timer;

        private void Awake() {
            SetInteractable();
            SetSliderColor();
            SetTimer();
        }

        public void SetInteractable() {
            slider.interactable = interactable;
            slider.transform.GetChild(2).gameObject.SetActive(interactable);
        }

        public void SetSliderColor() {
            foreground.color = foregroundColor;
            background.color = backgroundColor;
        }

        public void SetTimer() {
            if (!useTimer) return;

            if (timer == null) timer = gameObject.AddComponent<Timer>();
            timer.StartUp(time, loop);
        }

        public void SetSlider(float percentageFull) {
            if (percentageFull > 1 || percentageFull < 0) Debug.LogWarning($"{percentageFull} is not a  valid input");
            slider.value = percentageFull;
        }
    }
}
