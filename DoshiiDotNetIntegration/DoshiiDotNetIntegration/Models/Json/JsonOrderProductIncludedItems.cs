namespace DoshiiDotNetIntegration.Models.Json
{
    internal class JsonOrderProductIncludedItems : JsonOrderProduct
    {
        public override bool ShouldSerializeDescription()
        {
            return false;
        }

        public bool ShouldSerializeMenuDir()
        {
            return false;
        }

        public override bool ShouldSerializeIncludedItems()
        {
            return false;
        }

        public override bool ShouldSerializeUuid()
        {
            return false;
        }

        public bool ShouldSerializeType()
        {
            return false;
        }

        public bool ShouldSerializeProductSurcounts()
        {
            return false;
        }

        public bool ShouldSerializeTags()
        {
            return false;
        }
    }
}
