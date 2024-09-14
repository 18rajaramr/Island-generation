using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
   private Slider slider;
   public float FillSpeed = 0.5f; // The speed at which the slider progresses
   private float TargetProgress = 0; // The desired progress

   private void Awake()
   {
        // Get the slider component attached to the GameObject
        slider = gameObject.GetComponent<Slider>();
   }
   
   void Start()
   {

   }
   
   void Update()
   {
        // If the slider value is less than the target, gradually increase it
        if (slider.value < TargetProgress)
        {
            slider.value += FillSpeed * Time.deltaTime; // Smooth increment over time
        }
        // Make sure we don't overshoot the target progress
        if (slider.value > TargetProgress)
        {
            slider.value = TargetProgress;
        }
   }
   
   public void IncrementProgress(float newProgress)
   {
        // Set the target progress value, clamping it between 0 and 1 (100%)
        TargetProgress = Mathf.Clamp(newProgress, 0, 1f);
   }
}
