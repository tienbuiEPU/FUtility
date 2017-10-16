using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FUtilityApi.Models
{
    public class DefaultResponse
    {
        public Meta meta { get; set; }
        public object data { get; set; }

        public object metadata { get; set; }
        public object metadatainfo { get; set; }
    }

    public class Meta
    {
        public int error_code { get; set; }
        public string error_message { get; set; }

        public Meta()
        { }

        public Meta(int errorCode, string errorMessage)
        {
            this.error_code = errorCode;
            this.error_message = errorMessage;
        }
    }
}