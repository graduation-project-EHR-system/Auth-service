using Data.Layer.Helper.SendEmail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Layer.Interfaces
{
    public interface IEmailSend
    {
        public Task SendEmailAsync(Email email);

    }
}
