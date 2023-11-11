using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeamOS.Common.Api.Interfaces;
public interface IEndpoint<TRequest, TResponse>
{
    Task<TResponse> ExecuteAsync(TRequest req, CancellationToken ct);
}
