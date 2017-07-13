using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pos.Helpers
{
    public static class InputValidation
    {
        public static bool TestIsInteger(string stringToTest)
        {
            var resultInt = 0;
            if (int.TryParse(stringToTest, out resultInt))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool TestIsDecimal(string stringToTest)
        {
            Decimal resultInt = 0;
            if (Decimal.TryParse(stringToTest, out resultInt))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
