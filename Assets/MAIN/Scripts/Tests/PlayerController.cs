using System;
using Photon.Pun;
using UnityEngine;

namespace MAIN.Scripts.Tests
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float speedMove = 5;
        [SerializeField] private float speedRotate = 5;
        [SerializeField] private GameObject spellPrefab = null;
        [SerializeField] private float forceSpell = 100;

        private Transform _transform;

        private void Awake()
        {
            _transform = transform;
        }

        void Update()
        {
            Move();
            Rotate();

            if (Input.GetKeyDown(KeyCode.Space))
            {;
                Attack();
            }
        }

        private void Move()
        {
            float x = Input.GetAxis("Horizontal");
            float y = Input.GetAxis("Vertical");
            _transform.position = Vector3.MoveTowards(_transform.position,
                _transform.position + _transform.forward * y + _transform.right * x,
                Time.deltaTime * speedMove);
        }

        private void Rotate()
        {
            if (Input.GetMouseButton(0))
            {
                transform.Rotate(-Input.GetAxis("Mouse Y") * speedRotate, Input.GetAxis("Mouse X") * speedRotate, 0);
                transform.rotation = Quaternion.Euler(transform.eulerAngles.x,transform.eulerAngles.y,0);
            }
        }

        private void Attack()
        {
            GameObject playerGO = PhotonNetwork.Instantiate("Spell", transform.position + transform.forward, Quaternion.identity);
            playerGO.GetComponent<Rigidbody>().AddForce(transform.forward * forceSpell);
        }
    }
}
