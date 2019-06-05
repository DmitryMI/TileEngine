using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Objects.Equipment.Power
{
    public class PowerStorageModule : Equipment
    {
        [SerializeField] private float _powerStored = 0;
        [SerializeField] private float _powerCapacity = 10000;
        [SerializeField] private float _maximumOutput;

        protected override bool Transparent
        {
            get { return true; }
        }

        protected override bool CanWalkThrough
        {
            get { return false; }
        }

        protected override bool PassesGas
        {
            get { return true; }
        }

        public override string DescriptiveName
        {
            get { return "Power storage module"; }
        }

        protected override void Update()
        {
            base.Update();

            if (isServer && ServerController.Ready)
            {
                float send = Mathf.Min(_maximumOutput, _powerStored);
                ProcessPower(send);
            }
        }

        [Server]
        private void ProcessPower(float watts)
        {
            IPowerSender connector = TileController.Find<IPowerSender>(Cell.x, Cell.y);

            IPowerConsumer[] consumers = connector?.FindConsumers();

            if (consumers != null)
            {
                float portion = watts / consumers.Length;

                foreach (var consumer in consumers)
                {
                    Debug.DrawRay(transform.position, consumer.gameObject.transform.position - transform.position,
                        Color.white);

                    if(consumer.AmountOfNeededPower < portion)
                        //consumer.SendPower(consumer.AmountOfNeededPower);
                        SendPowerTo(consumer, consumer.AmountOfNeededPower);
                    else
                        //consumer.SendPower(portion);
                        SendPowerTo(consumer, portion);
                    
                }
            }

        }

        private void SendPowerTo(IPowerConsumer consumer, float amount)
        {
            if (_powerStored >= amount)
            {
                consumer.SendPower(amount);
                _powerStored -= amount;
            }

        }
    }
}
