﻿namespace Y.Domain.Exceptions;

public class UserNotFoundException : Exception
{
    public UserNotFoundException() : base() { }

    public UserNotFoundException(string message) : base(message) { }
}
