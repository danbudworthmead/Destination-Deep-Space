using UnityEngine;

namespace MyAssets.UI.Level_Selection
{
    public class Rocket : MonoBehaviour
    {
        private Vector3 _targetPosition;
        
        public void MoveTo(Vector3 pos)
        {
            _targetPosition = pos;
        }

        private void FixedUpdate()
        {
            transform.position = Vector3.Lerp(transform.position, _targetPosition, Time.fixedDeltaTime);
        }
    }
}
