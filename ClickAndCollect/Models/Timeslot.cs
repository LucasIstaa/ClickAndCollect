namespace ClickAndCollect.Models
{
    public class Timeslot
    {
        private int timeslotid;
        private TimeOnly start;
        private TimeOnly end;
        private Store store;
        private Order[] orders = new Order[10];
        private int ordernumber = 0;

        public int TimeslotId
        {
            get { return timeslotid; }
            set { timeslotid = value; }
        }

        public TimeOnly Start
        {
            get { return start; }
            set { start = value; }
        }

        public TimeOnly End
        {
            get { return end; }
            set { end = value; }
        }

        public Store Store
        {
            get { return store; }
            set { store = value; }
        }

        public int OrderNumber 
        {
            get { return ordernumber; }
        }

        public bool AddOrder(Order order) 
        {
            if (ordernumber <= 9)
            {
                orders[ordernumber] = order;
                ordernumber++;
                return true;
            }
            else 
            {
                throw new ArgumentException("Max Order attained for timeslot");
            }

            return false;
        }

        public Timeslot(TimeOnly start, TimeOnly end, Store store) 
        {
            this.Start = start;
            this.End = end;
            this.Store = store;
            store.AddTimeslot(this);
        }

        public Timeslot(int id,TimeOnly start, TimeOnly end, Store store) : this(start, end, store) 
        {
            this.TimeslotId = id;
        }

        public override bool Equals(object? obj)
        {
            return obj is Timeslot timeslot &&
                   TimeslotId == timeslot.TimeslotId;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(TimeslotId);
        }
    }
}
