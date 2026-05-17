namespace ClickAndCollect.Models.Classes
{
    public class Cart
    {
       
        private List<CartLine> lines = new List<CartLine>();


        public List<CartLine> Lines 
        {
            get { return lines; }
            set { lines = value; }
        }

        public void AddCartline(CartLine line) 
        {
            if (!lines.Contains(line))
            {
                lines.Add(line);
            }
            else 
            {
                throw new ArgumentException("Cartline already in list of cart");
            }
        }

        public Cart() { }

    }
}
