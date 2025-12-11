using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentApi.Application.Interfaces.Services
{
    public interface IJwtTokenService
    {
        string GenerateToken(int userId);
    }
}
