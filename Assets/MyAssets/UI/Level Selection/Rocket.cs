using UnityEngine;

namespace MyAssets.UI.Level_Selection
{
    public class Rocket : MonoBehaviour
    {
        [SerializeField] private float _speed;
        private Vector3 _targetPosition;
        
        public void MoveTo(Vector3 pos)
        {
            _targetPosition = pos;
            
            // look at the target
            var norTar = (pos-transform.position).normalized;
            var angle = Mathf.Atan2(norTar.y,norTar.x)*Mathf.Rad2Deg;
            var rotation = new Quaternion
            {
                eulerAngles = new Vector3(0,0,angle-90)
            };
            transform.rotation = rotation;
        }

        private void FixedUpdate()
        {
            transform.position = Vector3.MoveTowards(transform.position, _targetPosition, Time.fixedDeltaTime * _speed);
        }
    }
}
