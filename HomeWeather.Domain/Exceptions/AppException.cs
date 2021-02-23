using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HomeWeather.Domain.Exceptions
{
    [Serializable]
    public class AppException : Exception
    {
        public int StatusCode { get; set; }

        public AppException(string message)
            : base(message)
        {
        }

        public AppException()
        {
        }

        protected AppException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("Error", StatusCode);
        }
    }
}
