namespace ClickAndCollect.Models.Classes
{
    public class Cart : IDisposable
    {
       private bool _disposed = false;
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

        public void RemoveCartline(CartLine line)
        {
            if (lines.Contains(line))
            {
                lines.Remove(line);
            }
            else
            {
                throw new ArgumentException("Cartline not found in cart");
            }
        }

        public void Dispose()
        {
            if (!_disposed) 
            {
                _disposed = true;
                foreach (CartLine line in lines) 
                {
                    line.Dispose();
                }
                GC.SuppressFinalize(this);
            }
        }

        public Cart() { }

        ~Cart() 
        {
            Dispose();
        }

    }
}
