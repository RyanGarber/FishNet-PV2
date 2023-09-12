using UnityEngine;

using FishNet.Object;
using FishNet.Object.Prediction;
using FishNet.Transporting;

namespace FishNet_PV2
{
    public class Player : NetworkBehaviour
    {
        [SerializeField] private CharacterController _controller;

        private ReplicateData? _replicateDataLastTick;
        
        /// <summary>
        /// Send replicate and reconcile.
        /// </summary>
        public override void OnStartNetwork() => TimeManager.OnTick += OnTick;

        public override void OnStopNetwork() => TimeManager.OnTick -= OnTick;
        private void OnTick()
        {
            // Send replicate.
            ReplicateData data = SendInput ? new ReplicateData {Movement = Input} : default;
            if (SendInput) Debug.Log($"SENT: {data.Movement}");
            Replicate(data);

            // Send reconcile.
            if (IsServer) Reconcile(new ReconcileData { Position = transform.position });
        }

        /// <summary>
        /// Replicate.
        /// </summary>
        [ReplicateV2]
        private void Replicate(ReplicateData data, ReplicateState state = ReplicateState.Invalid, Channel channel = Channel.Unreliable)
        {
            if (SendInput) Debug.Log($"RECEIVED: {data.Movement} (ReplicateState.{state})");
            
            if (state is ReplicateState.UserCreated or ReplicateState.ReplayedUserCreated)
            {
                _replicateDataLastTick = data;
            }
            else if (_replicateDataLastTick.HasValue)
            {
                uint tick = data.GetTick();
                data = _replicateDataLastTick.Value;
                data.SetTick(tick);
            }
            
            _controller.Move(data.Movement * 10f * (float)TimeManager.TickDelta);
        }

        /// <summary>
        /// Reconcile.
        /// </summary>
        [ReconcileV2]
        private void Reconcile(ReconcileData data, Channel channel = Channel.Unreliable)
        {
            _controller.enabled = false;
            transform.position = data.Position;
            _controller.enabled = true;
        }
        
        /// <summary>
        /// Whether to send input.
        /// </summary>
        private bool SendInput => Overview.Bug switch
        {
            Bug.ServerInputBug => IsServer,
            Bug.ObserverReplicateBug => IsOwner
        };
        
        /// <summary>
        /// Fake input.
        /// </summary>
        private Vector3 Input => ((TimeManager.TicksToTime() * 2d) % 8d) switch
        {
            < 1d => Vector3.left,
            < 2d => Vector3.zero,
            < 3d => Vector3.right,
            < 4d => Vector3.zero,
            < 5d => Vector3.right,
            < 6d => Vector3.zero,
            < 7d => Vector3.left,
            _ => Vector3.zero
        };
    }
    
    public struct ReplicateData : IReplicateData
    {
        public Vector3 Movement;

        private uint _tick;
        public uint GetTick() => _tick;
        public void SetTick(uint tick) => _tick = tick;
        public void Dispose() { }
    }

    public struct ReconcileData : IReconcileData
    {
        public Vector3 Position;
            
        private uint _tick;
        public uint GetTick() => _tick;
        public void SetTick(uint tick) => _tick = tick;
        public void Dispose() { }
    }
}