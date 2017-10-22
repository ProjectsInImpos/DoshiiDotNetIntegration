namespace DoshiiDotNetIntegration.Models.Json
{
    internal class JsonOrderProductIncludedItems : JsonOrderProduct
    {
        public override bool ShouldSerializeDescription()
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

        public override bool ShouldSerializeType()
        {
            return false;
        }

        public override bool ShouldSerializeProductSurcounts()
        {
            return false;
        }

        public override bool ShouldSerializeTags()
        {
            return false;
        }

        public override bool ShouldSerializeTotalAfterSurcounts()
        {
            return false;
        }

        public override bool ShouldSerializeTotalBeforeSurcounts()
        {
            return false;
        }
    }
}
