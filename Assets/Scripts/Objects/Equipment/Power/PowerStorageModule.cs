using UnityEngine;

namespace Assets.Scripts.Objects.Equipment.Power
{
    public class PowerStorageModule : TileObject
    {
        [SerializeField] private float _maximumOutput;

        protected override bool Transperent
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
            
            SendPower(_maximumOutput);
        }

        private void SendPower(float watts)
        {
            WireConnector connector = TileController.Find<WireConnector>(Cell.x, Cell.y);

            if (connector == null)
            {
                Debug.LogWarning("PSM is not connected to a wire!");
                return;
            }

            IPowerConsumer[] consumers = connector.FindConsumers();

            if (consumers != null)
            {
                float portion = watts / consumers.Length;

                foreach (var consumer in consumers)
                {
                    Debug.DrawRay(transform.position, consumer.gameObject.transform.position - transform.position,
                        Color.white);

                    if(consumer.AmountOfNeededPower < portion)
                        consumer.SendPower(consumer.AmountOfNeededPower);
                    else
                        consumer.SendPower(portion);
                    
                }
            }

        }
    }
}
