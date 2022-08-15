namespace DefaultNamespace
{
    public class CellInfo
    {
        public enum DirectionMode
        {
            Wait = 0,      //wait some cicles before doing fagocitation
            Left = 1,
            Right = 2,
            Up = 3,
            Down = 4,
            All = 5        //Fagocitation
        }
        public int Shape  //0:None, 1:ellipse  2:triangle  3:rectangle
        {
            get => shape;
            set => shape = value;
        }

        public int Tt    //Type of Triangle ->  1:<  2:>  3:/\  4:\/ 
        {
            get => tt;
            set => tt = value;
        }

        public DirectionMode Direction  //1:left   2:right   3:up   4:down
        {
            get => direction;
            set => direction = value;
        }

        public int CBack
        {
            get => cBack;
            set => cBack = value;
        }

        public int CFore
        {
            get => cFore;
            set => cFore = value;
        }

        private int shape, tt;
        private DirectionMode direction;
        private int cBack, cFore;
    }
}