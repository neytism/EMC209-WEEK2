using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using Random = UnityEngine.Random;
using GNW2.InputData;

namespace GNW2.Player
{
    public class Player : NetworkBehaviour
    {
        [Networked] public Color playerColor { get; set; }
        [SerializeField] private float speed = 2f;
        
        private NetworkCharacterController _cc;
        private Renderer _ren;

        private void Awake()
        {
            _cc = GetComponent<NetworkCharacterController>();
            _ren = GetComponentInChildren<Renderer>();
        }

        public override void Spawned()
        {
            if (HasStateAuthority) // only the server set the color
            {
                playerColor = new Color(
                    Random.Range(0f, 1f),
                    Random.Range(0f, 1f),
                    Random.Range(0f, 1f),
                    1f
                );
            }
            
            UpdateColor();
        }

        private void UpdateColor()
        {
            if (_ren != null)
            {
                _ren.material.color = playerColor;
            }
        }

        public override void Render()
        {
            UpdateColor();
        }

        public override void FixedUpdateNetwork()
        {
           MoveHandler();
        }

        public void MoveHandler()
        {
            if (GetInput(out NetworkInputData data))
            {
                data.Direction.Normalize();
                _cc.Move(speed * data.Direction * Runner.DeltaTime);
            }
            
            if (data.Jump && _cc.Grounded) 
            {
                _cc.Jump();
            }
        }

        
        
    }
}

