using System;
using Core;

namespace Core.Interfaces;

public interface IOnvif
{
  //Task<Position> GetPositionAsync(CancellationToken token);
  Task SetPositionAsync(string ip, string username, string password,Position position, CancellationToken token);
}

