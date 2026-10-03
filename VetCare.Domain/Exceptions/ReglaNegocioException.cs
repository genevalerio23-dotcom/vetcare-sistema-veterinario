using System;

namespace VetCare.Domain.Exceptions;

public sealed class ReglaNegocioException : Exception
{
    public ReglaNegocioException(string mensaje) : base(mensaje)
    {
    }
}