using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;

namespace DoshiiDotNetIntegration.Helpers
{
    public class AutoMapperGenericsHelper<TSource, TDestination>
    {
        public static TDestination ConvertToDBEntity(TSource model)
        {
            return Mapper.Map<TSource, TDestination>(model);
        }
    }
}
