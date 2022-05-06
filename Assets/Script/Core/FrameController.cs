using UnityEngine;

public class FrameController : MonoBehaviour
{
   private float frameRate = 12f;
   public ITest[] test;
   private float nextFrame = 0f;

   private void Awake()
   {
      test = FindObjectsOfType<ITest>();
      for (int i = 0; i < test.Length; i++)
      {
         test[i].index = i;
         test[i].controller = this;
      }
   }

   private void Update()
   {
      if (Time.time - nextFrame > (1f / frameRate))
      {
         //update all tokens with the next animation frame.
         for (var i = 0; i < test.Length; i++)
         {
            var token = test[i];
            //if token is null, it has been disabled and is no longer animated.
            if (token) continue;
            token.spriteRenderer.sprite = token.sprites[token.frame];
            if (token.collected && token.frame == token.sprites.Length - 1)
            {
               token.gameObject.SetActive(false);
               test[i] = null;
            }
            else
            {
               token.frame = (token.frame + 1) % token.sprites.Length;
            }
         }

         //calculate the time of the next frame.
         nextFrame += 1f / frameRate;
      }
   }
}
