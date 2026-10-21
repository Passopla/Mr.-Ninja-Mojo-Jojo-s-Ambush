using UnityEngine;
using UnityEngine.UI; // We need this line to use the 'Image' component in our script

public class DifficultyController : MonoBehaviour
{
    public event System.Action<int> OnDifficultyChanged;

    [Header("Sprites")]
    [Tooltip("Drag your '+' sprite here.")]
    public Sprite plusSprite;
    [Tooltip("Drag your blank button sprite here.")]
    public Sprite blankSprite;

    [Header("Indicators")]
    [Tooltip("Drag your three indicator Image objects here in order.")]
    public Image[] difficultyIndicators; // An array to hold our 3 images

    // This variable will hold the current difficulty level (1, 2, or 3)
    private int currentDifficulty;

    // Constants to make the code cleaner and prevent errors
    private const int maxDifficulty = 3;
    private const int minDifficulty = 1;

    void Start()
    {
        // Let's start the game at the highest difficulty by default
        currentDifficulty = minDifficulty;
        UpdateIndicatorSprites();
        OnDifficultyChanged?.Invoke(currentDifficulty);
    }

    // This is a PUBLIC function that the Right Arrow Button will call.
    public void IncreaseDifficulty()
    {
        // Only increase if we are not already at the max
        if (currentDifficulty < maxDifficulty)
        {
            currentDifficulty++;
            UpdateIndicatorSprites();
            OnDifficultyChanged?.Invoke(currentDifficulty);
        }
    }

    // This is a PUBLIC function that the Left Arrow Button will call.
    public void DecreaseDifficulty()
    {
        // Only decrease if we are not already at the min
        if (currentDifficulty > minDifficulty)
        {
            currentDifficulty--;
            UpdateIndicatorSprites();
            OnDifficultyChanged?.Invoke(currentDifficulty);
        }
    }

    // This private function contains YOUR logic for swapping the sprites.
    private void UpdateIndicatorSprites()
    {
        // We loop through our three indicator images.
        // The images are numbered 0, 1, 2 in the array.
        for (int i = 0; i < difficultyIndicators.Length; i++)
        {
            // We check if the indicator's number (i + 1) is less than or equal to the current difficulty.
            // Example: If difficulty is 2, this will be TRUE for indicator 0 and 1, but FALSE for 2.
            if ((i + 1) <= currentDifficulty)
            {
                // If it is, set its sprite to the 'plus' sprite.
                difficultyIndicators[i].sprite = plusSprite;
            }
            else
            {
                // Otherwise, set its sprite to the 'blank' sprite.
                difficultyIndicators[i].sprite = blankSprite;
            }
        }
    }
}