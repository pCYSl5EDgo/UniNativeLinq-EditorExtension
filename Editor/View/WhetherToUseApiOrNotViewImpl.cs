namespace UniNativeLinq.Editor
{
    internal sealed class WhetherToUseApiOrNotViewImpl : IWhetherToUseApiOrNotView
    {
        public WhetherToUseApiOrNotViewImpl()
        {
        }
        public void Draw(string name)
        {
            switch (name)
            {
                case "MinByUInt32":
                case "MinByUInt64":
                case "MinByInt32":
                case "MinByInt64":
                case "MinBySingle":
                case "MinByDouble":
                    break;
                case "MaxByUInt32":
                case "MaxByUInt64":
                case "MaxByInt32":
                case "MaxByInt64":
                case "MaxBySingle":
                case "MaxByDouble":
                    break;
            }
        }
    }
}
