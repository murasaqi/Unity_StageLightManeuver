using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StageLightManeuver
{
    [ExecuteAlways]
    public class SmoothLookAt : MonoBehaviour
    {

        public Vector3 initialRotation = Vector3.zero;
        public Transform target;
        private Vector3 panVelocity;
        [Range(0.001f, 0.1f)] public float speed = 0.02f;
        private float _maxSpeed = float.PositiveInfinity;

        // Start is called before the first frame update
        void Start()
        {
            Init();
        }

        public void Init()
        {
            transform.rotation = Quaternion.Euler(initialRotation);
        }

        private void OnDestroy()
        {
            Init();
        }

        private void OnDisable()
        {
            Init();
        }

        private void OnEnable()
        {
            Init();
        }


        // Update is called once per frame
        void Update()
        {
            if(target == null ) return;
            var targetRotation = Quaternion.LookRotation(target.position - transform.position);
            var startRotation = transform.rotation;
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, speed * Time.fixedTime);
        }
    }
}